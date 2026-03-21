using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace QMatrix.GUI;

public class DateTimeFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm");
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is string str && DateTime.TryParse(str, out var dateTime))
        {
            return dateTime;
        }
        return DateTime.MinValue;
    }
}
