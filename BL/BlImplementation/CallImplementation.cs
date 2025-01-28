namespace BlImplementation;
using BlApi;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
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
        CallManager.CancelTreatment(RequesterId, AssignmentId);
    }

    /// <summary>
    /// Assigns a volunteer to treat a specific call.
    /// </summary>
    /// <param name="volunteerId">ID of the volunteer being assigned to the call.</param>
    /// <param name="CallId">ID of the call to be treated.</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if no matching open call is found.</exception>
    public void ChooseCallToTreat(int volunteerId, int CallId)
    {
        CallManager.ChooseCallToTreat(volunteerId, CallId);
    }


    /// <summary>
    /// Creates a new call in the database.
    /// </summary>
    /// <param name="call">The call object to be created.</param>
    /// <exception cref="BO.BlAlreadyExistsException">Thrown if the call already exists in the database.</exception>
    /// 
    public void Create(BO.Call call)
    {
        CallManager.Create(call);
    }

    /// <summary>
    /// Deletes an existing call from the database.
    /// </summary>
    /// <param name="id">The ID of the call to be deleted.</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the call does not exist.</exception>
    /// <exception cref="BO.BlcantDeleteItem">Thrown if the call cannot be deleted (due to assignments).</exception>
    public void Delete(int id)
    {
       CallManager.Delete(id);
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
        CallManager.EndTreatment(volunteerId, AssignmentId);   
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
        IEnumerable<DO.Call> allCalls;
        lock (AdminManager.BlMutex)
            allCalls = _dal.Call.ReadAll();

        IEnumerable<DO.Assignment> allAssignments;
        lock (AdminManager.BlMutex)
            allAssignments = _dal.Assignment.ReadAll();

        var filteredCalls = from call in allCalls
                            join assignment in allAssignments
                            on call.Id equals assignment.CallId
                            where assignment.VolunteerId == volunteerId &&
                                 (assignment.TheEndType != null ||
                                  assignment.TheEndType == DO.EndType.expired ||
                                  call.MaxTimeToEnd < AdminManager.Now)  // מוסיף גם קריאות שפג תוקפן
                            select new BO.ClosedCallInList
                            {
                                Id = call.Id,
                                TheCallType = (BO.CallType)call.TheCallType,
                                Address = call.Address,
                                OpeningTime = call.OpeningTime,
                                EntryTime = assignment.EntryTime,
                                ActualEndTime = assignment.ActualEndTime ?? AdminManager.Now,  // אם אין זמן סיום, משתמש בזמן הנוכחי
                                TheEndType = assignment.TheEndType == null && call.MaxTimeToEnd < AdminManager.Now ?
                                           BO.EndType.expired :
                                           (BO.EndType)assignment.TheEndType
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
        return CallManager.GetOpenCallInList(volunteerId, callTypeFilter, sortField);
    }

    /// <summary>
    /// Reads a specific call by its ID.
    /// </summary>
    /// <param name="id">The ID of the call to be read.</param>
    /// <returns>The call object corresponding to the provided ID.</returns>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the call does not exist.</exception>
    public BO.Call Read(int id)
    {
       return CallManager.Read(id);
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
        return CallManager.ReadAll(filter, toFilter, toSort);
    }


    /// <summary>
    /// Updates an existing call in the database.
    /// </summary>
    /// <param name="call">The call object with updated information.</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the call does not exist.</exception>
    public void Update(BO.Call call)
    {
        CallManager.Update(call);
    }

}

