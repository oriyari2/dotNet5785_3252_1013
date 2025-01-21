namespace BlImplementation;
using BlApi;
using Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
/*
        //CallManager
        //VolunteerManager 
*/
internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// Adds an observer that monitors updates for the entire list.
    /// </summary>
    /// <param name="listObserver">The callback action to be invoked on list updates.</param>
    public void AddObserver(Action listObserver) =>
        // Adds the provided observer for monitoring updates to the entire list.
        CallManager.Observers.AddListObserver(listObserver);

    /// <summary>
    /// Adds an observer that monitors updates for a specific object.
    /// </summary>
    /// <param name="id">The unique identifier of the object to observe.</param>
    /// <param name="observer">The callback action to be invoked on object updates.</param>
    public void AddObserver(int id, Action observer) =>
        // Adds the provided observer for monitoring updates to the object identified by the specified ID.
        CallManager.Observers.AddObserver(id, observer);

    /// <summary>
    /// Removes an observer that was monitoring updates for the entire list.
    /// </summary>
    /// <param name="listObserver">The callback action that was observing list updates.</param>
    public void RemoveObserver(Action listObserver) =>
        // Removes the provided observer from monitoring updates to the entire list.
        CallManager.Observers.RemoveListObserver(listObserver);

    /// <summary>
    /// Removes an observer that was monitoring updates for a specific object.
    /// </summary>
    /// <param name="id">The unique identifier of the object being observed.</param>
    /// <param name="observer">The callback action that was observing the object updates.</param>
    public void RemoveObserver(int id, Action observer) =>
        // Removes the provided observer from monitoring updates to the object identified by the specified ID.
        CallManager.Observers.RemoveObserver(id, observer);


    /// <summary>
    /// Retrieves the amount of calls grouped by their status.
    /// </summary>
    /// <returns>Enumerable of integer counts representing the number of calls per status.</returns>
    public IEnumerable<int> CallsAmount()
    {
        // Retrieve all the calls using the ReadAll method (null parameters mean no filter applied).
        var allCalls = ReadAll(null, null, null);

        // Group the calls by their status (casting status enum to int), and count occurrences of each group.
        var grouped = allCalls
            .GroupBy(call => (int)call.status)  // Group by status, using enum value as integer
            .ToDictionary(group => group.Key, group => group.Count());  // Convert the grouping to a dictionary with status as key and count as value

        // Create an array with a length equal to the number of status values in the Enum, and fill it with the count of each status.
        return Enumerable.Range(0, Enum.GetValues(typeof(BO.Status)).Length)  // Generate a range of numbers based on the Enum values length
                         .Select(index => grouped.ContainsKey(index) ? grouped[index] : 0)  // Select count for each index or 0 if not found
                         .ToArray();  // Convert the result to an integer array
    }

    /// <summary>
    /// Cancels a treatment assignment.
    /// </summary>
    /// <param name="RequesterId">ID of the volunteer requesting the cancellation.</param>
    /// <param name="AssignmentId">ID of the assignment to be canceled.</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the volunteer or assignment does not exist.</exception>
    /// <exception cref="BO.BlUserCantUpdateItemExeption">Thrown if the volunteer is not authorized to cancel the assignment.</exception>
    public void CancelTreatment(int RequesterId, int AssignmentId)
    {

        // Retrieve the assignment object based on its ID.
        var assignment = _dal.Assignment.Read(AssignmentId);
        // Check if the assignment does not exist.
        if (assignment == null)
            throw new BO.BlDoesNotExistException($"Assignment with id={AssignmentId} does Not exist\"");

        BO.Call call = Read(assignment.CallId);
        // Retrieve the volunteer (asker) object based on the RequesterId.
        var asker = _dal.Volunteer.Read(RequesterId);

        // Check if the volunteer does not exist.
        if (asker == null)
            throw new BO.BlDoesNotExistException($"Volunteer with id={RequesterId} does Not exist\"");
        
        // Check if the volunteer is not authorized to cancel the assignment (either not their own or not a manager).
        if (assignment.VolunteerId != RequesterId && asker.Role != DO.RoleType.manager)
            throw new BO.BlUserCantUpdateItemExeption($"Volunteer with id={RequesterId} can't change this assignment to cancel");

        //// Check if the assignment has already ended.
        //if (assignment.TheEndType != null || assignment.ActualEndTime != null)
        //    throw new BO.BlUserCantUpdateItemExeption("This assignment already ended");

        if (call.status != BO.Status.treatment && call.status != BO.Status.riskTreatment)
            throw new BO.BlUserCantUpdateItemExeption($"You can only unassign if the call is currently in progress.");

        // Create a new assignment object with updated end time and end type based on role.
        DO.Assignment newAssign;
        if (asker.Role == DO.RoleType.manager&& assignment.VolunteerId != RequesterId)
            newAssign = assignment with { ActualEndTime = AdminManager.Now, TheEndType = DO.EndType.manager };
        else
            newAssign = assignment with { ActualEndTime = AdminManager.Now, TheEndType = DO.EndType.self };
        try
        {
            // Update the assignment in the data layer.
            _dal.Assignment.Update(newAssign);
           
        }
        catch (DO.DalDoesNotExistException ex)
        {
            // If updating fails, throw an exception indicating the assignment does not exist.
            throw new BO.BlDoesNotExistException($"Assignment with ID={AssignmentId} does not exist", ex);
        }
        CallManager.Observers.NotifyItemUpdated(call.Id);  //update current call  and obserervers etc.
        CallManager.Observers.NotifyListUpdated();  //update list of calls  and obserervers etc.
        VolunteerManager.Observers.NotifyItemUpdated(assignment.VolunteerId);  //update current call  and obserervers etc.
        VolunteerManager.Observers.NotifyListUpdated();  //update list of calls  and obserervers etc.
    }

    /// <summary>
    /// Assigns a volunteer to treat a specific call.
    /// </summary>
    /// <param name="volunteerId">ID of the volunteer being assigned to the call.</param>
    /// <param name="CallId">ID of the call to be treated.</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if no matching open call is found.</exception>
    public void ChooseCallToTreat(int volunteerId, int CallId)
    {
        var currentCall = VolunteerManager.GetCurrentCall(volunteerId);
        if (currentCall != null)
            throw new BO.BlUserCantUpdateItemExeption("Volunteer cant choose new call to treat" +
                " because he already has one");

        // Retrieve the first open call that matches the given volunteerId and CallId from the list of open calls.
        var call = GetOpenCallInList(volunteerId, null, null).OrderByDescending(s=>s.OpeningTime).FirstOrDefault(s => s.Id == CallId);

        // Check if the call doesn't exist or does not meet the conditions.
        if (call == null)
            throw new BO.BlDoesNotExistException($"Call with needed conditions does not exist");

        // Create a new assignment object for the volunteer and the call.
        DO.Assignment assignment = new()
        {
            Id = 0,  // New assignment, so Id is set to 0
            CallId = CallId,  // Set the CallId
            VolunteerId = volunteerId,  // Set the VolunteerId
            EntryTime = AdminManager.Now,  // Set the entry time to the current time
            ActualEndTime = null,  // End time is null at the start
            TheEndType = null  // End type is null at the start
        };

        // Create the assignment in the data layer.
        _dal.Assignment.Create(assignment);
        
        CallManager.Observers.NotifyItemUpdated(CallId);  //update current call  and obserervers etc.
        CallManager.Observers.NotifyListUpdated();  //update list of calls  and obserervers etc.
        VolunteerManager.Observers.NotifyItemUpdated(volunteerId);  //update current call  and obserervers etc.
        VolunteerManager.Observers.NotifyListUpdated();

    }


    /// <summary>
    /// Creates a new call in the database.
    /// </summary>
    /// <param name="call">The call object to be created.</param>
    /// <exception cref="BO.BlAlreadyExistsException">Thrown if the call already exists in the database.</exception>
    /// 
    public void Create(BO.Call call)
    {
        // Call a helper method to convert BO.Call to DO.Call
        DO.Call doCall = CallManager.HelpCreateUodate(call);
        try
        {
            // Attempt to create the new call in the database
            _dal.Call.Create(doCall);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            // If the call already exists, throw a BO exception
            throw new BO.BlAlreadyExistsException($"Call with ID={call.Id} already exists", ex);
        }
        CallManager.Observers.NotifyListUpdated();  //update list of calls  and obserervers etc.
    }

    /// <summary>
    /// Deletes an existing call from the database.
    /// </summary>
    /// <param name="id">The ID of the call to be deleted.</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the call does not exist.</exception>
    /// <exception cref="BO.BlcantDeleteItem">Thrown if the call cannot be deleted (due to assignments).</exception>
    public void Delete(int id)
    {
        // Retrieve the call object by ID
        BO.Call call = Read(id);

        // Check if the call doesn't exist
        if (call == null)
            throw new BO.BlDoesNotExistException($"Call with ID={id} does Not exist");

        // Check if there are any assignments associated with the call
        if (call.listAssignForCall == null)
            throw new BO.BlcantDeleteItem($"Call with ID={id} can't be deleted because it never had an assignment");
        if (call.listAssignForCall.Count != 0)
            throw new BO.BlcantDeleteItem($"Call with ID={id} can't be deleted because it has an open assignment");


        try
        {
            // Attempt to delete the call from the database
            _dal.Call.Delete(id);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            // If the call does not exist, throw a BO exception
            throw new BO.BlDoesNotExistException($"Call with ID={id} does Not exist", ex);
        }
        CallManager.Observers.NotifyListUpdated();  //update list of calls  and obserervers etc.
    }

    /// <summary>
    /// Ends the treatment for a specific assignment.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer ending the treatment.</param>
    /// <param name="AssignmentId">The ID of the assignment to be ended.</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the assignment does not exist.</exception>
    /// <exception cref="BO.BlUserCantUpdateItemExeption">Thrown if the volunteer is not authorized to end the assignment.</exception>
    public void EndTreatment(int volunteerId, int AssignmentId)
    {
        // Retrieve the assignment by ID
        var assignment = _dal.Assignment.Read(AssignmentId);

        if (assignment == null)
            throw new BO.BlDoesNotExistException($"Assignment with id={AssignmentId} does Not exist\"");

        BO.Call call = Read(assignment.CallId);
        // Check if the assignment does not exist
        
        // Check if the volunteer is not the one assigned to this assignment
        if (assignment.VolunteerId != volunteerId)
            throw new BO.BlUserCantUpdateItemExeption($"Volunteer with id={volunteerId} can't change this assignment to end");

        // Check if the assignment has already ended
        if (assignment.ActualEndTime != null)
            throw new BO.BlUserCantUpdateItemExeption("This assignment already ended");

        // Create a new assignment object with updated end time and end type
        DO.Assignment newAssign = assignment with { ActualEndTime = AdminManager.Now, TheEndType = DO.EndType.treated };
        
        try
        {
            // Attempt to update the assignment in the database
            _dal.Assignment.Update(newAssign);

        }
        catch (DO.DalDoesNotExistException ex)
        {
            // If the assignment does not exist, throw a BO exception
            throw new BO.BlDoesNotExistException($"Assignment with ID={AssignmentId} does not exist", ex);
        }

        CallManager.Observers.NotifyItemUpdated(assignment.CallId);  //update current call  and obserervers etc.
        CallManager.Observers.NotifyListUpdated();  //update list of calls  and obserervers etc.
        VolunteerManager.Observers.NotifyItemUpdated(volunteerId);  //update current call  and obserervers etc.
        VolunteerManager.Observers.NotifyListUpdated();
    }

    /// <summary>
    /// Retrieves a list of closed calls for a specific volunteer, with optional filters and sorting.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <param name="callTypeFilter">An optional filter for the call type.</param>
    /// <param name="sortField">An optional field to sort by.</param>
    /// <returns>A list of closed calls in the specified format.</returns>
    public IEnumerable<BO.ClosedCallInList> GetClosedCallInList(int volunteerId, BO.CallType? callTypeFilter, BO.FieldsClosedCallInList? sortField)
    {
        // Retrieve all calls from the DAL (Data Access Layer)
        var allCalls = _dal.Call.ReadAll();

        // Retrieve all assignments from the DAL
        var allAssignments = _dal.Assignment.ReadAll();

        // Filter the calls by volunteer ID and closed status (TheEndType != null)
        var filteredCalls = from call in allCalls
                            join assignment in allAssignments
                            on call.Id equals assignment.CallId
                            where assignment.VolunteerId == volunteerId && assignment.TheEndType != null
                            select new BO.ClosedCallInList
                            {
                                Id = call.Id,
                                TheCallType = (BO.CallType)call.TheCallType, // Converts the call type to BO enumeration
                                Address = call.Address,
                                OpeningTime = call.OpeningTime,
                                EntryTime = assignment.EntryTime,
                                ActualEndTime = assignment.ActualEndTime,
                                TheEndType = (BO.EndType)assignment.TheEndType // Convert EndType to BO enumeration
                            };

        // Apply the call type filter if provided
        if (callTypeFilter.HasValue)
        {
            filteredCalls = filteredCalls.Where(c => c.TheCallType == callTypeFilter.Value);
        }

        // Sort by the specified field or by default (Call ID)
        if (sortField.HasValue)
        {
            filteredCalls = sortField.Value switch
            {
                BO.FieldsClosedCallInList.Id => filteredCalls.OrderBy(c => c.Id),
                BO.FieldsClosedCallInList.TheCallType => filteredCalls.OrderBy(c => c.TheCallType),
                BO.FieldsClosedCallInList.Address => filteredCalls.OrderBy(c => c.Address),
                BO.FieldsClosedCallInList.OpeningTime => filteredCalls.OrderBy(c => c.OpeningTime),
                BO.FieldsClosedCallInList.EntryTime => filteredCalls.OrderBy(c => c.EntryTime),
                BO.FieldsClosedCallInList.ActualEndTime => filteredCalls.OrderBy(c => c.ActualEndTime),
                BO.FieldsClosedCallInList.TheEndType => filteredCalls.OrderBy(c => c.TheEndType),
                _ => filteredCalls.OrderBy(c => c.Id)
            };
        }
        else
        {
            filteredCalls = filteredCalls.OrderBy(c => c.Id); // Default sorting by ID if no specific field is provided
        }

        return filteredCalls;
    }

    /// <summary>
    /// Retrieves a list of open calls for a specific volunteer, with optional filters and sorting.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <param name="callTypeFilter">An optional filter for the call type.</param>
    /// <param name="sortField">An optional field to sort by.</param>
    /// <returns>A list of open calls in the specified format.</returns>
    public IEnumerable<BO.OpenCallInList> GetOpenCallInList(int volunteerId, BO.CallType? callTypeFilter, BO.FieldsOpenCallInList? sortField)
    {
        // Retrieve the volunteer from the DAL
        DO.Volunteer volunteer = _dal.Volunteer.Read(volunteerId);
        if (volunteer == null)
            throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exists");

        // Retrieve all calls from the DAL
        var allCalls = ReadAll(null, null, null);

        // Retrieve all assignments from the DAL
        var allAssignments = _dal.Assignment.ReadAll();

        // Calculate the volunteer's latitude and longitude
        double lonVol = (double)volunteer.Longitude;
        double latVol = (double)volunteer.Latitude;

        // Group assignments by CallId and take the most recent assignment for each call
        //var latestAssignments = allAssignments
        //    .GroupBy(a => a.CallId)
        //    .Select(g => g.OrderByDescending(a => a.Id).FirstOrDefault()); // Assuming AssignmentTime exists

        // Filter calls by "open" or "riskOpen" status
        var filteredCalls = from call in allCalls
                            let boCall = Read(call.CallId) // Fetch the full call details
                            //join assignment in latestAssignments on call.Id equals assignment?.CallId into callAssignments
                            //from assignment in callAssignments.DefaultIfEmpty() // Allows joining even if no assignment exists for a call
                            let tmpDistance= volunteer?.Address != null ?
                                VolunteerManager.CalculateDistance(latVol, lonVol, boCall.Latitude, boCall.Longitude):0
                                let MaxDis=volunteer.MaxDistance
                            where ((boCall.status == BO.Status.open || boCall.status == BO.Status.riskOpen)&& (tmpDistance<= MaxDis|| MaxDis==null))

                            select new BO.OpenCallInList
                            {
                                Id = call.CallId,
                                TheCallType = call.TheCallType,
                                Address = boCall.Address,
                                OpeningTime = call.OpeningTime,
                                VerbalDescription=boCall.VerbalDescription,
                                MaxTimeToEnd = AdminManager.Now + call.TimeToEnd,
                                Distance = tmpDistance // Calculate the distance between the volunteer and the call
                            };

        // Apply the call type filter if provided
        if (callTypeFilter.HasValue)
        {
            filteredCalls = filteredCalls.Where(c => c.TheCallType == callTypeFilter.Value);
        }

        // Sort by the specified field or by default (Call ID)
        if (sortField.HasValue)
        {
            filteredCalls = sortField.Value switch
            {
                BO.FieldsOpenCallInList.Id => filteredCalls.OrderBy(c => c.Id),
                BO.FieldsOpenCallInList.TheCallType => filteredCalls.OrderBy(c => c.TheCallType),
                BO.FieldsOpenCallInList.Address => filteredCalls.OrderBy(c => c.Address),
                BO.FieldsOpenCallInList.OpeningTime => filteredCalls.OrderBy(c => c.OpeningTime),
                BO.FieldsOpenCallInList.Distance => filteredCalls.OrderBy(c => c.Distance),
                _ => filteredCalls.OrderBy(c => c.Id)
            };
        }
        else
        {
            filteredCalls = filteredCalls.OrderBy(c => c.Id); // Default sorting by ID if no specific field is provided
        }

        return filteredCalls;
    }

    /// <summary>
    /// Reads a specific call by its ID.
    /// </summary>
    /// <param name="id">The ID of the call to be read.</param>
    /// <returns>The call object corresponding to the provided ID.</returns>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the call does not exist.</exception>
    public BO.Call Read(int id)
    {
        // Retrieve the call from the DAL
        var call = _dal.Call.Read(id) ??
        throw new BO.BlDoesNotExistException($"Call with ID={id} does Not exist");

        // Retrieve the assignments for the call
        var assignments = _dal.Assignment.ReadAll(s => s.CallId == id)
                                        .OrderByDescending(s => s.Id);
        var latestAssignment = assignments.FirstOrDefault();

        return new BO.Call()
        {
            Id = call.Id,
            TheCallType = (BO.CallType)call.TheCallType,
            VerbalDescription = call.VerbalDescription,
            Address = call.Address,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            OpeningTime = call.OpeningTime,
            MaxTimeToEnd = call.MaxTimeToEnd,
            status = CallManager.CheckStatus(latestAssignment, call),
            listAssignForCall = assignments == null ? null :
                           CallManager.GetCallAssignInList(assignments)
        };
    }

    /// <summary>
    /// Reads all calls, with optional filters and sorting.
    /// </summary>
    /// <param name="filter">An optional filter field to apply to the calls.</param>
    /// <param name="toFilter">The value to filter the calls by.</param>
    /// <param name="toSort">The field to sort the calls by.</param>
    /// <returns>A list of calls, filtered and sorted as requested.</returns>
    public IEnumerable<BO.CallInList> ReadAll(BO.FieldsCallInList? filter, object? toFilter, BO.FieldsCallInList? toSort)
    {
        // Retrieve all calls from the DAL
        var listCall = _dal.Call.ReadAll();

        // Retrieve all assignments from the DAL
        var listAssignment = _dal.Assignment.ReadAll();
        // Join calls and assignments to create the list of calls in the desired format
        var callInList = from item in listCall
                         let assignment = listAssignment.Where(s => s.CallId == item.Id).OrderByDescending(s => s.Id).FirstOrDefault()
                         let volunteer = assignment != null ? _dal.Volunteer.Read(assignment.VolunteerId) : null
                         let TempTimeToEnd = item.MaxTimeToEnd - (AdminManager.Now)
                         let tmpstatus = CallManager.CheckStatus(assignment, item)
                         select new BO.CallInList
                         {
                             Id = assignment != null ? assignment.Id : null,
                             CallId = item.Id,
                             TheCallType = (BO.CallType)item.TheCallType,
                             OpeningTime = item.OpeningTime,
                             TimeToEnd = TempTimeToEnd > TimeSpan.Zero ? TempTimeToEnd : null,
                             LastVolunteer = volunteer != null ? volunteer.Name : null,
                             status = tmpstatus,
                             CompletionTreatment = tmpstatus == BO.Status.close ? assignment.ActualEndTime - item.OpeningTime : null,
                             TotalAssignments = listAssignment.Where(s => s.CallId == item.Id).Count()
                         };

        // Apply filter if specified
        if (filter.HasValue)
        {
            callInList = callInList.Where(call =>
            {
                return filter switch
                {
                    BO.FieldsCallInList.Id => call.Id.Equals(toFilter),
                    BO.FieldsCallInList.CallId => call.CallId.Equals(toFilter),
                    BO.FieldsCallInList.TheCallType => call.TheCallType.Equals(toFilter),
                    BO.FieldsCallInList.OpeningTime => call.OpeningTime.Equals(toFilter),
                    BO.FieldsCallInList.TimeToEnd => call.TimeToEnd.Equals(toFilter),
                    BO.FieldsCallInList.LastVolunteer => call.LastVolunteer != null && call.LastVolunteer.Equals(toFilter),
                    BO.FieldsCallInList.CompletionTreatment => call.CompletionTreatment.Equals(toFilter),
                    BO.FieldsCallInList.status => call.status.Equals(toFilter),
                    BO.FieldsCallInList.TotalAssignments => call.TotalAssignments.Equals(toFilter),
                    _ => true
                };
            });
        }

        // Sort the results if specified
        if (toSort.HasValue)
        {
            callInList = toSort switch
            {
                BO.FieldsCallInList.Id => callInList.OrderBy(call => call.Id),
                BO.FieldsCallInList.CallId => callInList.OrderBy(call => call.CallId),
                BO.FieldsCallInList.TheCallType => callInList.OrderBy(call => call.TheCallType),
                BO.FieldsCallInList.OpeningTime => callInList.OrderBy(call => call.OpeningTime),
                BO.FieldsCallInList.TimeToEnd => callInList.OrderBy(call => call.TimeToEnd),
                BO.FieldsCallInList.LastVolunteer => callInList.OrderBy(call => call.LastVolunteer),
                BO.FieldsCallInList.CompletionTreatment => callInList.OrderBy(call => call.CompletionTreatment),
                BO.FieldsCallInList.status => callInList.OrderBy(call => call.status),
                BO.FieldsCallInList.TotalAssignments => callInList.OrderBy(call => call.TotalAssignments),
                _ => callInList
            };
        }
        else
        {
            callInList = callInList.OrderBy(call => call.CallId); // Default sorting by CallId
        }

        return callInList;
    }


    /// <summary>
    /// Updates an existing call in the database.
    /// </summary>
    /// <param name="call">The call object with updated information.</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the call does not exist.</exception>
    public void Update(BO.Call call)
    {
        var oldCall = _dal.Call.Read(call.Id);
        // Convert BO.Call to DO.Call
        DO.Call doCall = CallManager.HelpCreateUodate(call);
        if (call.status == BO.Status.close || call.status == BO.Status.expired)
            throw new BO.BlUserCantUpdateItemExeption("This call is closed");
        if (call.status == BO.Status.treatment || call.status == BO.Status.riskTreatment)
        {
            if (doCall.Address != oldCall.Address || doCall.TheCallType != oldCall.TheCallType ||
                doCall.VerbalDescription != oldCall.VerbalDescription)
                throw new BO.BlUserCantUpdateItemExeption("These details cannot be changed because the call is already in progress.");
        }

        try
        {
            // Attempt to update the call in the DAL
            _dal.Call.Update(doCall);
        }
        catch
        {
            // Throw an exception if the update fails
            throw new BO.BlDoesNotExistException($"Call with ID={call.Id} does Not exist");
        }
        CallManager.Observers.NotifyItemUpdated(call.Id);  //update current call  and obserervers etc.
        CallManager.Observers.NotifyListUpdated();  //update list of calls  and obserervers etc.
    }

}

