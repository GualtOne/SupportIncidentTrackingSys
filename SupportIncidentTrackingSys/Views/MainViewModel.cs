using System.ComponentModel;
using System.Runtime.CompilerServices;
using SupportIncidentTrackingSys.Interfaces;
using SupportIncidentTrackingSys.Views;

namespace SupportIncidentTrackingSys.Views
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public IIncident IncidentsVM { get; }
        //public IStatisticsViewModel StatisticsVM { get; }

        public MainViewModel()
        {
            IncidentsVM = new IncidentsViewModel();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}