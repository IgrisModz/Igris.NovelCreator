using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Igris.NovelCreator.Converters
{
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Convert bool or Nullable bool to Visibility
        /// </summary>
        /// <param name="value">bool or Nullable bool</param>
        /// <param name="targetType">Visibility</param>
        /// <param name="parameter">null</param>
        /// <param name="culture">null</param>
        /// <returns>Visible or Collapsed</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bValue = false;
            if (value is bool boolean)
            {
                bValue = boolean;
            }
            else if (value is bool?)
            {
                bValue = (bool?)value ?? false;
            }
            return parameter.ToString() != "invert" ? bValue ? Visibility.Visible : Visibility.Collapsed : bValue ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Convert Visibility to boolean
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility visibility && visibility == (parameter.ToString() != "invert" ? Visibility.Visible : Visibility.Collapsed);
        }
    }
}
