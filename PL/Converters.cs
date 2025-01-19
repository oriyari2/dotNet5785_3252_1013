﻿using System.Globalization;
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
        return value.ToString() == "Update";
    }

    /// <summary>
    /// ConvertBack is not implemented because this converter is one-way.
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
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
        return value.ToString() != "Update";
    }

    /// <summary>
    /// ConvertBack is not implemented because this converter is one-way.
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
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
        if (value is BO.CallInProgress callInProgress)
        {
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

        return null;
    }

    /// <summary>
    /// ConvertBack is not implemented because this converter is one-way.
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("ConvertBack is not supported.");
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
        if (value != null && Enum.TryParse(value.ToString(), out BO.RoleType role))
        {
            return role == BO.RoleType.manager;
        }

        return false;
    }

    /// <summary>
    /// ConvertBack is not implemented because this converter is one-way.
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class DateTimeFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm");
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


public class TimeSpanToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TimeSpan timeSpan)
        {
            // פורמט שמציג ימים, שעות ודקות
            return $"{(int)timeSpan.TotalDays}D {timeSpan.Hours:D2}:{timeSpan.Minutes:D2}";
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string timeString)
        {
            try
            {
                // פיצול לפי פורמט הצפוי של "XD HH:MM"
                var parts = timeString.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 2 && parts[0].EndsWith("D"))
                {
                    // חלוקת היום והשעה
                    var daysPart = parts[0].TrimEnd('D');
                    var timePart = parts[1];

                    if (int.TryParse(daysPart, out int days) && TimeSpan.TryParse(timePart, out TimeSpan timeOfDay))
                    {
                        return new TimeSpan(days, timeOfDay.Hours, timeOfDay.Minutes, 0);
                    }
                }
            }
            catch
            {
                // טיפול במקרה של קלט לא תקין
            }
        }
        return TimeSpan.Zero;
    }
}


public class OpeningTimeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // בדיקה אם הערך הוא null או זמן ברירת מחדל
        if (value == null || (DateTime)value == default)
        {
            // מחזיר את השעה הנוכחית (שעון המערכת)
            return DateTime.Now;
        }

        // אם לא, מחזיר את הערך הקיים
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // במקרה הזה, אין צורך להחזיר חזרה את הערך (כי זה רק תצוגה)
        return value;
    }
}


public class ActiveCheckboxConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // value הוא CurrentCall
        // אם CurrentCall הוא null, מחזירים true (מאופשר), אחרת מחזירים false (לא מאופשר)
        return value == null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException(); // אין צורך במימוש ConvertBack
    }
}

public class DoubleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double doubleValue)
        {
            // פורמט התוצאה עם ארבע ספרות אחרי הנקודה
            return doubleValue.ToString("F4", CultureInfo.InvariantCulture);
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string stringValue && double.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
        {
            return result;
        }

        return value;
    }
}