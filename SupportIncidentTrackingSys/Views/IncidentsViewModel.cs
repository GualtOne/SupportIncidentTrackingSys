using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SupportIncidentTrackingSys.Interfaces;
using SupportIncidentTrackingSys.DBservice;
using SupportIncidentTrackingSys.Models;

namespace SupportIncidentTrackingSys.Views
{
    public class IncidentsViewModel : INotifyPropertyChanged, IIncident
    {
        private ObservableCollection<Incident>? _incidents;

        public ObservableCollection<Incident> Incidents
        {
            get => _incidents ?? Incidents;
            set
            {
                _incidents = value;
                OnPropertyChanged();
            }
        }

        private Incident? _selectedIncident;
        public Incident SelectedIncident
        {
            get => _selectedIncident ?? SelectedIncident;
            set
            {
                _selectedIncident = value;
                OnPropertyChanged();
            }
        }

        public IncidentsViewModel()
        {
            DBService.Connection();
            LoadIncidents();
        }

        public void AddIncident(Incident incident)
        {
            if (incident == null) return;
            DBService.Add(incident);
            if (Incidents.Count > 0)
            {
                incident.Id = Incidents.Last().Id + 1;
            }
            else
            {
                incident.Id = 1;
            }
            Incidents.Add(incident);
        }

        public void UpdateIncident(Incident incident)
        {
            if (incident == null) return;
            DBService.Update(incident);
            Incident? existing = Incidents.FirstOrDefault(predicate: t => t.Id == incident.Id);
            if (existing != null)
            {
                var index = Incidents.IndexOf(existing);
                Incidents[index] = incident;
            }
        }

        public void DeleteIncident(Incident incident)
        {
            if (incident == null) return;
            DBService.Delete(incident.Id);
            Incidents.Remove(incident);
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void LoadIncidents()
        {
            IEnumerable<Incident> data = DBService.GetAllFrom<Incident>(DBService.tablename);
            if (data != null)
            {
                Incidents = new ObservableCollection<Incident>(data);
            }    

        }

        void IIncident.LoadIncidents()
        {
            LoadIncidents();
        }

        int IIncident.GetHashCode()
        {
            return base.GetHashCode();
        }

        string IIncident.ToString()
        {
            return base.ToString() ?? nameof(IncidentsViewModel);
        }
    }
}
