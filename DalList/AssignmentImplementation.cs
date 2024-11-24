namespace Dal;
using DalApi;
using DO;

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
    public void Create(Assignment item)
    {
        Assignment NewItem = item with { Id = Config.NextAssignmentId }; // Create new item with unique ID
        DataSource.Assignments.Add(NewItem); // Add new item to the data source
    }

    /// <summary>
    /// Deletes an Assignment entity from the data source by its ID.
    /// Throws an exception if the entity is not found.
    /// </summary>
    /// <param name="id">The ID of the Assignment entity to be deleted.</param>
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
    public Assignment? Read(int id)
    {
        return DataSource.Assignments.Find(obj => obj.Id == id);
        // Use a lambda function to search for the entity by ID and return it, or null if not found
    }

    /// <summary>
    /// Reads all Assignment entities from the data source.
    /// Returns a copy of the data source's Assignments list.
    /// </summary>
    /// <returns>A list of all Assignment entities.</returns>
    public List<Assignment> ReadAll()
    {
        return new List<Assignment>(DataSource.Assignments);
        // Return a copy of the Assignments list to ensure encapsulation
    }

    /// <summary>
    /// Updates an existing Assignment entity in the data source.
    /// The existing entity is deleted, and the updated entity is added.
    /// Throws an exception if the entity with the given ID does not exist.
    /// </summary>
    /// <param name="item">The updated Assignment entity.</param>
    public void Update(Assignment item)
    {
        Delete(item.Id); // Delete the existing entity if it exists, or throw an exception
        Create(item); // Add the updated entity to the data source
    }
}
