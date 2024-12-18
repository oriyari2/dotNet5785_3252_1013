namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;

internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Creates a new Assignment, assigns it a unique ID, and saves it to the XML file.
    /// </summary>
    /// <param name="item">The Assignment to be created.</param>
    public void Create(Assignment item)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);

        int nextId = Config.NextAssignmentId;
        Assignment copy = item with { Id = nextId };
        Assignments.Add(copy);

        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignments_xml);
    }

    /// <summary>
    /// Deletes an Assignment with the specified ID from the XML file.
    /// </summary>
    /// <param name="id">The ID of the Assignment to be deleted.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if the Assignment does not exist.</exception>
    public void Delete(int id)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (Assignments.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Assignment with ID={id} does not exist");
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignments_xml);
    }

    /// <summary>
    /// Deletes all Assignments by saving an empty list to the XML file.
    /// </summary>
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_assignments_xml);
    }

    /// <summary>
    /// Reads an Assignment with the specified ID from the XML file.
    /// </summary>
    /// <param name="id">The ID of the Assignment to be read.</param>
    /// <returns>The Assignment with the specified ID, or null if not found.</returns>
    public Assignment? Read(int id)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignments_xml);
        return Assignments.FirstOrDefault(obj => obj.Id == id);
    }

    /// <summary>
    /// Reads an Assignment based on a provided filter function.
    /// </summary>
    /// <param name="filter">The filter function to select the desired Assignment.</param>
    /// <returns>The first Assignment that matches the filter, or null if none match.</returns>
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignments_xml);
        return Assignments.FirstOrDefault(filter);
    }

    /// <summary>
    /// Reads all Assignments, optionally filtering them based on a provided function.
    /// </summary>
    /// <param name="filter">An optional filter function to select specific Assignments.</param>
    /// <returns>A collection of Assignments that match the filter, or all Assignments if no filter is provided.</returns>
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);

        if(filter == null) 
        {
            return Assignments;
        }
        IEnumerable<Assignment> result = from item in Assignments
                                         where filter(item)
                                         select item;
        var toResult= result.ToList();
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignments_xml);

        return toResult;
    }

    /// <summary>
    /// Updates an existing Assignment in the XML file.
    /// </summary>
    /// <param name="item">The updated Assignment.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if the Assignment does not exist.</exception>
    public void Update(Assignment item)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (Assignments.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Assignment with ID={item.Id} does not exist");
        Assignments.Add(item);
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignments_xml);
    }
}
