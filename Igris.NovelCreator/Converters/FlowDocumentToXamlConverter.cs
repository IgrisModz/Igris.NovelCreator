using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Igris.NovelCreator.Converters
{
    [ValueConversion(typeof(string), typeof(FlowDocument))]
    public class FlowDocumentToXamlConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts from XAML markup to a WPF FlowDocument.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            /* See http://stackoverflow.com/questions/897505/getting-a-flowdocument-from-a-xaml-template-file */

            FlowDocument flowDocument = new();
            if (value != null)
            {
                string xamlText = value as string;
                flowDocument = XamlReader.Parse(xamlText) as FlowDocument;
            }

            // Set return value
            return flowDocument;
        }

        /// <summary>
        /// Converts from a WPF FlowDocument to a XAML markup string.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            /* This converter does not insert returns or indentation into the XAML. If you need to 
             * indent the XAML in a text box, see http://www.knowdotnet.com/articles/indentxml.html */

            // Exit if FlowDocument is null
            if (value == null)
            {
                return string.Empty;
            }

            // Get flow document from value passed in
            FlowDocument flowDocument = value as FlowDocument;

            // Convert to XAML and return
            return XamlWriter.Save(flowDocument);
        }

        #endregion
    }
}