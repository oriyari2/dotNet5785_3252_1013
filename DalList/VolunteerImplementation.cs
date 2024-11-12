using DalApi;
using DO;
namespace Dal;

public class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
        Volunteer NewItem = item with { Id = Config.NextAssignmentId };//create new item with right id, and any other details like item 
        DataSource.Volunteers.Add(NewItem);   //add new item to list
    }

    public void Delete(int id)
    {
        Volunteer? objToDelete = Read(id); //return obj needed if exit by the func read

        if (objToDelete != null)
        {
            DataSource.Volunteers.Remove(objToDelete); //delete obj from list
        }
        else
        {
            throw new Exception($"Object with Id {id} not found."); //didnt find

        }
    }

    public void DeleteAll()
    {
        DataSource.Volunteers.Clear(); //delete all objects from list the list
    }

    public Volunteer? Read(int id)
    {
        return DataSource.Volunteers.Find(obj => obj.Id == id);
        //lambada func that check if there is obj with id in the list
        //and return it,else return null
    }

    public List<Volunteer> ReadAll()
    {
        return new List<Volunteer>(DataSource.Volunteers);
        //return list that is copy of the list Volunteers that in DataSource
    }

    public void Update(Volunteer item)
    {
        Delete(item.Id); //if exist obj with item.Id,delete it,else throw Exception
        Create(item); //if didnt throw Exception in previous row, add item to list
    }
}
