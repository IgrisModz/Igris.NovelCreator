using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Igris.NovelCreator.Core
{
    public class RichTextBoxHelper : DependencyObject
    {
        private static readonly HashSet<Thread> _recursionProtection = new();

        public static string GetDocumentXaml(DependencyObject obj)
        {
            return (string)obj.GetValue(DocumentXamlProperty);
        }

        public static void SetDocumentXaml(DependencyObject obj, string value)
        {
            _recursionProtection.Add(Thread.CurrentThread);
            obj.SetValue(DocumentXamlProperty, value);
            _recursionProtection.Remove(Thread.CurrentThread);
        }

        public static readonly DependencyProperty DocumentXamlProperty = DependencyProperty.RegisterAttached(
            "DocumentXaml",
            typeof(string),
            typeof(RichTextBoxHelper),
            new FrameworkPropertyMetadata(
                "",
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                (obj, e) =>
                {
                    if (_recursionProtection.Contains(Thread.CurrentThread))
                    {
                        return;
                    }

                    RichTextBox richTextBox = (RichTextBox)obj;
                    try
                    {
                        //MemoryStream stream = new(Encoding.UTF8.GetBytes(GetDocumentXaml(richTextBox)));
                        //richTextBox.Document = (FlowDocument)XamlReader.Load(stream);

                        richTextBox.Document = GetCustomDocument(GetDocumentXaml(richTextBox));
                    }
                    catch (Exception)
                    {
                        richTextBox.Document = new FlowDocument();
                    }

                    richTextBox.TextChanged += (obj2, e2) =>
                    {
                        if (obj2 is RichTextBox richTextBox2)
                        {
                            SetDocumentXaml(richTextBox, XamlWriter.Save(richTextBox2.Document));
                        }
                    };
                }
            )
        );

        private static FlowDocument GetCustomDocument(string Text)
        {
            string text = "";
            string[] words = Text.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                //This condition could be replaced by the Regex
                if (words[i].StartsWith("#", StringComparison.CurrentCulture))
                {
                    string linkName = words[i][1..].Replace("</Run></Paragraph></FlowDocument>", "");
                    text += $"</Run><Hyperlink IsEnabled=\"True\" Tag=\"{linkName}\" TextDecorations=\"None\" CommandParameter=\"{{Binding Tag, RelativeSource ={{RelativeSource Self}}}}\" Command=\"{{Binding FandomCommand}}\">{linkName}</Hyperlink>";
                }
                else
                {

                    text += words[i];
                }
                if (i != words.Length - 1)
                {
                    text += " ";
                }
                if (i == words.Length - 1)
                {
                    if (text.EndsWith("</Paragraph>", StringComparison.CurrentCulture))
                    {
                        text += "</FlowDocument>";
                    }
                    else if (!text.EndsWith("</Paragraph></FlowDocument>", StringComparison.CurrentCulture))
                    {
                        text += "</Paragraph></FlowDocument>";
                    }
                }
            }
            text = text.Replace("</Hyperlink> </Run>", "</Hyperlink> ").Replace("</Hyperlink></Run>", "</Hyperlink>");
            FlowDocument document = XamlReader.Parse(text) as FlowDocument;
            return document;
        }
    }
}
