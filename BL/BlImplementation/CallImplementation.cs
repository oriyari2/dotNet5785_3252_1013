namespace BlImplementation;
using BlApi;
using Helpers;

using System.Collections.Generic;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public IEnumerable<int> CallsAmount()
    {
        throw new NotImplementedException();
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

    public IEnumerable<BO.CallInList> ReadAll(BO.FieldsCallInList filter, object? toFilter, BO.FieldsCallInList toSort)
    {
        throw new NotImplementedException();
    }

    public void Update(int id, BO.Call call)
    {
        throw new NotImplementedException();
    }
}
