namespace BlImplementation;
using BlApi;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// VolunteerImplementation class that implements the IVolunteer interface.
/// Handles operations related to volunteers such as creating, updating, deleting, and reading volunteer data.
/// </summary>
internal class VolunteerImplementation : IVolunteer
{
    /// <summary>
    /// Data access layer for interacting with volunteer data.
    /// </summary>
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// Adds an observer that monitors updates for the entire list.
    /// </summary>
    /// <param name="listObserver">The callback action to be invoked on list updates.</param>
    public void AddObserver(Action listObserver) =>
        // Adds the provided observer for monitoring updates to the entire list.
        VolunteerManager.Observers.AddListObserver(listObserver);

    /// <summary>
    /// Adds an observer that monitors updates for a specific object.
    /// </summary>
    /// <param name="id">The unique identifier of the object to observe.</param>
    /// <param name="observer">The callback action to be invoked on object updates.</param>
    public void AddObserver(int id, Action observer) =>
        // Adds the provided observer for monitoring updates to the object identified by the specified ID.
        VolunteerManager.Observers.AddObserver(id, observer);

    /// <summary>
    /// Removes an observer that was monitoring updates for the entire list.
    /// </summary>
    /// <param name="listObserver">The callback action that was observing list updates.</param>
    public void RemoveObserver(Action listObserver) =>
        // Removes the provided observer from monitoring updates to the entire list.
        VolunteerManager.Observers.RemoveListObserver(listObserver);

    /// <summary>
    /// Removes an observer that was monitoring updates for a specific object.
    /// </summary>
    /// <param name="id">The unique identifier of the object being observed.</param>
    /// <param name="observer">The callback action that was observing the object updates.</param>
    public void RemoveObserver(int id, Action observer) =>
        // Removes the provided observer from monitoring updates to the object identified by the specified ID.
        VolunteerManager.Observers.RemoveObserver(id, observer);


    /// <summary>
    /// Creates a new volunteer in the system.
    /// </summary>
    /// <param name="boVolunteer">The business object representing the volunteer to be created.</param>
    /// <exception cref="BO.BlAlreadyExistsException">Thrown if a volunteer with the same ID already exists.</exception>
    public void Create(BO.Volunteer boVolunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        // Validate the volunteer's data using utility functions
        VolunteerManager.IsValidEmail(boVolunteer.Email); // Check if the email is valid
        VolunteerManager.IsValidID(boVolunteer.Id); // Check if the ID is valid
        VolunteerManager.IsValidPhoneNumber(boVolunteer.PhoneNumber); // Check if the phone number is valid
        double[] dis = VolunteerManager.GetCoordinates(boVolunteer.Address); // Get latitude and longitude from address
        string password = VolunteerManager.GenerateStrongPassword(); // Generate a strong password
        password = VolunteerManager.EncryptPassword(password);

        // Create the corresponding data object (DO) for the volunteer
        DO.Volunteer doVolunteer = new(boVolunteer.Id, boVolunteer.Name,
            boVolunteer.PhoneNumber, boVolunteer.Email, password, boVolunteer.Address, dis[0],
            dis[1], DO.RoleType.volunteer, boVolunteer.Active, boVolunteer.MaxDistance, (DO.DistanceType)boVolunteer.TheDistanceType);

        try
        {
            // Attempt to add the volunteer to the database
            lock (AdminManager.BlMutex)
                _dal.Volunteer.Create(doVolunteer);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            // Handle case when volunteer already exists in the database
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} already exists", ex);
        }
        VolunteerManager.Observers.NotifyListUpdated();//update list of volunteers and obserervers etc.
    }

    /// <summary>
    /// Deletes a volunteer from the system.
    /// </summary>
    /// <param name="id">The ID of the volunteer to be deleted.</param>
    /// <exception cref="BO.BlcantDeleteItem">Thrown if the volunteer cannot be deleted due to an ongoing assignment.</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the volunteer does not exist.</exception>
    public void Delete(int id)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        var volunteer = Read(id); // Check if volunteer exists by reading the volunteer's information
        if (volunteer.IsProgress != null) // If the volunteer has an ongoing assignment, prevent deletion
            throw new BO.BlcantDeleteItem($"Volunteer with ID={id} can't be deleted because he has a current call in progress");
        var allVolunteers = ReadAll(null);
        var manager = from item in allVolunteers
                      let Checkvolunteer = Read(item.Id).Role
                      where Checkvolunteer == BO.RoleType.manager
                      select item;
        if(!manager.Any(s=>s.Id != id))
            throw new BO.BlcantDeleteItem($"Volunteer with ID={id} can't be deleted because he is the last manager in the system");
        try
        {
            // Attempt to delete the volunteer from the database
            lock (AdminManager.BlMutex)
                _dal.Volunteer.Delete(id);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            // Handle case when the volunteer does not exist in the database
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does Not exist", ex);
        }
        VolunteerManager.Observers.NotifyListUpdated();//update list of volunteers and obserervers etc.
        
    }

    /// <summary>
    /// Logs in a volunteer by verifying their name and password.
    /// </summary>
    /// <param name="id">The id of the volunteer.</param>
    /// <param name="password">The password provided by the volunteer.</param>
    /// <returns>The role of the volunteer upon successful login.</returns>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the volunteer does not exist.</exception>
    /// <exception cref="BO.BlIncorrectValueException">Thrown if the password is incorrect.</exception>
    public BO.RoleType LogIn(int id, string password)
    {
        BO.Volunteer volunteer = Read(id); // Find volunteer by id, if not exist throw an expectation


        // Check if the password is correct by decrypting and comparing
        if (volunteer.Password != password)
        {
            // If password does not match, throw exception
            throw new BO.BlIncorrectValueException("incorrect password");
        }

        // Return the volunteer's role
        return (BO.RoleType)volunteer.Role;
    }

    /// <summary>
    /// Retrieves a volunteer's details by their ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer to be retrieved.</param>
    /// <returns>The business object representing the volunteer.</returns>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the volunteer does not exist.</exception>
    public BO.Volunteer Read(int id)
    {
        // Try to read the volunteer from the database
        DO.Volunteer? doVolunteer;
        lock (AdminManager.BlMutex)
             doVolunteer = _dal.Volunteer.Read(id) ??
        throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does Not exist");

        // Map the data object (DO) to the business object (BO)
        return new BO.Volunteer()
        {
            Id = id,
            Name = doVolunteer.Name,
            PhoneNumber = doVolunteer.PhoneNumber,
            Email = doVolunteer.Email,
            Password = VolunteerManager.DecryptPassword(doVolunteer.Password), // Decrypt password for the volunteer
            Address = doVolunteer.Address,
            Latitude = doVolunteer.Latitude,
            Longitude = doVolunteer.Longitude,
            Role = (BO.RoleType)doVolunteer.Role,
            Active = doVolunteer.Active,
            MaxDistance = doVolunteer.MaxDistance,
            TheDistanceType = (BO.DistanceType)doVolunteer.TheDistanceType,
            TotalHandled = VolunteerManager.TotalCall(id, DO.EndType.treated), // Get total handled calls
            TotalCanceled = VolunteerManager.TotalCall(id, DO.EndType.self), // Get total canceled calls
            TotalExpired = VolunteerManager.TotalCall(id, DO.EndType.expired), // Get total expired calls
            IsProgress = VolunteerManager.callProgress(id) // Check if the volunteer has any ongoing call
        };
    }

    /// <summary>
    /// Retrieves a list of all volunteers, with optional filtering and sorting.
    /// </summary>
    /// <param name="active">Filter for active volunteers. Null returns all volunteers.</param>
    /// <param name="field">The field to sort the list by.</param>
    /// <returns>A list of volunteers, sorted and filtered based on the provided criteria.</returns>
    public IEnumerable<BO.VolunteerInList> ReadAll(bool? active, BO.FieldsVolunteerInList field = BO.FieldsVolunteerInList.Id, BO.CallType? callType = null)
    {
        // Retrieve all volunteers from the database, filtering by active status if needed
        IEnumerable<DO.Volunteer> listVol;
        lock (AdminManager.BlMutex)
            listVol = active != null
            ? _dal.Volunteer.ReadAll(s => s.Active == active)
            : _dal.Volunteer.ReadAll();

        // Filter the list by call type, if a call type is specified
        if (callType != null && callType != BO.CallType.None)
        {
            listVol = listVol.Where(item =>
            {
                var currentCall = VolunteerManager.GetCurrentCall(item.Id);
                return currentCall != null && VolunteerManager.GetCallType((int)currentCall) == callType;
            });
        }

        // Sorting the list based on the specified field
        var sortedList = field switch
        {
            BO.FieldsVolunteerInList.Id => listVol.OrderBy(item => item.Id),
            BO.FieldsVolunteerInList.Name => listVol.OrderBy(item => item.Name),
            BO.FieldsVolunteerInList.TotalHandled => listVol.OrderBy(item => VolunteerManager.TotalCall(item.Id, DO.EndType.treated)),
            BO.FieldsVolunteerInList.TotalCanceled => listVol.OrderBy(item => VolunteerManager.TotalCall(item.Id, DO.EndType.self)),
            BO.FieldsVolunteerInList.TotalExpired => listVol.OrderBy(item => VolunteerManager.TotalCall(item.Id, DO.EndType.expired)),
            _ => listVol.OrderBy(item => item.Id) // Default sorting by ID
        };

        // Map each volunteer to a VolunteerInList object
        var listSort = from item in sortedList
                       let call = VolunteerManager.GetCurrentCall(item.Id) // Get the current call for the volunteer
                       select new BO.VolunteerInList
                       {
                           Id = item.Id,
                           Name = item.Name,
                           Active = item.Active,
                           TotalHandled = VolunteerManager.TotalCall(item.Id, DO.EndType.treated), // Calculate total handled calls
                           TotalCanceled = VolunteerManager.TotalCall(item.Id, DO.EndType.self), // Calculate total canceled calls
                           TotalExpired = VolunteerManager.TotalCall(item.Id, DO.EndType.expired), // Calculate total expired calls
                           CurrentCall = call, // Set the current call for the volunteer
                           TheCallType = call == null ? BO.CallType.None : VolunteerManager.GetCallType((int)call) // Get the call type, if there is an ongoing call
                       };

        return listSort; // Return the sorted list of volunteers
    }


    /// <summary>
    /// Updates the details of a volunteer.
    /// </summary>
    /// <param name="id">The ID of the volunteer to be updated.</param>
    /// <param name="volunteer">The business object containing the updated volunteer details.</param>
    /// <exception cref="BO.BlUserCantUpdateItemExeption">Thrown if the user does not have permission to update this volunteer.</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the volunteer does not exist.</exception>
    public void Update(int id, BO.Volunteer volunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        BO.Volunteer asker = Read(id); // Get the volunteer that is trying to update
        if (asker.Id != id && asker.Role != BO.RoleType.manager)
            throw new BO.BlUserCantUpdateItemExeption("The asker can't update this Volunteer");

        if (volunteer.Active == false && volunteer.IsProgress != null)
            throw new BO.BlUserCantUpdateItemExeption("This volunteer cant become not active becouse he has call in progress");
        BO.Volunteer oldVolunteer = Read(volunteer.Id); // Get the current volunteer data

        // Validate the updated data
        VolunteerManager.IsValidEmail(volunteer.Email); // Check if the email is valid
        VolunteerManager.IsValidID(volunteer.Id); // Check if the ID is valid
        VolunteerManager.IsValidPhoneNumber(volunteer.PhoneNumber); // Check if the phone number is valid

        double[] cordinate;
        if (oldVolunteer.Address != volunteer.Address|| oldVolunteer.Latitude==null|| oldVolunteer.Longitude==null)
            cordinate = VolunteerManager.GetCoordinates(volunteer.Address); // Get coordinates from the address
        else

            cordinate = [(double)oldVolunteer.Latitude,(double)oldVolunteer.Longitude];

        string password = volunteer.Password;
        if (password != oldVolunteer.Password)
        {
            VolunteerManager.ValidateStrongPassword(password); // Validate if the password is strong
        }
        password = VolunteerManager.EncryptPassword(password); // Encrypt the password

        // Prevent updating restricted fields
        if (asker.Role != BO.RoleType.manager && volunteer.Role != BO.RoleType.volunteer)
            throw new BO.BlUserCantUpdateItemExeption("Volunteer Can't change the role of the volunteer");

        if (volunteer.TotalCanceled != oldVolunteer.TotalCanceled ||
            volunteer.TotalExpired != oldVolunteer.TotalExpired ||
            volunteer.TotalHandled != oldVolunteer.TotalHandled)
            throw new BO.BlUserCantUpdateItemExeption("Total calls can't be modified!");

        try
        {
            // Create the updated data object (DO) for the volunteer
            DO.Volunteer doVolunteer = new(volunteer.Id, volunteer.Name,
                volunteer.PhoneNumber, volunteer.Email, password, volunteer.Address, cordinate[0],
                cordinate[1], (DO.RoleType)volunteer.Role, volunteer.Active, volunteer.MaxDistance,
                (DO.DistanceType)volunteer.TheDistanceType);

            // Update the volunteer in the database
            lock (AdminManager.BlMutex)
                _dal.Volunteer.Update(doVolunteer);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            // Handle case when the volunteer does not exist
            throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteer.Id} does not exist", ex);
        }
        VolunteerManager.Observers.NotifyItemUpdated(volunteer.Id);//update current volunteer and obserervers etc.
        VolunteerManager.Observers.NotifyListUpdated();//update list of volunteers and obserervers etc.
    }
}
