using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Prism.Mvvm;
using SQLite;
using TimeTrackerXamarin._UseCases.Contracts;

namespace TimeTrackerXamarin.ViewModels
{
    public class BindableTicket : Ticket, INotifyPropertyChanged
    {

        private bool currentlyTracking = false;
        private bool selected = false;

        [Ignore]
        public bool CurrentlyTracking
        {
            get => currentlyTracking;
            set => SetField(ref currentlyTracking, value);
        }

        [Ignore]
        public bool Selected
        {
            get => selected;
            set => SetField(ref selected, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}