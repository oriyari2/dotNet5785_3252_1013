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
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does Not exist", ex);
        }

    }

    public BO.RoleType LogIn(string name, string password)
    {
        var volunteer = _dal.Volunteer.ReadAll(s => (s.Name == name) && (s.Password == password)).FirstOrDefault();

        if (volunteer==null)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with Name={name} does Not exist");
        }
        if(volunteer.Password!= password)
        {
            throw new BO.IncorrectValueException("incorrect password");
        }
        return (BO.RoleType)volunteer.Role;
    }

    public BO.Volunteer Read(int id)
    {
        
            var doVolunteer = _dal.Volunteer.Read(id)??
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does Not exist");

        return new BO.Volunteer()
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


    public IEnumerable<BO.VolunteerInList> ReadAll(bool? active, BO.FieldsVolunteerInList field = BO.FieldsVolunteerInList.Id)
    {
        // קבלת הרשימה מה-DAL
        var listVol = active != null
            ? _dal.Volunteer.ReadAll(s => s.Active == active)
            : _dal.Volunteer.ReadAll();

        // הגדרת המיון
        var sortedList = field switch
        {
            BO.FieldsVolunteerInList.Id => listVol.OrderBy(item => item.Id),
            BO.FieldsVolunteerInList.Name => listVol.OrderBy(item => item.Name),
            BO.FieldsVolunteerInList.TotalHandled => listVol.OrderBy(item => VolunteerManager.TotalCall(item.Id, DO.EndType.treated)),
            BO.FieldsVolunteerInList.TotalCanceled => listVol.OrderBy(item => VolunteerManager.TotalCall(item.Id, DO.EndType.self)),
            BO.FieldsVolunteerInList.TotalExpired => listVol.OrderBy(item => VolunteerManager.TotalCall(item.Id, DO.EndType.expired)),
            _ => listVol.OrderBy(item => item.Id) // ברירת מחדל במקרה שלא הוגדר שדה
        };

        // יצירת אובייקטים מסוג VolunteerInList
        var listSort = from item in sortedList
                       let call = VolunteerManager.GetCurrentCall(item.Id)
                       select new BO.VolunteerInList
                       {
                           Id = item.Id,
                           Name = item.Name,
                           Active = item.Active,
                           TotalHandled = VolunteerManager.TotalCall(item.Id, DO.EndType.treated),
                           TotalCanceled = VolunteerManager.TotalCall(item.Id, DO.EndType.self),
                           TotalExpired = VolunteerManager.TotalCall(item.Id, DO.EndType.expired),
                           CurrentCall = call,
                           TheCallType = call == null ? BO.CallType.None : VolunteerManager.GetCallType((int)call)
                       };

        return listSort;
    }

    public void Update(int id, BO.Volunteer volunteer)
    {
         
    }
}
