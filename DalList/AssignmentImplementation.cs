namespace Dal;
using DalApi;
using DO;
using System.Runtime.CompilerServices;

/// <summary>
/// Implementation of the IAssignment interface for managing Assignment entities in the data source.
/// </summary>
internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Creates a new Assignment entity and adds it to the data source.
    /// The new entity is assigned a unique ID based on the configuration.
    /// </summary>
    /// <param name="item">The Assignment entity to be created.</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Assignment item)
    {
        Assignment NewItem = item with { Id = Config.NextAssignmentId }; // Create new item with unique ID
        DataSource.Assignments.Add(NewItem); // Add new item to the data source
    }

    /// <summary>
    /// Deletes an Assignment entity from the data source by its ID.
    /// Throws an exception if the entity is not found
    /// </summary>
    /// <param name="id">The ID of the Assignment entity to be deleted.</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        Assignment? objToDelete = Read(id); // Retrieve the entity using its ID

        if (objToDelete != null)
        {
            DataSource.Assignments.Remove(objToDelete); // Remove the entity from the data source
        }
        else
        {
            throw new DalDoesNotExistException($"Object with Id {id} not found."); // Entity not found, throw exception
        }
    }

    /// <summary>
    /// Deletes all Assignment entities from the data source.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Assignments.Clear(); // Clear all entities from the data source
    }

    /// <summary>
    /// Reads an Assignment entity from the data source by its ID.
    /// Returns the entity if found, otherwise returns null.
    /// </summary>
    /// <param name="id">The ID of the Assignment entity to be read.</param>
    /// <returns>The Assignment entity with the specified ID, or null if not found.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(int id)
    {
        return DataSource.Assignments.FirstOrDefault(obj => obj.Id == id);
        // Use a lambda function to search for the entity by ID and return it, or null if not found
    }

    /// <summary>
    /// Reads all Assignment entities from the data source.
    /// Returns a copy of the data source's Assignments list.
    /// </summary>
    /// <returns>A list of all Assignment entities.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null) // Defines a function that returns a collection of assignments, with an optional custom filter
 => filter != null // Checks if a filter condition was provided
        ? from item in DataSource.Assignments // If there is a filter, starts a query on the assignments collection
          where filter(item) // Filters the assignments based on the function passed as a parameter
          select item // Returns the assignments that meet the filter condition
        : from item in DataSource.Assignments // If no filter is provided, starts a query on the entire assignments collection
          select item; // Returns all assignments from the source (DataSource)



    /// <summary>
    /// Updates an existing Assignment entity in the data source.
    /// The existing entity is deleted, and the updated entity is added.
    /// Throws an exception if the entity with the given ID does not exist.
    /// </summary>
    /// <param name="item">The updated Assignment entity.</param>
    /// <exception cref="Exception">Thrown if the Assignment to update is not found.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Assignment item)
    {
        Delete(item.Id); // Delete the existing entity if it exists, or throw an exception
        Create(item); // Add the updated entity to the data source
    }

    /// <summary>
    /// Retrieves an Assignment entity from the data source that matches the given filter.
    /// </summary>
    /// <param name="filter">A predicate function to filter the Assignment objects.</param>
    /// <returns>The first matching Assignment object, or null if no match is found.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return DataSource.Assignments.FirstOrDefault(filter); // Searches for the first Assignment that matches the filter
    }

}
