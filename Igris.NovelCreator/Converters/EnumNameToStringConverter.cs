using Igris.NovelCreator.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Igris.NovelCreator.Converters
{
    public class EnumNameToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<string> res = new();
            Genres[] result = (Genres[])value;
            foreach (Genres genres in result)
            {
                res.Add(Enum.GetName(genres).Replace("_", " "));
            }
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
