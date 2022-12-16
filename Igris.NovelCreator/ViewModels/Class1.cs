using Igris.Mvvm;
using System;
using System.Windows;
using System.Windows.Input;

namespace Igris.NovelCreator.ViewModels
{
    public class Class1 : ViewModelBase
    {
        private string testStr;

        public ICommand TestCommand { get; }
        public ICommand TestParamCommand { get; }

        public DateTime TT { get; }

        public string TestString { get => GetProperty(() => TestString); set => SetProperty(() => TestString, value); }
        public string TestStr { get => testStr; set => SetValue(ref testStr, value); }

        public Class1()
        {
            TestCommand = new DelegateCommand(Test, AllowTest);
            TestParamCommand = new DelegateCommand<string>(s => TestParam(s), s => AllowTestParam(s));
            TT = DateTime.Now;
        }

        private bool AllowTest()
        {
            return TestString == "Test" || TestStr == "Test";
        }

        private bool AllowTestParam(string text)
        {
            return !string.IsNullOrEmpty(text);
        }

        private void Test()
        {
            MessageBox.Show("Test réussi", "Test");
        }

        private void TestParam(string text)
        {
            MessageBox.Show(text, "Test");
        }
    }
}
