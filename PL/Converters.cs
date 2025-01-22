using System.Globalization;
using System.Windows.Data;

namespace PL;

/// <summary>
/// Converter to determine if the action is "Update".
/// Returns true if the value is "Update", otherwise false.
/// </summary>
public class ConvertObjIdToTF : IValueConverter
{
    /// <summary>
    /// Converts the input value to a boolean based on whether it equals "Update".
    /// </summary>
    /// <param name="value">The input value.</param>
    /// <param name="targetType">The target type (ignored).</param>
    /// <param name="parameter">Optional parameter (ignored).</param>
    /// <param name="culture">The culture info.</param>
    /// <returns>True if the value is "Update", otherwise false.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value.ToString() == "Update"; // Checking if the value equals "Update"
    }

    /// <summary>
    /// ConvertBack is not implemented because this converter is one-way.
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException(); // No conversion back is required
    }
}

/// <summary>
/// Converter to determine if the action is NOT "Update".
/// Returns false if the value is "Update", otherwise true.
/// </summary>
public class ConvertObjPasswordToTF : IValueConverter
{
    /// <summary>
    /// Converts the input value to a boolean based on whether it does not equal "Update".
    /// </summary>
    /// <param name="value">The input value.</param>
    /// <param name="targetType">The target type (ignored).</param>
    /// <param name="parameter">Optional parameter (ignored).</param>
    /// <param name="culture">The culture info.</param>
    /// <returns>False if the value is "Update", otherwise true.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value.ToString() != "Update"; // Checking if the value is NOT "Update"
    }

    /// <summary>
    /// ConvertBack is not implemented because this converter is one-way.
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException(); // No conversion back is required
    }
}

/// <summary>
/// Converter for displaying the progress status of a call.
/// Converts a <see cref="BO.CallInProgress"/> object to a formatted string.
/// </summary>
public class CallInProgressConverter : IValueConverter
{
    /// <summary>
    /// Converts a <see cref="BO.CallInProgress"/> object to a readable string format.
    /// </summary>
    /// <param name="value">The input value.</param>
    /// <param name="targetType">The target type (ignored).</param>
    /// <param name="parameter">Optional parameter (ignored).</param>
    /// <param name="culture">The culture info.</param>
    /// <returns>A formatted string representing the call details, or null if invalid.</returns>
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BO.CallInProgress callInProgress) // Check if the value is a CallInProgress object
        {
            // Construct a formatted string with call details
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

        return null; // Return null if the value is not a valid CallInProgress object
    }

    /// <summary>
    /// ConvertBack is not implemented because this converter is one-way.
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("ConvertBack is not supported."); // No conversion back is required
    }
}

/// <summary>
/// Converter to check if the role type is "manager".
/// Returns true if the role is "manager", otherwise false.
/// </summary>
public class ConvertRoleToTF : IValueConverter
{
    /// <summary>
    /// Converts the input role type to a boolean indicating whether it is "manager".
    /// </summary>
    /// <param name="value">The input value.</param>
    /// <param name="targetType">The target type (ignored).</param>
    /// <param name="parameter">Optional parameter (ignored).</param>
    /// <param name="culture">The culture info.</param>
    /// <returns>True if the role is "manager", otherwise false.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null && Enum.TryParse(value.ToString(), out BO.RoleType role)) // Try parsing the value to a RoleType enum
        {
            return role == BO.RoleType.manager; // Check if the role is manager
        }

        return false; // Return false if the value is not a valid role
    }

    /// <summary>
    /// ConvertBack is not implemented because this converter is one-way.
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException(); // No conversion back is required
    }
}

/// <summary>
/// Converter to format DateTime values.
/// </summary>
public class DateTimeFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime dateTime) // Check if the value is a DateTime
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm"); // Format the DateTime
        }
        return value; // Return the original value if it's not a DateTime
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException(); // No conversion back is required
    }
}

/// <summary>
/// Converter for converting TimeSpan to a formatted string.
/// </summary>
public class TimeSpanToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TimeSpan timeSpan) // Check if the value is a TimeSpan
        {
            int days = (int)timeSpan.TotalDays; // Calculate total days
            int hours = timeSpan.Hours; // Get the hours
            int minutes = timeSpan.Minutes; // Get the minutes

            return $"{days}D {hours:D2}:{minutes:D2}"; // Return a formatted string
        }
        return string.Empty; // Return an empty string if the value is not a TimeSpan
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string timeString) // Check if the value is a string
        {
            try
            {
                // Split the string into days and time parts
                var parts = timeString.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 2 && parts[0].EndsWith("D"))
                {
                    string daysPart = parts[0].TrimEnd('D'); // Remove the 'D' character from days
                    string timePart = parts[1]; // Get the time part

                    if (int.TryParse(daysPart, out int days) && TimeSpan.TryParseExact(timePart, "hh\\:mm", culture, out TimeSpan timeOfDay))
                    {
                        return new TimeSpan(days, timeOfDay.Hours, timeOfDay.Minutes, 0); // Return a TimeSpan object
                    }
                }
            }
            catch
            {
                // Handle any parsing errors
            }
        }
        return TimeSpan.Zero; // Return zero TimeSpan if parsing fails
    }
}

/// <summary>
/// Converter for handling default opening time values.
/// </summary>
public class OpeningTimeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || (DateTime)value == default) // Check if the value is null or the default DateTime
        {
            return DateTime.Now; // Return the current system time
        }

        return value; // Return the existing value if it's not null or default
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value; // No conversion back is required
    }
}

/// <summary>
/// Converter for enabling/disabling checkboxes based on the current call.
/// </summary>
public class ActiveCheckboxConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value == null; // Enable if value is null, otherwise disable
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException(); // No conversion back is required
    }
}

/// <summary>
/// Converter for formatting double values with four decimal places.
/// </summary>
public class DoubleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double doubleValue) // Check if the value is a double
        {
            return doubleValue.ToString("F4", CultureInfo.InvariantCulture); // Format the double with four decimal places
        }

        return value; // Return the original value if it's not a double
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string stringValue && double.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
        {
            return result; // Return the parsed double value
        }

        return value; // Return the original value if parsing fails
    }
}

/// <summary>
/// Converter to check if the value is not null.
/// </summary>
public class IsNotNullToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value != null; // Return true if the value is not null, otherwise false
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException(); // No conversion back is required
    }
}

/// <summary>
/// Converter for enabling selection based on multiple values.
/// </summary>
public class IsEnabledSelectConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2) return false; // Check if there are at least two values

        var firstValue = values[0]; // Get the first value
        var secondValue = values[1] as bool?; // Get the second value as a boolean

        return firstValue == null && secondValue == true; // Enable if the first value is null and second value is true
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException(); // No conversion back is required
    }
}
