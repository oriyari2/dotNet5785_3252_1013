namespace Dal;
using DalApi;
using DO;
//using System.Collections.Generic;##we can remove that vut the compiler add this

public class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        Assignment NewItem = item with { Id = Config.NextAssignmentId };//create new item with right id, and any other details like item 
        DataSource.Assignments.Add(NewItem);   //add new item to list 

    }

    public void Delete(int id)
    {
        Assignment? objToDelete = Read(id); //return obj needed if exit by the func read

        if (objToDelete != null)
        {
            DataSource.Assignments.Remove(objToDelete); //delete obj from list
        }
        else
        {
            throw new Exception($"Object with Id {id} not found."); //didnt find

        }
    }

    public void DeleteAll()
    {
        DataSource.Assignments.Clear(); //delete all objects from list the list
    }

    public Assignment? Read(int id)
    {
        return DataSource.Assignments.Find(obj => obj.Id == id);
        //lambada func that check if there is obj with id in the list
        //and return it,else return null

    }

    public List<Assignment> ReadAll()
    {
        return new List<Assignment>(DataSource.Assignments);
        //return list that is copy of the list Assignments that in DataSource
    }

    public void Update(Assignment item)
    {
        Delete(item.Id); //if exist obj with item.Id,delete it,else throw Exception
        Create(item); //if didnt throw Exception in previous row, add item to list
    }
}
