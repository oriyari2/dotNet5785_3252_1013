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
    // Static DAL instance used for data access operations.
    private static IDal s_dal = Factory.Get;

    /// <summary>
    /// Checks the status of a call based on its assignment and the current time.
    /// </summary>
    internal static BO.Status CheckStatus(DO.Assignment? assignment, DO.Call call)
    {
        AdminImplementation admin = new();  // Creates an instance of AdminImplementation to access admin settings.

        // Checks if the call has expired based on the MaxTimeToEnd and current time.
        if (call.MaxTimeToEnd < ClockManager.Now)
            return BO.Status.expired;  // Returns expired if the call time has passed.

        // If there is no assignment or the end type is self or manager (open/risknot)
        if (assignment == null || assignment.TheEndType == DO.EndType.self || assignment.TheEndType == DO.EndType.manager)
            if ((ClockManager.Now - call.MaxTimeToEnd) > admin.GetRiskRange())  // Checks if the call exceeds the risk range.
                return BO.Status.riskOpen;  // Returns riskOpen if the call exceeds the risk range.
            else
                return BO.Status.open;  // Returns open if the call is still within the range.

        // If the end type is null (treatment-related cases)
        if (assignment.TheEndType == null)
            if ((ClockManager.Now - call.MaxTimeToEnd) > admin.GetRiskRange())  // Checks if the call exceeds the risk range.
                return BO.Status.riskTreatment;  // Returns riskTreatment if the call exceeds the risk range.
            else
                return BO.Status.treatment;  // Returns treatment if within the range.

        // If the assignment end type is treated, the call is closed.
        else
            return BO.Status.close;  // Returns close if the treatment is completed.
    }

    /// <summary>
    /// Gets a list of calls with their assignment information.
    /// </summary>
    internal static List<BO.CallAssignInList> GetCallAssignInList(IEnumerable<DO.Assignment> assignment)
    {
        // Projects the assignments to a list of CallAssignInList objects.
        var toReturn = from item in assignment
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
    internal static DO.Call HelpCreateUodate(BO.Call call)
    {
        double[] cordinate = VolunteerManager.GetCoordinates(call.Address);  // Retrieves the coordinates based on the address. Throws an exception if the address is invalid.

        // Checks if the MaxTimeToEnd is smaller than the OpeningTime, throws exception if true.
        if (call.MaxTimeToEnd < call.OpeningTime)
            throw new BO.BlUserCantUpdateItemExeption("Max Time To End of Call can't be smaller than the Opening Time");

        // Returns a new DO.Call object with the updated values.
        return new()
        {
            Id = call.Id,
            TheCallType = (DO.CallType)call.TheCallType,  // Converts the call type to DO.CallType.
            VerbalDescription = call.VerbalDescription,  // Sets the verbal description.
            Address = call.Address,  // Sets the address.
            Latitude = cordinate[0],  // Sets the latitude.
            Longitude = cordinate[1],  // Sets the longitude.
            OpeningTime = call.OpeningTime,  // Sets the opening time.
            MaxTimeToEnd = call.MaxTimeToEnd  // Sets the max time to end.
        };
    }

    /// <summary>
    /// Updates expired calls by checking the current assignments and their time status.
    /// </summary>
    internal static void UpdateExpired()
    {
        // Step 1: Retrieves all calls where the MaxTimeToEnd has passed.
        var expiredCalls = s_dal.Call.ReadAll(c => c.MaxTimeToEnd < ClockManager.Now);

        // Step 2: Checks for calls without assignments and creates a new assignment with the expired status.
        foreach (var call in expiredCalls)
        {
            var hasAssignment = s_dal.Assignment
                .ReadAll(a => a.CallId == call.Id)
                .Any();

            if (!hasAssignment)  // If there is no assignment for the call yet
            {
                var newAssignment = new DO.Assignment(
                    Id: 0,  // Creates a new ID for the assignment.
                    CallId: call.Id,
                    VolunteerId: 0,  // Volunteer ID is set to 0 (no assignment).
                    EntryTime: ClockManager.Now,  // Sets the entry time to the current time.
                    ActualEndTime: ClockManager.Now,  // Sets the actual end time to the current time.
                    TheEndType: DO.EndType.expired  // Sets the end type to expired.
                );
                s_dal.Assignment.Create(newAssignment);  // Creates the new assignment.
            }
        }

        // Step 3: Updates assignments with null ActualEndTime for calls that are expired.
        foreach (var assignment in s_dal.Assignment.ReadAll(a => a.ActualEndTime == null))
        {
            var call = expiredCalls.FirstOrDefault(c => c.Id == assignment.CallId);
            if (call != null)  // If the call is still marked as expired
            {
                var updatedAssignment = assignment with
                {
                    ActualEndTime = ClockManager.Now,  // Sets the actual end time to the current time.
                    TheEndType = DO.EndType.expired  // Marks the end type as expired.
                };

                s_dal.Assignment.Update(updatedAssignment);  // Updates the assignment with the new values.
            }
        }
    }
}
