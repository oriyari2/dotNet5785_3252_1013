namespace BlImplementation;
using BlApi;

using Helpers;
using System.Collections.Generic;
using System.Linq.Expressions;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void Create(BO.Volunteer boVolunteer)
    {

        DO.Volunteer doVolunteer = new(boVolunteer.Id, boVolunteer.Name,
        boVolunteer.PhoneNumber, boVolunteer.Email, boVolunteer.Password, boVolunteer.Address, boVolunteer.Latitude,
        boVolunteer.Longitude, (DO.RoleType)boVolunteer.Role, boVolunteer.Active, boVolunteer.MaxDistance, (DO.DistanceType)boVolunteer.TheDistanceType);
        try
        {
            _dal.Volunteer.Create(doVolunteer);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} already exists", ex);
        }

    }

    public void Delete(int id)
    {

        var volunteer = Read(id);
        if (volunteer.IsProgress != null)
            throw new BO.cantDeleteItem($"Volunteer with ID={id} can't be deleted");
        try
        {
            _dal.Volunteer.Delete(id);
        }
        catch(DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} can't be deleted", ex);
        }
    }

    public BO.RoleType LogIn(string name, string password)
    {
        return (BO.RoleType)_dal.Volunteer.ReadAll(s => (s.Name == name) && (s.Password == password)).First().Role;
    }

    public BO.Volunteer Read(int id)
    {
        try
        {
            var doVolunteer = _dal.Volunteer.Read(id);

            return new()
            {
                Id = id,
                Name = doVolunteer.Name,
                PhoneNumber = doVolunteer.PhoneNumber,
                Email = doVolunteer.Email,
                Password = doVolunteer.Password,
                Address = doVolunteer.Address,
                Latitude = doVolunteer.Latitude,
                Longitude = doVolunteer.Longitude,
                Role = (BO.RoleType)doVolunteer.Role,
                Active = doVolunteer.Active,
                MaxDistance = doVolunteer.MaxDistance,
                TheDistanceType = (BO.DistanceType)doVolunteer.TheDistanceType,
                TotalHandled = VolunteerManager.TotalCall(id, DO.EndType.treated),
                TotalCanceled = VolunteerManager.TotalCall(id, DO.EndType.self),
                TotalExpired = VolunteerManager.TotalCall(id, DO.EndType.expired),
                IsProgress = VolunteerManager.callProgress(id)
            };
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does Not exist",ex);
        }
    }


    public IEnumerable<BO.VolunteerInList> ReadAll(bool? active, BO.FieldsVolunteerInList field = BO.FieldsVolunteerInList.Id)
    {
        var volList = _dal.Volunteer.ReadAll(s=> s.Active == active).OrderBy(field => field);
        retrun volList.OrderBy(field => field);
    }

    public void Update(int id, BO.Volunteer volunteer)
    {
        throw new NotImplementedException();
    }
}
