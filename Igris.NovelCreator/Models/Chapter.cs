using Igris.Mvvm;
using System;

namespace Igris.NovelCreator.Models
{
    public class Chapter : ViewModelBase
    {
        public float Id { get => GetProperty(() => Id); set => SetProperty(() => Id, value); }
        public string Title { get => GetProperty(() => Title); set => SetProperty(() => Title, value); }
        public string Text { get => GetProperty(() => Text); set => SetProperty(() => Text, value); }
        public string AuthorDescription { get => GetProperty(() => AuthorDescription); set => SetProperty(() => AuthorDescription, value); }
        public DateTime CreationDate { get => GetProperty(() => CreationDate); set => SetProperty(() => CreationDate, value); }
        public ChapterType Type { get => GetProperty(() => Type); set => SetProperty(() => Type, value); }
    }
}
