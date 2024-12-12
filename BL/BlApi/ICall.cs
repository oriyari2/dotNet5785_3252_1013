


namespace BlApi;

public interface ICall
{
    public IEnumerable<int> CallsAmount();

    public IEnumerable<BO.CallInList> ReadAll(BO.FieldsCallInList? filter,object? toFilter, BO.FieldsCallInList? toSort);
    public BO.Call Read(int id);
    public void Update(BO.Call call); 
    public void Delete(int id);
    public void Create(BO.Call call);
    public IEnumerable<BO.ClosedCallInList> GetClosedCallInList(int id,BO.CallType? filter, BO.FieldsClosedCallInList toSort );
    public IEnumerable<BO.ClosedCallInList> GetOpenCallInList(int id, BO.CallType? filter, BO.FieldsOpenCallInList toSort);
    public void EndTreatment(int volunteerId, int AssignmentId);
    public void CancelTreatment(int RequesterId, int AssignmentId);
    public void ChooseCallToTreat (int volunteerId, int CallId);

}
