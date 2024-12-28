namespace BlApi;

public interface IVolunteer: IObservable
{
    public BO.RoleType LogIn(string name, string password);

    public IEnumerable<BO.VolunteerInList>  ReadAll(bool? active, BO.FieldsVolunteerInList field= BO.FieldsVolunteerInList.Id, BO.CallType? callType = null);

    public BO.Volunteer Read(int id);

    public void Update(int id, BO.Volunteer volunteer);

    public void Delete(int id);

    public void Create(BO.Volunteer volunteer);

}
