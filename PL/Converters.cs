using System.Globalization;
using System.Windows.Data;

namespace PL;

// Converter to determine if the action is "Update"
public class ConvertObjIdToTF : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Check if the value is "Update" and return true; otherwise, return false
        if (value.ToString() == "Update")
        {
            return true;
        }
        return false;
    }

    // ConvertBack is not implemented because this converter is one-way
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

// Converter to determine if the action is NOT "Update"
public class ConvertObjPasswordToTF : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Check if the value is "Update" and return false; otherwise, return true
        if (value.ToString() == "Update")
        {
            return false;
        }
        return true;
    }

    // ConvertBack is not implemented because this converter is one-way
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


public class CallInProgressConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return null; // אם אין נתונים, מחזיר null

        if (value is BO.CallInProgress callInProgress)
        {
            // בניית מחרוזת המייצגת את הנתונים בצורה קריאה
            return $"ID: {callInProgress.Id}\n" +
                   $"Call ID: {callInProgress.CallId}\n" +
                   $"Type: {callInProgress.TheCallType}\n" +
                   $"Description: {callInProgress.VerbalDescription ?? "N/A"}\n" +
                   $"Address: {callInProgress.Address}\n" +
                   $"Opening Time: {callInProgress.OpeningTime}\n" +
                   $"Max Time to End: {callInProgress.MaxTimeToEnd?.ToString() ?? "N/A"}\n" +
                   $"Entry Time: {callInProgress.EntryTime}\n" +
                   $"Distance: {callInProgress.Distance} km\n" +
                   $"Status: {callInProgress.status}";
        }

        return null; // במקרה של סוג לא מתאים
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("ConvertBack is not supported.");
    }
}

// Converter to check if the role type is "manager"
public class ConvertRoleToTF : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Ensure the value is not null and can be parsed to the RoleType enum
        if (value != null && Enum.TryParse(value.ToString(), out BO.RoleType role))
        {
            // Return true if the role is "manager"; otherwise, return false
            return role == BO.RoleType.manager;
        }

        // Return false if the value is null or parsing fails
        return false;
    }

    // ConvertBack is not implemented because this converter is one-way
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
