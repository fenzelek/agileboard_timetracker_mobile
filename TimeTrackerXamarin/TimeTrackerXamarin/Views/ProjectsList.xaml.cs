using System;
using TimeTrackerXamarin.i18n;
using TimeTrackerXamarin.ViewModels;
using Xamarin.Forms;

namespace TimeTrackerXamarin.Views
{
    public partial class ProjectsList : ContentPage
    {
        private readonly ITranslationManager translationManager;

        public ProjectsList(ITranslationManager translationManager)
        {
            this.translationManager = translationManager;
            InitializeComponent();
        }
        public static Action EmulateBackPressed;

        private bool AcceptBack;

        protected override bool OnBackButtonPressed()
        {
            if (AcceptBack)
                return false;

            PromptForExit();
            return true;
        }

        private async void PromptForExit()
        {
            var message = translationManager.Translate("views.projectslist.exit-confirm.message");
            var yes = translationManager.Translate("views.projectslist.exit-confirm.yes");
            var no = translationManager.Translate("views.projectslist.exit-confirm.no");
            if (await DisplayAlert("", message, yes, no))
            {
                AcceptBack = true;
                EmulateBackPressed();
            }
        }

        protected override void OnAppearing()
        {
            var x = ((ProjectsListViewModel)BindingContext);
            if (x == null) return;
            x.OnAppearing();
        }
    }
}
