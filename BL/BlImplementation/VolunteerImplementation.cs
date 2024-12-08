namespace BlImplementation;
using BlApi;

using Helpers;
using System.Collections.Generic;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void Create(BO.Volunteer boVolunteer)
    {
        //DO.RoleType role = (DO.RoleType)boVolunteer.Role;
        //bool temp = (DO.RoleType.TryParse(boVolunteer.Role.ToString(), out role));

        //DO.DistanceType distanceType;
        //temp = (DO.DistanceType.TryParse(boVolunteer.TheDistanceType.ToString(), out distanceType));


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
        throw new NotImplementedException();
    }

    public BO.RoleType LogIn(string name, string password)
    {
        throw new NotImplementedException();
    }
    
    public BO.Volunteer Read(int id)
    {

        var doVolunteer = _dal.Volunteer.Read(id) ??
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does Not exist");
        //BO.RoleType role;
        //bool temp = (BO.RoleType.TryParse(doVolunteer.Role.ToString(), out role));

        //BO.DistanceType distanceType;
        //temp = (BO.DistanceType.TryParse(doVolunteer.TheDistanceType.ToString(), out distanceType));

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
            TotalHandled= 
        };
    }
    

    public IEnumerable<BO.VolunteerInList> ReadAll(bool? active, BO.FieldsVolunteerInList field = BO.FieldsVolunteerInList.Id)
        =>
    

    public void Update(int id, BO.Volunteer volunteer)
    {
        throw new NotImplementedException();
    }
}
