using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Prism.Mvvm;
using TimeTrackerXamarin._UseCases.Contracts;

namespace TimeTrackerXamarin.ViewModels
{
    public partial class LogsPageViewModel : ObservableObject
    {

        private readonly ILogStorage storage;

        [ObservableProperty]
        private List<DateTime> logDates = new List<DateTime> { DateTime.Now };

        [ObservableProperty]
        private List<Log> logs;

        [ObservableProperty]
        private DateTime selectedDate;

        public LogsPageViewModel(ILogStorage storage)
        {
            this.storage = storage;
            LogDates = storage.GetSavedLogDates();
            SelectedDate = DateTime.Now;
        }

        partial void OnSelectedDateChanged(DateTime date)
        {
            Logs = storage.GetLogs(date);
        }
    }
}