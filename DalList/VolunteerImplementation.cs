using DalApi;
using DO;
using System.Runtime.CompilerServices;
namespace Dal;

/// <summary>
/// Implementation of the IVolunteer interface, handling CRUD operations for Volunteer entities.
/// </summary>
internal class VolunteerImplementation : IVolunteer
{
    /// <summary>
    /// Adds a new Volunteer entity to the data source.
    /// </summary>
    /// <param name="item">The Volunteer object to be added.</param>
    /// <exception cref="Exception">Thrown if a Volunteer with the same ID already exists.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Volunteer item)
    {
        if (Read(item.Id) == null) // Check if a Volunteer with the same ID already exists
            DataSource.Volunteers.Add(item);   // Add the new Volunteer to the data source
        else // Throw an exception if a Volunteer with the same ID is found
            throw new DalAlreadyExistsException($"Volunteer with ID={item.Id} already exists");
    }

    /// <summary>
    /// Deletes a Volunteer entity from the data source by its ID.
    /// </summary>
    /// <param name="id">The ID of the Volunteer to delete.</param>
    /// <exception cref="Exception">Thrown if no Volunteer with the specified ID is found.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        Volunteer? objToDelete = Read(id); // Retrieve the Volunteer object using its ID

        if (objToDelete != null)
        {
            DataSource.Volunteers.Remove(objToDelete); // Remove the Volunteer from the data source
        }
        else
        {
            throw new DalDoesNotExistException($"Object with Id {id} not found."); // Throw an exception if the Volunteer is not found
        }
    }

    /// <summary>
    /// Removes all Volunteer entities from the data source.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Volunteers.Clear(); // Clear all Volunteer objects from the data source
    }

    /// <summary>
    /// Retrieves a Volunteer entity from the data source by its ID.
    /// </summary>
    /// <param name="id">The ID of the Volunteer to retrieve.</param>
    /// <returns>The Volunteer object if found; otherwise, null.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(int id)
    {
        return DataSource.Volunteers.FirstOrDefault(obj => obj.Id == id);
        // Use a lambda expression to find and return the Volunteer with the specified ID
        // Return null if no such Volunteer is found
    }

    /// <summary>
    /// Retrieves all Volunteer entities from the data source.
    /// </summary>
    /// <returns>A list of all Volunteer objects in the data source.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null) // Defines a function that returns a collection of volunteers, with an optional custom filter
   => filter != null  // Checks if a filter condition was provided
          ? from item in DataSource.Volunteers // If there is a filter, starts a query on the volunteers collection
            where filter(item) // Filters the volunteers based on the function passed as a parameter
            select item // Returns the volunteers that meet the filter condition
          : from item in DataSource.Volunteers // If no filter is provided, starts a query on the entire volunteers collection
            select item; // Returns all volunteers from the source (DataSource)


    /// <summary>
    /// Updates an existing Volunteer entity in the data source.
    /// The existing entity is deleted, and the updated entity is added.
    /// Throws an exception if the entity with the given ID does not exist.
    /// </summary>
    /// <param name="item">The updated Volunteer object.</param>
    /// <exception cref="Exception">Thrown if the Volunteer to update is not found.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Volunteer item)
    {
        Delete(item.Id); // Delete the existing Volunteer object by its ID; throw an exception if not found
        Create(item); // Add the updated Volunteer object to the data source
    }

    /// <summary>
    /// Retrieves a Volunteer entity from the data source that matches the given filter.
    /// </summary>
    /// <param name="filter">A predicate function to filter the Volunteer objects.</param>
    /// <returns>The first matching Volunteer object, or null if no match is found.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return DataSource.Volunteers.FirstOrDefault(filter); // Searches for the first Volunteer that matches the filter
    }

}
