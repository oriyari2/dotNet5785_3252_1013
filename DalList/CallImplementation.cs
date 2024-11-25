namespace Dal;
using DalApi;
using DO;

/// <summary>
/// Implementation of the ICall interface, managing CRUD operations for Call entities.
/// </summary>
internal class CallImplementation : ICall
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
            throw new DalDoesNotExistException($"Object with Id {id} not found."); // Throw an exception if the Call is not found
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
        return DataSource.Calls.FirstOrDefault(obj => obj.Id == id);
        // Use a lambda expression to find the Call with the specified ID in the data source
        // Return the Call object if found, or null if not found
    }

    /// <summary>
    /// Reads all Call entities from the data source.
    /// </summary>
    /// <returns>A list containing all Call objects in the data source.</returns>
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null) // Defines a function that returns a collection of calls, with an optional custom filter
    => filter != null // Checks if a filter condition was provided
           ? from item in DataSource.Calls // If there is a filter, starts a query on the calls collection
             where filter(item) // Filters the calls based on the function passed as a parameter
             select item // Returns the calls that meet the filter condition
           : from item in DataSource.Calls // If no filter is provided, starts a query on the entire calls collection
             select item; // Returns all calls from the source (DataSource)


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

    /// <summary>
    /// Retrieves a Call entity from the data source that matches the given filter.
    /// </summary>
    /// <param name="filter">A predicate function to filter the Call objects.</param>
    /// <returns>The first matching Call object, or null if no match is found.</returns>
    public Call? Read(Func<Call, bool> filter)
    {
        return DataSource.Calls.FirstOrDefault(filter); // Searches for the first Call that matches the filter
    }

}
