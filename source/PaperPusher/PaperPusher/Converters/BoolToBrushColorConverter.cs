using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PaperPusher.Converters
{
    public class BoolToBrushColorConverter : IValueConverter
    {
        public Color TrueColor { get; set; } = Colors.Transparent;
        public Color FalseColor { get; set; } = Colors.Red;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return new SolidColorBrush(TrueColor);

            return new SolidColorBrush(FalseColor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
