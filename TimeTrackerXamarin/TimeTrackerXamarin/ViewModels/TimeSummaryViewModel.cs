using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using TimeTrackerXamarin._UseCases.TimeTracking;
using TimeTrackerXamarin.i18n;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TimeTrackerXamarin.ViewModels
{
    public partial class TimeSummaryViewModel : ObservableObject
    {
        [ObservableProperty] private bool isBusy;

        [ObservableProperty] private KeyValuePair<int, string> currentMonth;

        [ObservableProperty] private int currentYear;

        [ObservableProperty] private List<TrackHistory> history;

        [ObservableProperty] private string totalSum;

        private readonly GetTimeSummary getTimeSummary;
        private readonly IErrorHandler errorHandler;
        public List<KeyValuePair<int, string>> Months { get; set; } = new List<KeyValuePair<int, string>>();
        public List<int> Years { get; set; } = new List<int>();

        public TimeSummaryViewModel(GetTimeSummary getTimeSummary, IErrorHandler errorHandler,
            ITranslationManager translation)
        {
            this.getTimeSummary = getTimeSummary;
            this.errorHandler = errorHandler;
            for (var i = 1; i <= 12; i++)
            {
                Months.Add(new KeyValuePair<int, string>(
                    i,
                    translation.Language.Culture.DateTimeFormat.MonthNames[i - 1]
                ));
            }


            DateTime dateNow = DateTime.Now;
            for (int i = 0; i < 3; i++)
            {
                Years.Add(dateNow.Year - i);
            }

            CurrentYear = dateNow.Year;
            
            CurrentMonth = Months.Find(x => x.Key == dateNow.Month);

            var firstDayOfMonth = new DateTime(dateNow.Year, dateNow.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            getList(firstDayOfMonth.ToString("yyyy-MM-dd"), lastDayOfMonth.ToString("yyyy-MM-dd"));
        }

        [RelayCommand]
        private void GetSpecific(object obj)
        {
            var firstDayOfMonth = new DateTime(CurrentYear, CurrentMonth.Key, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            getList(firstDayOfMonth.ToString("yyyy-MM-dd"), lastDayOfMonth.ToString("yyyy-MM-dd"));
        }

        private async void getList(string from, string to)
        {
            IsBusy = true;

            try
            {
                var connection = Connectivity.NetworkAccess == NetworkAccess.Internet;
                getTimeSummary.SetConnection(connection);
                History = await getTimeSummary.GettTrackHistory(
                    int.Parse(Preferences.Get("current_company_id", string.Empty)), from, to);
                TotalSum = "0h 0m";
                long ticks = 0;
                if (History?.Count > 0)
                {
                    foreach (var history in History)
                    {
                        ticks += history.tracked;
                    }

                    var span = TimeSpan.FromSeconds(ticks);
                    TotalSum = $"{Math.Truncate(span.TotalHours)}h {span.Minutes}m";
                }
            }
            catch (Exception e)
            {
                errorHandler.Handle(e);
            }

            IsBusy = false;
        }
    }
}