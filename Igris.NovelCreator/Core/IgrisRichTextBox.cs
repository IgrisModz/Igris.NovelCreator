using Igris.Mvvm;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Igris.NovelCreator.Core
{
    public class IgrisRichTextBox : RichTextBox
    {

        public static readonly DependencyProperty CustomTextProperty = DependencyProperty.Register("CustomText", typeof(string), typeof(IgrisRichTextBox),
       new PropertyMetadata(string.Empty, CustomTextChangedCallback), CustomTextValidateCallback);

        private static void CustomTextChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as IgrisRichTextBox).Document = GetCustomDocument(e.NewValue as string);
        }

        private static bool CustomTextValidateCallback(object value)
        {
            return value != null;
        }

        public string CustomText
        {
            get => (string)GetValue(CustomTextProperty);
            set => SetValue(CustomTextProperty, value);
        }

        private static FlowDocument GetCustomDocument(string Text)
        {
            FlowDocument document = new();
            Paragraph para = new();
            para.Margin = new Thickness(1); // remove indent between paragraphs
            foreach (string word in Text.Split(new string[] { " " }, StringSplitOptions.None))
            {
                bool asDot = word.Contains("#", StringComparison.CurrentCulture);
                if (asDot || word.Contains("|", StringComparison.CurrentCulture))
                {
                    SolidColorBrush brush = asDot ? Brushes.Red : Brushes.HotPink;

                    string[] tags = word.Split(new string[] { "#", "|" }, StringSplitOptions.None);

                    bool start = word.StartsWith("#", StringComparison.CurrentCulture) || word.StartsWith("|", StringComparison.CurrentCulture);
                    string linkName = tags[1];

                    if (!start)
                    {
                        para.Inlines.Add(tags[0]);
                    }

                    Hyperlink link = new();
                    link.IsEnabled = true;
                    link.TextDecorations = null;
                    link.Foreground = brush;
                    link.CommandParameter = linkName;
                    link.Command = new DelegateCommand<string>(n => MyFandom(n));
                    link.Inlines.Add(linkName);
                    para.Inlines.Add(link);

                    if (tags.Length is 2 or 3)
                    {
                        para.Inlines.Add(tags[^1]);
                    }
                }
                else
                {
                    para.Inlines.Add(word);
                }
                para.Inlines.Add(" ");
            }
            document.Blocks.Add(para);
            return document;
        }

        private static void MyFandom(string fandom)
        {
            MessageBox.Show("Succès");
        }
    }
}
