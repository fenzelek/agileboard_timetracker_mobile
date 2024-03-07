using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Prism.Mvvm;
using TimeTrackerXamarin._UseCases.Contracts;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace TimeTrackerXamarin.ViewModels
{
    public partial class DebugPanelViewModel : ObservableObject
    {
        private readonly IDebugTools debugTools;

        [ObservableProperty]
        private List<DatabaseDump.Table> tables;

        [ObservableProperty]
        private List<string> tableNames;

        [ObservableProperty]
        private int selectedIndex = -1;

        [ObservableProperty]
        private List<Dictionary<string, string>> selectedTable;
        
        public DebugPanelViewModel(IDebugTools debugTools)
        {
            this.debugTools = debugTools;
            Dump();
        }

        async Task Dump()
        {
            Tables = debugTools.DumpDatabase().Tables;
            TableNames = Tables.Select(t => t.Name).ToList();
        }

        partial void OnSelectedIndexChanged(int index)
        {
            if (index == -1)
            {
                SelectedTable = null;
                return;
            }

            SelectedTable = Tables[index].Data;
        }
    }
}