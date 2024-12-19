using BlImplementation;
using DalApi;
using System.Net;
using System.Xml.Linq;

namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get;
    internal static BO.Status CheckStatus(DO.Assignment? assignment, DO.Call call)
    {
        AdminImplementation admin = new();

        if (call.MaxTimeToEnd < ClockManager.Now) //time past either there is assignment to call or not
            return BO.Status.expired;
        if (assignment == null || assignment.TheEndType == DO.EndType.self || assignment.TheEndType == DO.EndType.manager) //(open/risknot)
            if ((ClockManager.Now - call.MaxTimeToEnd) > admin.GetRiskRange())
                return BO.Status.riskOpen;
            else
                return BO.Status.open;
        if (assignment.TheEndType == null)
            if ((ClockManager.Now - call.MaxTimeToEnd) > admin.GetRiskRange())
                return BO.Status.riskTreatment;
            else
                return BO.Status.treatment;
        else  //if(assignment.TheEndType ==DO.EndType.treated)
            return BO.Status.close;

    }
    internal static List<BO.CallAssignInList> GetCallAssignInList(IEnumerable<DO.Assignment> assignment)
    {
        var toReturn = from item in assignment
                       let volunteerr = (s_dal.Volunteer.Read(item.VolunteerId))
                       select new BO.CallAssignInList()
                       {
                           VolunteerId = item.VolunteerId,
                           Name = volunteerr == null ? null : volunteerr.Name,
                           EntryTime = item.EntryTime,
                           ActualEndTime = item.ActualEndTime,
                           TheEndType = item.TheEndType == null ? null :(BO.EndType)item.TheEndType
                       };
        return toReturn.ToList();
    }
    internal static DO.Call HelpCreateUodate(BO.Call call)
    {
        double[] cordinate = VolunteerManager.GetCoordinates(call.Address); //if wrrong adreess throw exeption

        if (call.MaxTimeToEnd < call.OpeningTime)
            throw new BO.BlUserCantUpdateItemExeption("Max Time To End of Call can't be smaller than the Opening Time");
        return new()
        {
            Id = call.Id,
            TheCallType = (DO.CallType)call.TheCallType,
            VerbalDescription = call.VerbalDescription,
            Address = call.Address,
            Latitude = cordinate[0],
            Longitude = cordinate[1],
            OpeningTime = call.OpeningTime,
            MaxTimeToEnd = call.MaxTimeToEnd
        };
    }
    internal static void UpdateExpired()
    {
        // שלב 1: שליפת קריאות שזמן הסיום המקסימלי שלהן עבר
        var expiredCalls = s_dal.Call.ReadAll(c => c.MaxTimeToEnd < ClockManager.Now);

        // שלב 2: קריאות ללא הקצאה - יצירת הקצאה חדשה עבורן
        foreach (var call in expiredCalls)
        {
            var hasAssignment = s_dal.Assignment
                .ReadAll(a => a.CallId == call.Id)
                .Any();

            if (!hasAssignment) // אם אין עדיין הקצאה לקריאה הזו
            {
                var newAssignment = new DO.Assignment(
                    Id: 0,  // יצירת ID חדש להקצאה
                    CallId: call.Id,
                    VolunteerId: 0, // ת.ז מתנדב = 0
                    EntryTime: ClockManager.Now,
                    ActualEndTime: ClockManager.Now,
                    TheEndType: DO.EndType.expired // סוג סיום "ביטול פג תוקף"
                );
                s_dal.Assignment.Create(newAssignment); 
            }
        }

        // שלב 3: קריאות עם הקצאה קיימת אך זמן סיום הטיפול שלהן הוא null - עדכון ההקצאה
        foreach (var assignment in s_dal.Assignment.ReadAll(a => a.ActualEndTime == null))
        {
            var call = expiredCalls.FirstOrDefault(c => c.Id == assignment.CallId);
            if (call != null) // אם הקריאה עדיין מוגדרת כפג תוקף
            {
                var updatedAssignment = assignment with
                {
                    ActualEndTime = ClockManager.Now,
                    TheEndType = DO.EndType.expired
                };

                s_dal.Assignment.Update(updatedAssignment);
            }
        }
    }

}
