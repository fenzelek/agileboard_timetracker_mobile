using System;
using System.ComponentModel;
using Prism.Ioc;

namespace TimeTrackerXamarin.i18n
{
    public class TextBindings : INotifyPropertyChanged
    {
        
        private static readonly Lazy<ITranslationManager> TranslationManagerLazy =
            new Lazy<ITranslationManager>(() => ContainerLocator.Container.Resolve<ITranslationManager>());

        public string this[string text]
        {
            get => ContainerLocator.Container.Resolve<ITranslationManager>().Translate(text.Replace(":", "."));
        }

        public static readonly TextBindings Instance = new TextBindings();

        public event PropertyChangedEventHandler PropertyChanged;

        public void Invalidate()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}