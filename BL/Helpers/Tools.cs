using System.Reflection;

namespace Helpers;

internal static class Tools
{
    internal static string ToStringProperty<T>(this T t)
    {
        string str = "";
        foreach (PropertyInfo item in t.GetType().GetProperties())
            str += "\n" + item.Name + ": " + item.GetValue(t, null);
        return str;
    }
    internal static void IsNumericField(string input)
    {
        // אם הקלט ריק או מכיל רק רווחים
        if (string.IsNullOrWhiteSpace(input))
            throw new BO.BlInvalidValueExeption("");
        // מנסה להמיר את הקלט למספר שלם (או מספר נקודה צפה) לפי הצורך
        if (int.TryParse(input, out _) || double.TryParse(input, out _) == false)
            throw new BO.BlInvalidValueExeption("Invalid Phone Number"); 
    }
}
