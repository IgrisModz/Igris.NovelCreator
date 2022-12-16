using Igris.Mvvm;
using Igris.NovelCreator.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Igris.NovelCreator.ViewModels
{
    public class ChapterReadingViewModel : ViewModelBase
    {
        public ChapterListViewModel ChapterList { get => GetProperty(() => ChapterList); set => SetProperty(() => ChapterList, value); }
        public ObservableCollection<Chapter> Chapters { get => GetProperty(() => Chapters); set => SetProperty(() => Chapters, value); }
        public Chapter Chapter { get => GetProperty(() => Chapter); set => SetProperty(() => Chapter, value); }
        public string Text { get => GetProperty(() => Text); set => SetProperty(() => Text, value); }

        public ICommand CloseCommand { get; }
        public ICommand PrevCommand { get; }
        public ICommand NextCommand { get; }

        public ChapterReadingViewModel(ChapterListViewModel vm, ObservableCollection<Chapter> chapters, Chapter chapter)
        {
            ChapterList = vm ?? throw new ArgumentNullException(nameof(vm));
            Chapters = chapters ?? throw new ArgumentNullException(nameof(chapters));
            Chapter = chapter ?? throw new ArgumentNullException(nameof(chapter));
            CloseCommand = new DelegateCommand(Close);
            PrevCommand = new DelegateCommand(Prev, AllowPrev);
            NextCommand = new DelegateCommand(Next, AllowNext);
            Text = Chapter.Text;
        }

        private ChapterReadingViewModel()
        {
        }

        private bool AllowPrev()
        {
            return Chapters.Any(c => c.Id < Chapter.Id);
        }

        private bool AllowNext()
        {
            return Chapters.Any(c => c.Id > Chapter.Id);
        }

        private void Close()
        {
            ChapterList.ContentControl = null;
            ChapterList.ContentIsOpen = false;
        }

        private void Prev()
        {
            Chapter = Chapters.OrderBy(c => c.Id).LastOrDefault(c => c.Id < Chapter.Id);
            Text = Chapter.Text;
            ChapterList.ContentControl.TextBox.UpdateDefaultStyle();
        }

        private void Next()
        {
            Chapter = Chapters.OrderBy(c => c.Id).FirstOrDefault(c => c.Id > Chapter.Id);
            Text = Chapter.Text;
            ChapterList.ContentControl.TextBox.UpdateDefaultStyle();
        }
    }
}
