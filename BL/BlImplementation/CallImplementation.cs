﻿namespace BlImplementation;
using BlApi;

using Helpers;
using System;
using System.Collections.Generic;
using System.Net;
/*
        //CallManager
        //VolunteerManager 
*/
internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public IEnumerable<int> CallsAmount()
    {    // שליפת כל הקריאות באמצעות ReadAll
        var allCalls = ReadAll(null, null, null);

        // קיבוץ וספירה לפי סטטוס, תוך שימוש בערך המספרי של ה-Enum
        var grouped = allCalls
            .GroupBy(call => (int)call.status)
            .ToDictionary(group => group.Key, group => group.Count());

        // יצירת מערך בגודל ה-Enum, ומילויו בכמויות לפי הסטטוס
        return Enumerable.Range(0, Enum.GetValues(typeof(BO.Status)).Length)
                         .Select(index => grouped.ContainsKey(index) ? grouped[index] : 0)
                         .ToArray(); // ממיר את התוצאה למערך int[]
    }


    public void CancelTreatment(int RequesterId, int AssignmentId)
    {
        throw new NotImplementedException();
    }

    public void ChooseCallToTreat(int volunteerId, int CallId)
    {
        throw new NotImplementedException();
    } 

    public void Create(BO.Call call)
    {
        DO.Call doCall = CallManager.HelpCreateUodate(call);
        try
        {
            _dal.Call.Create(doCall);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID={call.Id} does not exists", ex);
        }
    }


    public void Delete(int id)
    {
      BO.Call call= Read(id);
    
     if (call.listAssignForCall.Count!=0)

            throw new BO.cantDeleteItem($"call with ID={id} can't be deleted");

        try
        {
            _dal.Volunteer.Delete(id);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does Not exist", ex);
        }
    }

    public void EndTreatment(int volunteerId, int AssignmentId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.ClosedCallInList> GetCloseCallsInList(int volunteerId,BO.CallType? callTypeFilter,BO.FieldsClosedCallInList? sortField)
    {
        // קבלת כל הקריאות מה-DAL
        var allCalls = _dal.Call.ReadAll();

        // קבלת כל השיבוצים מה-DAL
        var allAssignments = _dal.Assignment.ReadAll();

        // סינון לפי ת.ז של מתנדב והסטטוס סגור
        var filteredCalls = from call in allCalls
                            join assignment in allAssignments
                            on call.Id equals assignment.CallId
                            where assignment.VolunteerId == volunteerId && assignment.TheEndType != null
                            select new BO.ClosedCallInList
                            {
                                Id = call.Id,
                                TheCallType = (BO.CallType)call.TheCallType,
                                Address = call.Address,
                                OpeningTime = call.OpeningTime,
                                EntryTime = assignment.EntryTime,
                                ActualEndTime = assignment.ActualEndTime,
                                TheEndType = (BO.EndType)assignment.TheEndType
                            };

        // סינון לפי סוג קריאה אם קיים
        if (callTypeFilter.HasValue)
        {
            filteredCalls = filteredCalls.Where(c => c.TheCallType == callTypeFilter.Value);
        }

        // מיון לפי השדה המבוקש או לפי ברירת מחדל (מספר קריאה)
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
            filteredCalls = filteredCalls.OrderBy(c => c.Id);
        }

        return filteredCalls;
    }


    public IEnumerable<BO.OpenCallInList> GetOpenCallsInList(int volunteerId, BO.CallType? callTypeFilter, BO.FieldsOpenCallInList? sortField)
    {
        double volunteerLong, volunteerLat, callLong, callLat;
        // קבלת כל הקריאות מה-DAL
        var allCalls = ReadAll(null,null,null);

        // קבלת כל השיבוצים מה-DAL
        var allAssignments = _dal.Assignment.ReadAll();
        var volunteer = Read(volunteerId);
        // סינון לפי סטטוס "פתוחה" או "פתוחה בסיכון" בלבד
        var filteredCalls = from call in allCalls
                            join assignment in allAssignments on call.Id equals assignment.CallId into callAssignments
                            from assignment in callAssignments.DefaultIfEmpty()
                            where (call.status == BO.Status.open || call.status == BO.Status.riskOpen)
                            let theAddress= Read(call.CallId).Address
                            select new BO.OpenCallInList
                            {
                                Id = call.CallId,
                                TheCallType = call.TheCallType,
                                Address = theAddress,
                                OpeningTime = call.OpeningTime,
                                Distance = volunteer?.Address != null ?
          VolunteerManager.CalculateDistance(
              VolunteerManager.GetCoordinates(volunteer.Address,out  volunteerLong, out  volunteerLat),
              VolunteerManager.GetCoordinates(theAddress, out  callLong, out  callLat))
          : 0  // חישוב המרחק בין המתנדב לקריאה

                            };
        // סינון לפי סוג קריאה אם קיים
        if (callTypeFilter.HasValue)
        {
            filteredCalls = filteredCalls.Where(c => c.TheCallType == callTypeFilter.Value);
        }

        // מיון לפי השדה המבוקש או לפי ברירת מחדל (מספר קריאה)
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
            filteredCalls = filteredCalls.OrderBy(c => c.Id);
        }

        return filteredCalls;
    }



    public BO.Call Read(int id)
    {
        
        var call = _dal.Call.Read(id) ??
        throw new BO.BlDoesNotExistException($"Call with ID={id} does Not exist");
        var assignment = _dal.Assignment.ReadAll(s => s.CallId == id);
        return new BO.Call()
        {
            Id = call.Id,
            TheCallType = call.TheCallType,
            VerbalDescription = call.VerbalDescription,
            Address = call.Address,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            OpeningTime = call.OpeningTime,
            MaxTimeToEnd = call.MaxTimeToEnd,
            status = CallManager.CheckStatus(assignment,call),
            listAssignForCall= assignment==null?null: CallManager.GetCallAssignInList(assignment)
        };
        
    

}

    public IEnumerable<BO.CallInList> ReadAll(BO.FieldsCallInList? filter, object? toFilter, BO.FieldsCallInList? toSort)
    {


        var listCall = _dal.Call.ReadAll();
        var listAssignment = _dal.Assignment.ReadAll();
        var callInList = from item in listCall
                         let assignment = listAssignment.Where(s => s.CallId == item.Id).OrderByDescending(s => s.EntryTime).FirstOrDefault()
                         let volunteer = assignment != null ? _dal.Volunteer.Read(assignment.VolunteerId) : null
                         let TempTimeToEnd = item.MaxTimeToEnd - (ClockManager.Now)
                         select new BO.CallInList
                         {
                             Id = assignment != null ? assignment.Id : null,
                             CallId = item.Id,
                             TheCallType = (BO.CallType)item.TheCallType,
                             OpeningTime = item.OpeningTime,
                             TimeToEnd = TempTimeToEnd > TimeSpan.Zero ? TempTimeToEnd : null,
                             LastVolunteer = volunteer.Name,
                             CompletionTreatment = assignment.TheEndType != null ? assignment.ActualEndTime - item.OpeningTime : null,
                             status = CallManager.CheckStatus(assignment, item),
                             TotalAssignments = listAssignment.Where(s => s.CallId == item.Id).Count()
                         };


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
                    BO.FieldsCallInList.LastVolunteer => call.LastVolunteer.Equals(toFilter),
                    BO.FieldsCallInList.CompletionTreatment => call.CompletionTreatment.Equals(toFilter),
                    BO.FieldsCallInList.status => call.status.Equals(toFilter),
                    BO.FieldsCallInList.TotalAssignments => call.TotalAssignments.Equals(toFilter),
                    _ => true
                };
            });

        }

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
            callInList = callInList.OrderBy(call => call.CallId);
        }
        return callInList;
    }

    public void Update( BO.Call call)
    {
        DO.Call doCall = CallManager.HelpCreateUodate(call);
        try 
        { 
            _dal.Call.Update(doCall);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID={call.Id} does not exists", ex);
        }
    }
}
