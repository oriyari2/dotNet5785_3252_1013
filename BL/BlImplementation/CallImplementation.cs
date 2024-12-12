 namespace BlImplementation;
using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;
using System;
using System.Collections.Generic;

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
        return Enumerable.Range(0, Enum.GetValues(typeof(Status)).Length)
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
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public void EndTreatment(int volunteerId, int AssignmentId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.ClosedCallInList> GetClosedCallInList(int id, BO.CallType? filter, BO.FieldsClosedCallInList toSort)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.ClosedCallInList> GetOpenCallInList(int id, BO.CallType? filter, BO.FieldsOpenCallInList toSort)
    {
        throw new NotImplementedException();
    }

    public BO.Call Read(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.CallInList> ReadAll(BO.FieldsCallInList? filter, object? toFilter, BO.FieldsCallInList? toSort)
    {
        var listCall = _dal.Call.ReadAll();
        var listAssignment = _dal.Assignment.ReadAll();
        var callInList = from item in listCall
                         let assignment = listAssignment.Where(s => s.CallId == item.Id).OrderByDescending(s => s.EntryTime).FirstOrDefault()
                         select new BO.CallInProgress
                         {
                             Id = assignment != null ? assignment.Id : null,
                         }

        listCall = listCall.Where(call =>
        {
            return filter switch
            {
                BO.FieldsCallInList.Id => call.Id.Equals(toFilter),
                BO.FieldsCallInList.CallId => call.Equals(toFilter),
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

    public void Update(int id, BO.Call call)
    {
        throw new NotImplementedException();
    }
}
