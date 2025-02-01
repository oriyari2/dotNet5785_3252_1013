using System.Reflection;

namespace Helpers;  // Declares the Helpers namespace, containing utility classes.

/// <summary>
/// Tools class containing helper methods for handling properties and validation.
/// </summary>
internal static class Tools
{
    /// <summary>
    /// Converts an object to a string representation of its properties.
    /// If the object is an IEnumerable (except strings), it processes each element.
    /// </summary>
    internal static string ToStringProperty<T>(this T t)
    {
        string str = "";  // Initializes an empty string to store the property values.

        // Check if the object is of type IEnumerable but not a string
        if (t is IEnumerable<T> enumerable && t is not string)
        {
            // Iterate over each element in the enumerable collection
            foreach (var elem in enumerable)
            {
                // Iterate over all properties of the element
                foreach (PropertyInfo item in elem.GetType().GetProperties())
                    str += "\n" + item.Name + ": " + item.GetValue(elem, null);  // Add property name and value to the string.
                str += "\n";  // Adds a blank line between items.
            }
        }
        else
        {
            // If the object is not an IEnumerable, iterate over its properties
            foreach (PropertyInfo item in t.GetType().GetProperties())
                str += "\n" + item.Name + ": " + item.GetValue(t, null);  // Add property name and value to the string.
        }
        return str;  // Returns the constructed string.
    } 
}
