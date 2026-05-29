using System.ComponentModel;
using System.Runtime.CompilerServices;
using SupportIncidentTrackingSys.Interfaces;

namespace SupportIncidentTrackingSys.Views
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public IIncident IncidentsVM { get; }

        public MainViewModel()
        {
            IncidentsVM = new IncidentsViewModel();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}