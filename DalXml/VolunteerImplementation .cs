namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;

/// <summary>
/// Implementation of the IVolunteer interface for managing volunteer data using XML.
/// </summary>
internal class VolunteerImplementation : IVolunteer
{
    /// <summary>
    /// Creates a new Volunteer and saves it to the XML file.
    /// </summary>
    /// <param name="item">The Volunteer to be created.</param>
    /// <exception cref="DalAlreadyExistsException">
    /// Thrown if a Volunteer with the same ID already exists.
    /// </exception>
    public void Create(Volunteer item)
    {
        List<Volunteer> Volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        if (Read(item.Id) == null)
        { 
            Volunteers.Add(item);
        }
        else
            throw new DalAlreadyExistsException($"Volunteer with ID={item.Id} already exists");
        XMLTools.SaveListToXMLSerializer(Volunteers, Config.s_volunteers_xml);
    }

    /// <summary>
    /// Deletes a Volunteer with the specified ID from the XML file.
    /// </summary>
    /// <param name="id">The ID of the Volunteer to be deleted.</param>
    /// <exception cref="DalDoesNotExistException">
    /// Thrown if the Volunteer does not exist.
    /// </exception>
    public void Delete(int id)
    {
        List<Volunteer> Volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        if (Volunteers.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Volunteer with ID={id} does not exist");
        XMLTools.SaveListToXMLSerializer(Volunteers, Config.s_volunteers_xml);
    }

    /// <summary>
    /// Deletes all Volunteers by saving an empty list to the XML file.
    /// </summary>
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Volunteer>(), Config.s_volunteers_xml);
    }

    /// <summary>
    /// Reads a Volunteer with the specified ID from the XML file.
    /// </summary>
    /// <param name="id">The ID of the Volunteer to be read.</param>
    /// <returns>
    /// The Volunteer with the specified ID, or null if no such Volunteer exists.
    /// </returns>
    public Volunteer? Read(int id)
    {
        List<Volunteer> Volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        XMLTools.SaveListToXMLSerializer(Volunteers, Config.s_volunteers_xml);
        return Volunteers.FirstOrDefault(obj => obj.Id == id);
    }

    /// <summary>
    /// Reads a Volunteer based on a provided filter function.
    /// </summary>
    /// <param name="filter">The filter function to select the desired Volunteer.</param>
    /// <returns>
    /// The first Volunteer that matches the filter, or null if no such Volunteer exists.
    /// </returns>
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        List<Volunteer> Volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        XMLTools.SaveListToXMLSerializer(Volunteers, Config.s_volunteers_xml);
        return Volunteers.FirstOrDefault(filter);
    }

    /// <summary>
    /// Reads all Volunteers, optionally filtering them based on a provided function.
    /// </summary>
    /// <param name="filter">
    /// An optional filter function to select specific Volunteers. If no filter is provided, all Volunteers are returned.
    /// </param>
    /// <returns>
    /// A collection of Volunteers that match the filter, or all Volunteers if no filter is provided.
    /// </returns>
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        List<Volunteer> Volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);

        IEnumerable<Volunteer> result = filter != null
            ? Volunteers.Where(filter)
            : Volunteers;

        XMLTools.SaveListToXMLSerializer(Volunteers, Config.s_volunteers_xml);

        return result;
    }

    /// <summary>
    /// Updates an existing Volunteer in the XML file.
    /// </summary>
    /// <param name="item">The updated Volunteer.</param>
    /// <exception cref="DalDoesNotExistException">
    /// Thrown if the Volunteer does not exist.
    /// </exception>
    public void Update(Volunteer item)
    {
        List<Volunteer> Volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        if (Volunteers.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Volunteer with ID={item.Id} does not exist");
        Volunteers.Add(item);
        XMLTools.SaveListToXMLSerializer(Volunteers, Config.s_volunteers_xml);
    }
}
