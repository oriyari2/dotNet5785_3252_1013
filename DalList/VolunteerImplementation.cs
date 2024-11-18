using DalApi;
using DO;
namespace Dal;

/// <summary>
/// Implementation of the IVolunteer interface, handling CRUD operations for Volunteer entities.
/// </summary>
public class VolunteerImplementation : IVolunteer
{
    /// <summary>
    /// Adds a new Volunteer entity to the data source.
    /// </summary>
    /// <param name="item">The Volunteer object to be added.</param>
    /// <exception cref="Exception">Thrown if a Volunteer with the same ID already exists.</exception>
    public void Create(Volunteer item)
    {
        if (Read(item.Id) == null) // Check if a Volunteer with the same ID already exists
            DataSource.Volunteers.Add(item);   // Add the new Volunteer to the data source
        else // Throw an exception if a Volunteer with the same ID is found
            throw new Exception($"Volunteer with ID={item.Id} already exists");
    }

    /// <summary>
    /// Deletes a Volunteer entity from the data source by its ID.
    /// </summary>
    /// <param name="id">The ID of the Volunteer to delete.</param>
    /// <exception cref="Exception">Thrown if no Volunteer with the specified ID is found.</exception>
    public void Delete(int id)
    {
        Volunteer? objToDelete = Read(id); // Retrieve the Volunteer object using its ID

        if (objToDelete != null)
        {
            DataSource.Volunteers.Remove(objToDelete); // Remove the Volunteer from the data source
        }
        else
        {
            throw new Exception($"Object with Id {id} not found."); // Throw an exception if the Volunteer is not found
        }
    }

    /// <summary>
    /// Removes all Volunteer entities from the data source.
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Volunteers.Clear(); // Clear all Volunteer objects from the data source
    }

    /// <summary>
    /// Retrieves a Volunteer entity from the data source by its ID.
    /// </summary>
    /// <param name="id">The ID of the Volunteer to retrieve.</param>
    /// <returns>The Volunteer object if found; otherwise, null.</returns>
    public Volunteer? Read(int id)
    {
        return DataSource.Volunteers.Find(obj => obj.Id == id);
        // Use a lambda expression to find and return the Volunteer with the specified ID
        // Return null if no such Volunteer is found
    }

    /// <summary>
    /// Retrieves all Volunteer entities from the data source.
    /// </summary>
    /// <returns>A list of all Volunteer objects in the data source.</returns>
    public List<Volunteer> ReadAll()
    {
        return new List<Volunteer>(DataSource.Volunteers);
        // Return a new list containing all Volunteer objects in the data source
    }

    /// <summary>
    /// Updates an existing Volunteer entity in the data source.
    /// </summary>
    /// <param name="item">The updated Volunteer object.</param>
    /// <exception cref="Exception">Thrown if the Volunteer to update is not found.</exception>
    public void Update(Volunteer item)
    {
        Delete(item.Id); // Delete the existing Volunteer object by its ID; throw an exception if not found
        Create(item); // Add the updated Volunteer object to the data source
    }
}
