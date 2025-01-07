namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

/// <summary>
/// Implementation of the ICall interface for managing calls data using XML.
/// </summary>
internal class CallImplementation : ICall
{
    /// <summary>
    /// Converts an XElement to a Call object.
    /// </summary>
    /// <param name="s">The XElement representing a Call.</param>
    /// <returns>A Call object populated with data from the XElement.</returns>
    /// <exception cref="FormatException">Thrown if data conversion fails.</exception>
    static Call getCall(XElement s)
    {
        return new DO.Call()
        {
            Id = s.ToIntNullable("Id") ?? throw new FormatException("can't convert id"),
            TheCallType = s.ToEnumNullable<DO.CallType>("CallType") ?? throw new FormatException("can't convert CallType"),
            VerbalDescription = (string?)s.Element("VerbalDescription") ?? null,
            Address= (string?)s.Element("Address") ??"",
            Latitude = s.ToDoubleNullable("Latitude") ?? throw new FormatException("can't convert double"),
            Longitude = s.ToDoubleNullable("Longitude") ?? throw new FormatException("can't convert double"),
            OpeningTime = s.ToDateTimeNullable("OpeningTime") ?? throw new FormatException("can't convert DateTime"),
            MaxTimeToEnd = s.ToDateTimeNullable("MaxTimeToEnd") ?? default(DateTime)
        };
    }

    /// <summary>
    /// Converts a Call object to an XElement.
    /// </summary>
    /// <param name="item">The Call object to convert.</param>
    /// <returns>An XElement representing the Call.</returns>
    static XElement createCallElement(Call item)
    {
        return new XElement("Call",
            new XElement("Id", Config.NextAssignmentId),
            new XElement("CallType", item.TheCallType),
            new XElement("VerbalDescription", item.VerbalDescription),
            new XElement("Address",item.Address),
            new XElement("Latitude", item.Latitude),
            new XElement("Longitude", item.Longitude),
            new XElement("OpeningTime", item.OpeningTime),
            new XElement("MaxTimeToEnd", item.MaxTimeToEnd)
        );
    }

    static XElement updateCallElement(Call item)
    {
        return new XElement("Call",
            new XElement("Id", item.Id),
            new XElement("CallType", item.TheCallType),
            new XElement("VerbalDescription", item.VerbalDescription),
            new XElement("Address", item.Address),
            new XElement("Latitude", item.Latitude),
            new XElement("Longitude", item.Longitude),
            new XElement("OpeningTime", item.OpeningTime),
            new XElement("MaxTimeToEnd", item.MaxTimeToEnd)
        );
    }

    /// <summary>
    /// Adds a new Call to the XML data.
    /// </summary>
    /// <param name="item">The Call to add.</param>
    /// <exception cref="DO.DalAlreadyExistsException">Thrown if a Call with the same ID already exists.</exception>
    public void Create(Call item)
    {
        XElement callsRootElem = XMLTools.LoadListFromXMLElement(Config.s_calls_xml);

        //if (callsRootElem.Elements().Any(st => (int?)st.Element("Id") == item.Id))
        //    throw new DO.DalAlreadyExistsException($"Call with ID={item.Id} already exists");

        callsRootElem.Add(createCallElement(item));
        XMLTools.SaveListToXMLElement(callsRootElem, Config.s_calls_xml);
    }

    /// <summary>
    /// Deletes a Call by its ID.
    /// </summary>
    /// <param name="id">The ID of the Call to delete.</param>
    /// <exception cref="DO.DalDoesNotExistException">Thrown if the Call does not exist.</exception>
    public void Delete(int id)
    {
        XElement callsRootElem = XMLTools.LoadListFromXMLElement(Config.s_calls_xml);

        XElement? callElem = callsRootElem.Elements().FirstOrDefault(st => (int?)st.Element("Id") == id);
        if (callElem == null)
            throw new DO.DalDoesNotExistException($"Call with ID={id} does not exist");

        callElem.Remove();
        XMLTools.SaveListToXMLElement(callsRootElem, Config.s_calls_xml);
    }

    /// <summary>
    /// Deletes all Calls from the XML data.
    /// </summary>
    public void DeleteAll()
    {
        XElement callsRootElem = new XElement("Calls");
        XMLTools.SaveListToXMLElement(callsRootElem, Config.s_calls_xml);
    }

    /// <summary>
    /// Reads a Call by its ID.
    /// </summary>
    /// <param name="id">The ID of the Call to read.</param>
    /// <returns>The Call object if found, or null if not.</returns>
    public Call? Read(int id)
    {
        XElement? callElem = XMLTools.LoadListFromXMLElement(Config.s_calls_xml).Elements().FirstOrDefault(st => (int?)st.Element("Id") == id);
        return callElem is null ? null : getCall(callElem);
    }

    /// <summary>
    /// Reads a Call that matches a given filter.
    /// </summary>
    /// <param name="filter">The filter function to apply.</param>
    /// <returns>The first Call object matching the filter, or null if none are found.</returns>
    public Call? Read(Func<Call, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_calls_xml).Elements().Select
            (s => getCall(s)).FirstOrDefault(filter);
    }

    /// <summary>
    /// Reads all Calls, optionally filtering them.
    /// </summary>
    /// <param name="filter">An optional filter function.</param>
    /// <returns>An enumerable collection of Calls matching the filter, or all Calls if no filter is provided.</returns>
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {

        IEnumerable<Call> allCalls = XMLTools.LoadListFromXMLElement(Config.s_calls_xml)
                   .Elements().Select(s => getCall(s));
        return filter is null ? allCalls : allCalls.Where(filter);
    }

    /// <summary>
    /// Updates an existing Call in the XML data.
    /// </summary>
    /// <param name="item">The updated Call object.</param>
    /// <exception cref="DO.DalDoesNotExistException">Thrown if the Call does not exist.</exception>
    public void Update(Call item)
    {
        XElement callsRootElem = XMLTools.LoadListFromXMLElement(Config.s_calls_xml);

        XElement? callElem = callsRootElem.Elements()
            .FirstOrDefault(st => (int?)st.Element("Id") == item.Id);

        if (callElem == null)
            throw new DO.DalDoesNotExistException($"Call with ID={item.Id} does not exist");

        callElem.Remove();
        callsRootElem.Add(updateCallElement(item));

        XMLTools.SaveListToXMLElement(callsRootElem, Config.s_calls_xml);
    }
}
