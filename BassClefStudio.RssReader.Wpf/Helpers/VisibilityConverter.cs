using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace BassClefStudio.RssReader.Wpf.Helpers
{
    public class VisibilityConverter : IValueConverter
    {
        public object CheckFor { get; set; }

        public bool IsEqual { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b1 && bool.TryParse(CheckFor.ToString(), out var b2))
            {
                if ((b1 == b2) == IsEqual)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            else
            {
                if (value != null && (value.Equals(CheckFor)) == IsEqual)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
