using System.Globalization;
using System.Windows.Data;
namespace PL;

public class ConvertObjIdToTF : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        
        if (value.ToString() == "Update")
        {
            return true;
        }
        return false;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class ConvertObjPasswordToTF : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {

        if (value.ToString() == "Update")
        {
            return false;
        }
        return true;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


public class ConvertRoleToTF : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null && Enum.TryParse(value.ToString(), out BO.RoleType role))
        {
            // Directly return the result of the comparison
            return role == BO.RoleType.manager;
        }

        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
