using DO;

namespace DalApi;

/// <summary>
/// Generic CRUD interface for managing data access layer (DAL) entities.
/// </summary>
/// <typeparam name="T">The type of entity managed by the CRUD operations. 
/// Must be a reference type.</typeparam>
public interface ICrud<T> where T : class
{
    void Create(T item); //Creates new entity object in DAL
    T? Read(int id); //Reads entity object by its ID 
    IEnumerable<T> ReadAll(Func<T, bool>? filter = null); //stage 2 only, Reads all entity objects
    void Update(T item); //Updates entity object
    void Delete(int id); //Deletes an object by its Id
    void DeleteAll(); //Delete all entity objects
    T? Read(Func<T, bool> filter); // stage 2

}

