namespace Dal;
using DalApi;
using DO;

/// <summary>
/// Implementation of the ICall interface, managing CRUD operations for Call entities.
/// </summary>
public class CallImplementation : ICall
{
    /// <summary>
    /// Creates a new Call entity and adds it to the data source.
    /// </summary>
    /// <param name="item">The Call object to be added.</param>
    public void Create(Call item)
    {
        Call newItem = item with { Id = Config.NextCallId }; // Create a new Call with the next available ID
        DataSource.Calls.Add(newItem); // Add the new Call to the data source
    }

    /// <summary>
    /// Deletes a Call entity from the data source by its ID.
    /// </summary>
    /// <param name="id">The ID of the Call to be deleted.</param>
    /// <exception cref="Exception">Thrown if the Call with the specified ID is not found.</exception>
    public void Delete(int id)
    {
        Call? objToDelete = Read(id); // Retrieve the Call object to delete using its ID

        if (objToDelete != null)
        {
            DataSource.Calls.Remove(objToDelete); // Remove the Call from the data source
        }
        else
        {
            throw new Exception($"Object with Id {id} not found."); // Throw an exception if the Call is not found
        }
    }

    /// <summary>
    /// Deletes all Call entities from the data source.
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Calls.Clear(); // Clear the entire list of Calls in the data source
    }

    /// <summary>
    /// Reads a Call entity from the data source by its ID.
    /// </summary>
    /// <param name="id">The ID of the Call to be read.</param>
    /// <returns>The Call object if found; otherwise, null.</returns>
    public Call? Read(int id)
    {
        return DataSource.Calls.Find(obj => obj.Id == id);
        // Use a lambda expression to find the Call with the specified ID in the data source
        // Return the Call object if found, or null if not found
    }

    /// <summary>
    /// Reads all Call entities from the data source.
    /// </summary>
    /// <returns>A list containing all Call objects in the data source.</returns>
    public List<Call> ReadAll()
    {
        return new List<Call>(DataSource.Calls);
        // Return a new list containing all Call objects from the data source
    }

    /// <summary>
    /// Updates an existing Call entity in the data source.
    /// </summary>
    /// <param name="item">The updated Call object.</param>
    /// <exception cref="Exception">Thrown if the Call to update is not found.</exception>
    public void Update(Call item)
    {
        Delete(item.Id); // Delete the existing Call object by its ID; throws an exception if not found
        Create(item); // Add the updated Call object to the data source
    }
}
