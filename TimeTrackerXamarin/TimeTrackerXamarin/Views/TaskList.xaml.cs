using System;
using System.Collections.Generic;
using System.Linq;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin.i18n;
using TimeTrackerXamarin.ViewModels;
using Xamarin.Forms;

namespace TimeTrackerXamarin.Views
{
    public partial class TaskList : ContentPage
    {
        private readonly ITranslationManager translationManager;

        public TaskList(ITranslationManager translationManager)
        {
            this.translationManager = translationManager;
            InitializeComponent();
        }
        private void ToggleButton(object sender, EventArgs e)
        {
            PlayBtn.IsVisible = PlayBtn.IsVisible ? false : true;
            StopBtn.IsVisible = StopBtn.IsVisible ? false : true;
        }

        private void UserClicked(object sender, EventArgs e)
        {
            UserPicker.Focus();
        }

        private void UserPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            User user = (User)picker?.SelectedItem;
            if (user != null)
            {
                clearPickerBtn.IsVisible = true;
                UserBtn.Text = user.first_name;
                UserBtn.FontAttributes = FontAttributes.Bold;
                
            }
            else
            {
                UserBtn.Text = translationManager.Translate("views.tasklist.assigned");
                UserBtn.FontAttributes = FontAttributes.None;
            }
            
        }
        private void ClearPicker(object sender,EventArgs e)
        {
            var btn = (Button)sender;
            btn.IsVisible = false;
            UserPicker.SelectedItem = null;
        }
    }
}
