namespace Dal;

using DO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

/// <summary>
/// Provides utility methods for working with XML files, including serialization and deserialization.
/// </summary>
static class XMLTools
{
    /// <summary>
    /// The directory path where XML files are stored.
    /// </summary>
    const string s_xmlDir = @"..\xml\";

    /// <summary>
    /// Static constructor to ensure the XML directory exists.
    /// </summary>
    static XMLTools()
    {
        if (!Directory.Exists(s_xmlDir))
            Directory.CreateDirectory(s_xmlDir);
    }

    #region SaveLoadWithXMLSerializer

    /// <summary>
    /// Saves a list of objects to an XML file using XML serialization.
    /// </summary>
    /// <typeparam name="T">The type of objects in the list.</typeparam>
    /// <param name="list">The list of objects to save.</param>
    /// <param name="xmlFileName">The name of the XML file to save to.</param>
    /// <exception cref="DalXMLFileLoadCreateException">
    /// Thrown if there is an issue creating or writing to the XML file.
    /// </exception>
    public static void SaveListToXMLSerializer<T>(List<T> list, string xmlFileName) where T : class
    {
        string xmlFilePath = s_xmlDir + xmlFileName;

        try
        {
            using FileStream file = new(xmlFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
            new XmlSerializer(typeof(List<T>)).Serialize(file, list);
        }
        catch (Exception ex)
        {
            throw new DalXMLFileLoadCreateException($"Failed to create XML file: {s_xmlDir + xmlFilePath}, {ex.Message}");
        }
    }

    /// <summary>
    /// Loads a list of objects from an XML file using XML serialization.
    /// </summary>
    /// <typeparam name="T">The type of objects in the list.</typeparam>
    /// <param name="xmlFileName">The name of the XML file to load from.</param>
    /// <returns>A list of objects loaded from the XML file. If the file does not exist, returns an empty list.</returns>
    /// <exception cref="DalXMLFileLoadCreateException">
    /// Thrown if there is an issue loading or deserializing the XML file.
    /// </exception>
    public static List<T> LoadListFromXMLSerializer<T>(string xmlFileName) where T : class
    {
        string xmlFilePath = s_xmlDir + xmlFileName;

        try
        {
            if (!File.Exists(xmlFilePath)) return new();
            using FileStream file = new(xmlFilePath, FileMode.Open);
            XmlSerializer x = new(typeof(List<T>));
            return x.Deserialize(file) as List<T> ?? new();
        }
        catch (Exception ex)
        {
            throw new DalXMLFileLoadCreateException($"Failed to load XML file: {xmlFilePath}, {ex.Message}");
        }
    }

    #endregion



    #region SaveLoadWithXElement

    /// <summary>
    /// Saves an XElement to an XML file.
    /// </summary>
    /// <param name="rootElem">The root XElement to save.</param>
    /// <param name="xmlFileName">The name of the XML file to save to.</param>
    /// <exception cref="DalXMLFileLoadCreateException">
    /// Thrown if there is an issue creating or writing to the XML file.
    /// </exception>
    public static void SaveListToXMLElement(XElement rootElem, string xmlFileName)
    {
        string xmlFilePath = s_xmlDir + xmlFileName;

        try
        {
            rootElem.Save(xmlFilePath);
        }
        catch (Exception ex)
        {
            throw new DalXMLFileLoadCreateException($"Failed to create XML file: {s_xmlDir + xmlFilePath}, {ex.Message}");
        }
    }

    /// <summary>
    /// Loads an XElement from an XML file. If the file does not exist, creates a new XElement with the specified name.
    /// </summary>
    /// <param name="xmlFileName">The name of the XML file to load from.</param>
    /// <returns>The loaded XElement or a new XElement if the file does not exist.</returns>
    /// <exception cref="DalXMLFileLoadCreateException">
    /// Thrown if there is an issue loading the XML file.
    /// </exception>
    public static XElement LoadListFromXMLElement(string xmlFileName)
    {
        string xmlFilePath = s_xmlDir + xmlFileName;

        try
        {
            if (File.Exists(xmlFilePath))
                return XElement.Load(xmlFilePath);
            XElement rootElem = new(xmlFileName);
            rootElem.Save(xmlFilePath);
            return rootElem;
        }
        catch (Exception ex)
        {
            throw new DalXMLFileLoadCreateException($"Failed to load XML file: {s_xmlDir + xmlFilePath}, {ex.Message}");
        }
    }

    #endregion

    #region XmlConfig

    /// <summary>
    /// Retrieves and increments an integer value in the specified XML file.
    /// </summary>
    /// <param name="xmlFileName">The name of the XML file.</param>
    /// <param name="elemName">The name of the element containing the integer value.</param>
    /// <returns>The current integer value before incrementing.</returns>
    /// <exception cref="FormatException">
    /// Thrown if the element value cannot be converted to an integer.
    /// </exception>
    public static int GetAndIncreaseConfigIntVal(string xmlFileName, string elemName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        int nextId = root.ToIntNullable(elemName) ?? throw new FormatException($"Can't convert: {xmlFileName}, {elemName}");
        root.Element(elemName)?.SetValue((nextId + 1).ToString());
        XMLTools.SaveListToXMLElement(root, xmlFileName);
        return nextId;
    }

    /// <summary>
    /// Retrieves an integer value from the specified XML file.
    /// </summary>
    /// <param name="xmlFileName">The name of the XML file.</param>
    /// <param name="elemName">The name of the element containing the integer value.</param>
    /// <returns>The integer value.</returns>
    /// <exception cref="FormatException">
    /// Thrown if the element value cannot be converted to an integer.
    /// </exception>
    public static int GetConfigIntVal(string xmlFileName, string elemName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        int num = root.ToIntNullable(elemName) ?? throw new FormatException($"Can't convert: {xmlFileName}, {elemName}");
        return num;
    }

    /// <summary>
    /// Retrieves a DateTime value from the specified XML file.
    /// </summary>
    /// <param name="xmlFileName">The name of the XML file.</param>
    /// <param name="elemName">The name of the element containing the DateTime value.</param>
    /// <returns>The DateTime value.</returns>
    /// <exception cref="FormatException">
    /// Thrown if the element value cannot be converted to a DateTime.
    /// </exception>
    public static DateTime GetConfigDateVal(string xmlFileName, string elemName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        DateTime dt = root.ToDateTimeNullable(elemName) ?? throw new FormatException($"Can't convert: {xmlFileName}, {elemName}");
        return dt;
    }

    /// <summary>
    /// Sets an integer value in the specified XML file.
    /// </summary>
    /// <param name="xmlFileName">The name of the XML file.</param>
    /// <param name="elemName">The name of the element to set.</param>
    /// <param name="elemVal">The integer value to set.</param>
    public static void SetConfigIntVal(string xmlFileName, string elemName, int elemVal)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        root.Element(elemName)?.SetValue((elemVal).ToString());
        XMLTools.SaveListToXMLElement(root, xmlFileName);
    }

    /// <summary>
    /// Sets a DateTime value in the specified XML file.
    /// </summary>
    /// <param name="xmlFileName">The name of the XML file.</param>
    /// <param name="elemName">The name of the element to set.</param>
    /// <param name="elemVal">The DateTime value to set.</param>
    public static void SetConfigDateVal(string xmlFileName, string elemName, DateTime elemVal)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        root.Element(elemName)?.SetValue((elemVal).ToString());
        XMLTools.SaveListToXMLElement(root, xmlFileName);
    }

    #endregion



    #region ExtensionFuctions

    /// <summary>
    /// Converts the value of an XElement to a nullable enum of type T.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="element">The XElement containing the value.</param>
    /// <param name="name">The name of the element to convert.</param>
    /// <returns>A nullable enum of type T if conversion is successful, otherwise null.</returns>
    public static T? ToEnumNullable<T>(this XElement element, string name) where T : struct, Enum =>
        Enum.TryParse<T>((string?)element.Element(name), out var result) ? (T?)result : null;

    /// <summary>
    /// Converts the value of an XElement to a nullable DateTime.
    /// </summary>
    /// <param name="element">The XElement containing the value.</param>
    /// <param name="name">The name of the element to convert.</param>
    /// <returns>A nullable DateTime if conversion is successful, otherwise null.</returns>
    public static DateTime? ToDateTimeNullable(this XElement element, string name) =>
        DateTime.TryParse((string?)element.Element(name), out var result) ? (DateTime?)result : null;

    /// <summary>
    /// Converts the value of an XElement to a nullable double.
    /// </summary>
    /// <param name="element">The XElement containing the value.</param>
    /// <param name="name">The name of the element to convert.</param>
    /// <returns>A nullable double if conversion is successful, otherwise null.</returns>
    public static double? ToDoubleNullable(this XElement element, string name) =>
        double.TryParse((string?)element.Element(name), out var result) ? (double?)result : null;

    /// <summary>
    /// Converts the value of an XElement to a nullable integer.
    /// </summary>
    /// <param name="element">The XElement containing the value.</param>
    /// <param name="name">The name of the element to convert.</param>
    /// <returns>A nullable integer if conversion is successful, otherwise null.</returns>
    public static int? ToIntNullable(this XElement element, string name) =>
        int.TryParse((string?)element.Element(name), out var result) ? (int?)result : null;

    #endregion

    #region TimeSpan Handling

    /// <summary>
    /// Converts the value of an XElement to a nullable TimeSpan.
    /// </summary>
    /// <param name="element">The XElement containing the value.</param>
    /// <param name="name">The name of the element to convert.</param>
    /// <returns>A nullable TimeSpan if conversion is successful, otherwise null.</returns>
    public static TimeSpan? ToTimeSpanNullable(this XElement element, string name) =>
        TimeSpan.TryParse((string?)element.Element(name), out var result) ? (TimeSpan?)result : null;

    /// <summary>
    /// Sets a TimeSpan value for the specified element in an XML file.
    /// </summary>
    /// <param name="xmlFileName">The name of the XML file.</param>
    /// <param name="elemName">The name of the element to set.</param>
    /// <param name="elemVal">The TimeSpan value to set.</param>
    public static void SetConfigTimeSpanVal(string xmlFileName, string elemName, TimeSpan elemVal)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        root.Element(elemName)?.SetValue(elemVal.ToString());
        XMLTools.SaveListToXMLElement(root, xmlFileName);
    }

    /// <summary>
    /// Retrieves a TimeSpan value from the specified element in an XML file.
    /// </summary>
    /// <param name="xmlFileName">The name of the XML file.</param>
    /// <param name="elemName">The name of the element containing the TimeSpan value.</param>
    /// <returns>The TimeSpan value.</returns>
    /// <exception cref="FormatException">
    /// Thrown if the element value cannot be converted to a TimeSpan.
    /// </exception>
    public static TimeSpan GetConfigTimeSpanVal(string xmlFileName, string elemName)
    {
        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
        return root.ToTimeSpanNullable(elemName) ?? throw new FormatException($"Can't convert: {xmlFileName}, {elemName}");
    }

    #endregion


}