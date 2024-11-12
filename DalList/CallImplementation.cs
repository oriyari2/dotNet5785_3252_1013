namespace Dal;
using DalApi;
using DO;

public class CallImplementation : ICall
{
    public void Create(Call item)
    {
        Call NewItem = item with { Id = Config.NextAssignmentId };//create new item with right id, and any other details like item 
        DataSource.Calls.Add(NewItem);   //add new item to list
    }

    public void Delete(int id)
    {
        Call? objToDelete = Read(id); //return obj needed if exit by the func read

        if (objToDelete != null)
        {
            DataSource.Calls.Remove(objToDelete); //delete obj from list
        }
        else
        {
            throw new Exception($"Object with Id {id} not found."); //didnt find

        }
    }

    public void DeleteAll()
    {
        DataSource.Calls.Clear(); //delete all objects from list the list
    }

    public Call? Read(int id)
    {
        return DataSource.Calls.Find(obj => obj.Id == id);
        //lambada func that check if there is obj with id in the list
        //and return it,else return null
    }

    public List<Call> ReadAll()
    {
        return new List<Call>(DataSource.Calls);
        //return list that is copy of the list Calls that in DataSource
    }

    public void Update(Call item)
    {
        Delete(item.Id); //if exist obj with item.Id,delete it,else throw Exception
        Create(item); //if didnt throw Exception in previous row, add item to list
    }
}
