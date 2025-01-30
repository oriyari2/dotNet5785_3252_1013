using BlImplementation;
using DalApi;
using System.Net;
using System.Xml.Linq;

namespace Helpers;  // Declares the Helpers namespace, containing utility classes for call management.

/// <summary>
/// CallManager class that manages call assignments and statuses.
/// </summary>
internal static class CallManager
{
    internal static ObserverManager Observers = new(); //stage 5 
    // Static DAL instance used for data access operations.
    private static IDal s_dal = Factory.Get;

    /// <summary>
    /// Checks the status of a call based on its assignment and the current time.
    /// </summary>
    internal static BO.Status CheckStatus(DO.Assignment? assignment, DO.Call call)
    {
        AdminImplementation admin = new();

        // First check if the call was successfully treated
        if (assignment?.TheEndType == DO.EndType.treated)
            return BO.Status.close;

        // Then check if the assignment is marked as expired
        if (assignment?.TheEndType == DO.EndType.expired)
            return BO.Status.expired;

        // Then check for time expiration
        if (call.MaxTimeToEnd <= admin.GetClock())
            return BO.Status.expired;

        // Then handle open and risk states
        if (assignment == null || assignment.TheEndType == DO.EndType.self || assignment.TheEndType == DO.EndType.manager)
        {
            if ((call.MaxTimeToEnd - admin.GetClock()) <= admin.GetRiskRange())
                return BO.Status.riskOpen;
            else
                return BO.Status.open;
        }

        // Finally handle treatment states
        if ((call.MaxTimeToEnd - admin.GetClock()) <= admin.GetRiskRange())
            return BO.Status.riskTreatment;
        else
            return BO.Status.treatment;
    }    /// <summary>
         /// Gets a list of calls with their assignment information.
         /// </summary>
    internal static List<BO.CallAssignInList> GetCallAssignInList(IEnumerable<DO.Assignment> assignment)
    {
        IEnumerable<BO.CallAssignInList> toReturn;
        // Projects the assignments to a list of CallAssignInList objects.
        lock (AdminManager.BlMutex)
            toReturn = from item in assignment
                       let volunteerr = (s_dal.Volunteer.Read(item.VolunteerId))  // Retrieves volunteer details.
                       select new BO.CallAssignInList()
                       {
                           VolunteerId = item.VolunteerId,  // Sets volunteer ID.
                           Name = volunteerr == null ? null : volunteerr.Name,  // Sets volunteer name if available.
                           EntryTime = item.EntryTime,  // Sets the assignment entry time.
                           ActualEndTime = item.ActualEndTime,  // Sets the actual end time of the assignment.
                           TheEndType = item.TheEndType == null ? null : (BO.EndType)item.TheEndType  // Converts EndType if available.
                       };

        return toReturn.ToList();  // Returns the list of assignments as CallAssignInList objects.
    }

    /// <summary>
    /// Creates or updates a call based on the provided BO.Call object.
    /// </summary>
    internal static DO.Call HelpCreateUpdate(BO.Call call, double[]? coordinates = null)
    {
        // Retrieves the coordinates based on the address. Throws an exception if the address is invalid.
        AdminImplementation admin = new();  // Creates an instance of AdminImplementation to access admin settings.

        // Checks if the MaxTimeToEnd is smaller than the OpeningTime, throws exception if true.
        if (call.MaxTimeToEnd < admin.GetClock() + admin.GetRiskRange())
            throw new BO.BlUserCantUpdateItemExeption("Max Time To End of Call can't be smaller than the Opening Time + risk range");

        if (call.MaxTimeToEnd < admin.GetClock())
            throw new BO.BlUserCantUpdateItemExeption("Max Time To End of Call can't be smaller than the Opening Time");

        // Returns a new DO.Call object with the updated values.
        DO.Call newCall = new()
        {
            Id = call.Id,
            TheCallType = (DO.CallType)call.TheCallType,  // Converts the call type to DO.CallType.
            VerbalDescription = call.VerbalDescription,  // Sets the verbal description.
            Address = call.Address,  // Sets the address.
            Latitude = coordinates != null ? coordinates[0] : null,  // Uses existing coordinates if available.
            Longitude = coordinates != null ? coordinates[1] : null,  // Uses existing coordinates if available.
            OpeningTime = call.OpeningTime,  // Sets the opening time.
            MaxTimeToEnd = call.MaxTimeToEnd  // Sets the max time to end.
        };
        return newCall;
    }

    /// <summary>
    /// Updates expired calls by checking the current assignments and their time status.
    /// </summary>
    internal static void UpdateExpired()
    {
        IEnumerable<DO.Call> expiredCalls;

        lock (AdminManager.BlMutex)
        {
            expiredCalls = s_dal.Call.ReadAll(c =>
                c.MaxTimeToEnd < AdminManager.Now
            );
        }

        foreach (var call in expiredCalls)
        {
            bool hasAssignment;
            lock (AdminManager.BlMutex)
                hasAssignment = s_dal.Assignment.ReadAll(a => a.CallId == call.Id).Any();

            if (!hasAssignment)
            {
                var newAssignment = new DO.Assignment(
                    Id: 0,
                    CallId: call.Id,
                    VolunteerId: 0,
                    EntryTime: AdminManager.Now,
                    ActualEndTime: AdminManager.Now,
                    TheEndType: DO.EndType.expired
                );

                lock (AdminManager.BlMutex)
                    s_dal.Assignment.Create(newAssignment);

                Console.WriteLine($"[UpdateExpired] Created expired assignment for Call ID={call.Id}");
            }
        }

        IEnumerable<DO.Assignment> listAssi;
        lock (AdminManager.BlMutex)
            listAssi = s_dal.Assignment.ReadAll(a => a.TheEndType != DO.EndType.treated);

        foreach (var assignment in listAssi)
        {
            var call = expiredCalls.FirstOrDefault(c => c.Id == assignment.CallId);
            if (call != null)
            {
                var updatedAssignment = assignment with
                {
                    ActualEndTime = AdminManager.Now,
                    TheEndType = DO.EndType.expired
                };

                lock (AdminManager.BlMutex)
                    s_dal.Assignment.Update(updatedAssignment);
            }
        }
    }
    #region MoveFromImplemenation
    internal static void CancelTreatment(int RequesterId, int AssignmentId, bool isSimulation = false)
    {
        if(isSimulation ==  false) 
            AdminManager.ThrowOnSimulatorIsRunning();
        // Retrieve the assignment object based on its ID.
        DO.Assignment? assignment;
        lock (AdminManager.BlMutex)
            assignment = s_dal.Assignment.Read(AssignmentId);
        // Check if the assignment does not exist.
        if (assignment == null)
            throw new BO.BlDoesNotExistException($"Assignment with id={AssignmentId} does Not exist\"");

        BO.Call call = Read(assignment.CallId);
        // Retrieve the volunteer (asker) object based on the RequesterId.
        DO.Volunteer? asker;
        lock (AdminManager.BlMutex)
            asker = s_dal.Volunteer.Read(RequesterId);

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
        if (asker.Role == DO.RoleType.manager && assignment.VolunteerId != RequesterId)
            newAssign = assignment with { ActualEndTime = AdminManager.Now, TheEndType = DO.EndType.manager };
        else
            newAssign = assignment with { ActualEndTime = AdminManager.Now, TheEndType = DO.EndType.self };
        try
        {
            // Update the assignment in the data layer.
            lock (AdminManager.BlMutex)
                s_dal.Assignment.Update(newAssign);

        }
        catch (DO.DalDoesNotExistException ex)
        {
            // If updating fails, throw an exception indicating the assignment does not exist.
            throw new BO.BlDoesNotExistException($"Assignment with ID={AssignmentId} does not exist", ex);
        }
        Observers.NotifyItemUpdated(call.Id);  //update current call  and obserervers etc.
        Observers.NotifyListUpdated();  //update list of calls  and obserervers etc.
        VolunteerManager.Observers.NotifyItemUpdated(assignment.VolunteerId);  //update current call  and obserervers etc.
        VolunteerManager.Observers.NotifyListUpdated();  //update list of calls  and obserervers etc.
    }

    internal static void ChooseCallToTreat(int volunteerId, int CallId, bool isSimulation = false)
    {
        if(isSimulation == false) 
            AdminManager.ThrowOnSimulatorIsRunning();
        var currentCall = VolunteerManager.GetCurrentCall(volunteerId);
        if (currentCall != null)
            throw new BO.BlUserCantUpdateItemExeption("Volunteer cant choose new call to treat" +
                " because he already has one");
        DO.Volunteer vol;
        lock (AdminManager.BlMutex)
            vol = s_dal.Volunteer.Read(volunteerId);
        if (vol.Active == false)
            throw new BO.BlUserCantUpdateItemExeption("Volunteer cant choose new call to treat" +
                " because he is not active");
        // Retrieve the first open call that matches the given volunteerId and CallId from the list of open calls.
        var call = GetOpenCallInList(volunteerId, null, null).OrderByDescending(s => s.OpeningTime).FirstOrDefault(s => s.Id == CallId);

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
        lock (AdminManager.BlMutex)
            s_dal.Assignment.Create(assignment);

        Observers.NotifyItemUpdated(CallId);  //update current call  and obserervers etc.
        Observers.NotifyListUpdated();  //update list of calls  and obserervers etc.
        VolunteerManager.Observers.NotifyItemUpdated(volunteerId);  //update current call  and obserervers etc.
        VolunteerManager.Observers.NotifyListUpdated();

    }

    internal static void Create(BO.Call call)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        // Call a helper method to convert BO.Call to DO.Call
        DO.Call doCall = HelpCreateUpdate(call);
        try
        {
            // Attempt to create the new call in the database
            lock (AdminManager.BlMutex)
                s_dal.Call.Create(doCall);
            lock (AdminManager.BlMutex)
                doCall = s_dal.Call.ReadAll().OrderByDescending(S=>S.Id).First();
             _ = VolunteerManager.GetCoordinates(doCall);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            // If the call already exists, throw a BO exception
            throw new BO.BlAlreadyExistsException($"Call with ID={call.Id} already exists", ex);
        }
        Observers.NotifyListUpdated();  //update list of calls  and obserervers etc.
    }

    internal static void Delete(int id)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
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
            lock (AdminManager.BlMutex)
                s_dal.Call.Delete(id);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            // If the call does not exist, throw a BO exception
            throw new BO.BlDoesNotExistException($"Call with ID={id} does Not exist", ex);
        }
        Observers.NotifyListUpdated();  //update list of calls  and obserervers etc.
    }

    internal static void EndTreatment(int volunteerId, int AssignmentId, bool isSimulation = false)
    {
        if(isSimulation == false)
            AdminManager.ThrowOnSimulatorIsRunning();
        // Retrieve the assignment by ID
        DO.Assignment? assignment;
        lock (AdminManager.BlMutex)
            assignment = s_dal.Assignment.Read(AssignmentId);

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
            lock (AdminManager.BlMutex)
                s_dal.Assignment.Update(newAssign);

        }
        catch (DO.DalDoesNotExistException ex)
        {
            // If the assignment does not exist, throw a BO exception
            throw new BO.BlDoesNotExistException($"Assignment with ID={AssignmentId} does not exist", ex);
        }

        Observers.NotifyItemUpdated(assignment.CallId);  //update current call  and obserervers etc.
        Observers.NotifyListUpdated();  //update list of calls  and obserervers etc.
        VolunteerManager.Observers.NotifyItemUpdated(volunteerId);  //update current call  and obserervers etc.
        VolunteerManager.Observers.NotifyListUpdated();
    }

    internal static IEnumerable<BO.OpenCallInList> GetOpenCallInList(int volunteerId, BO.CallType? callTypeFilter, BO.FieldsOpenCallInList? sortField)
    {
        // Retrieve the volunteer from the DAL
        DO.Volunteer volunteer;
        lock (AdminManager.BlMutex)
            volunteer = s_dal.Volunteer.Read(volunteerId);
        if (volunteer == null)
            throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exist");

        // Retrieve all calls from the DAL
        var allCalls = ReadAll(null, null, null);

        // Retrieve all assignments from the DAL
        IEnumerable<DO.Assignment> allAssignments;
        lock (AdminManager.BlMutex)
            allAssignments = s_dal.Assignment.ReadAll();

        // Get volunteer coordinates if available
        double? lonVol = volunteer.Longitude;
        double? latVol = volunteer.Latitude;

        // Filter calls by "open" or "riskOpen" status
        var filteredCalls = from call in allCalls
                            let boCall = Read(call.CallId) // Fetch the full call details
                            let lat = boCall.Latitude
                            let lon = boCall.Longitude
                            let tmpDistance = (volunteer?.Address != null && latVol != null && lonVol != null && lat != null && lon != null) ?
                                VolunteerManager.CalculateDistance((double)latVol, (double)lonVol, (double)lat, (double)lon) : 0
                            let MaxDis = volunteer.MaxDistance
                            where ((boCall.status == BO.Status.open || boCall.status == BO.Status.riskOpen) &&
                                   (tmpDistance <= MaxDis || MaxDis == null))
                            select new BO.OpenCallInList
                            {
                                Id = call.CallId,
                                TheCallType = call.TheCallType,
                                Address = boCall.Address,
                                OpeningTime = call.OpeningTime,
                                VerbalDescription = boCall.VerbalDescription,
                                MaxTimeToEnd = AdminManager.Now + call.TimeToEnd,
                                Distance = tmpDistance // אם אין חישוב מרחק, יהיה 0
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

    internal static BO.Call Read(int id)
    {
        // Retrieve the call from the DAL
        DO.Call call;
        lock (AdminManager.BlMutex)
            call = s_dal.Call.Read(id) ??
        throw new BO.BlDoesNotExistException($"Call with ID={id} does Not exist");

        // Retrieve the assignments for the call
        IOrderedEnumerable<DO.Assignment> assignments;
        lock (AdminManager.BlMutex)
            assignments = s_dal.Assignment.ReadAll(s => s.CallId == id)
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
            status = CheckStatus(latestAssignment, call),
            listAssignForCall = assignments == null ? null :
                           GetCallAssignInList(assignments)
        };
    }

    internal static IEnumerable<BO.CallInList> ReadAll(BO.FieldsCallInList? filter, object? toFilter, BO.FieldsCallInList? toSort)
    {
        IEnumerable<DO.Call> listCall;
        // Retrieve all calls from the DAL
        lock (AdminManager.BlMutex)
            listCall = s_dal.Call.ReadAll();

        // Retrieve all assignments from the DAL
        IEnumerable<DO.Assignment> listAssignment;
        lock (AdminManager.BlMutex)
            listAssignment = s_dal.Assignment.ReadAll();
        // Join calls and assignments to create the list of calls in the desired format

        IEnumerable<BO.CallInList>? callInList;
        lock (AdminManager.BlMutex)
            callInList = from item in listCall
                         let assignment = listAssignment.Where(s => s.CallId == item.Id).OrderByDescending(s => s.Id).FirstOrDefault()
                         let volunteer = assignment != null ? s_dal.Volunteer.Read(assignment.VolunteerId) : null
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

    internal static void Update(BO.Call call)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        DO.Call? oldCall;
        lock (AdminManager.BlMutex)
            oldCall = s_dal.Call.Read(call.Id);

        // Convert BO.Call to DO.Call
        if (call.status == BO.Status.close)
            throw new BO.BlUserCantUpdateItemExeption("This call is closed");
        if (call.status == BO.Status.expired)
            throw new BO.BlUserCantUpdateItemExeption("This call is expired");

        DO.Call doCall;

        // אם הכתובת השתנתה, יש לאתחל את הקורדינטות מחדש
        if (oldCall.Address != call.Address)
        {
            doCall = CallManager.HelpCreateUpdate(call);
        }
        else
        {
            // רק אם הקורדינטות אינן null, נשלח אותן
            double[]? coordinates = (call.Latitude != null && call.Longitude != null)
                ? new double[] { (double)call.Latitude, (double)call.Longitude }
                : null;

            doCall = CallManager.HelpCreateUpdate(call, coordinates);
        }

        // אם השיחה כבר בטיפול, לא ניתן לעדכן פרטים מסוימים
        if (call.status == BO.Status.treatment || call.status == BO.Status.riskTreatment)
        {
            if (doCall.Address != oldCall.Address || doCall.TheCallType != oldCall.TheCallType ||
                doCall.VerbalDescription != oldCall.VerbalDescription)
                throw new BO.BlUserCantUpdateItemExeption("These details cannot be changed because the call is already in progress.");
        }

        try
        {
            // Attempt to update the call in the DAL
            lock (AdminManager.BlMutex)
                s_dal.Call.Update(doCall);
            if (oldCall.Address != call.Address || call.Latitude == null || call.Longitude == null)
            {
                _ = VolunteerManager.GetCoordinates(doCall);
            }
        }
        catch
        {
            // Throw an exception if the update fails
            throw new BO.BlDoesNotExistException($"Call with ID={call.Id} does not exist");
        }

        Observers.NotifyItemUpdated(call.Id);  // Update current call and observers etc.
        Observers.NotifyListUpdated();  // Update list of calls and observers etc.
    }

    #endregion
}
