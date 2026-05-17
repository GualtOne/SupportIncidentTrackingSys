using System.Collections.ObjectModel;
using SupportIncidentTrackingSys.Models;

namespace SupportIncidentTrackingSys.Interfaces
{
    public interface IIncident
    {
        ObservableCollection<Incident> Incidents { get; }
        Incident SelectedIncident { get; set; }

        void LoadIncidents();
        void AddIncident(Incident incident);
        void UpdateIncident(Incident incident);
        void DeleteIncident(Incident incident);
        int GetHashCode();
        string ToString();
    }
}