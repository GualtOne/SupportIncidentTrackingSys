using System.Collections.ObjectModel;
using System.ComponentModel;
using SupportIncidentTrackingSys.Models;

namespace SupportIncidentTrackingSys.Interfaces
{
    public interface IIncident
    {
        ObservableCollection<Incident> Incidents { get; }
        ObservableCollection<Attachment> CurrentIncidentAttachments { get; }
        Incident SelectedIncident { get; set; }

        void AddIncident(Incident incident);
        void UpdateIncident(Incident incident);
        void DeleteIncident(Incident incident);
        void ChangeStatus(Incident incident);
        void ChangeResponsible(Incident incident);
        void DeleteCommentHistory(CommentsHistory commentsHistory);

        ICollectionView? IncidentsView { get; }
        string? SearchText { get; set; }
        Status? SelectedFilterStatus { get; set; }
        void RefreshFilter();
        void CheckOverdue();
        Task DeleteAttachmentAsync(Attachment attachmentd);
        Task AddAttachmentAsync(int incidentId, string sourceFilePath);
        void EditCommentHistory(CommentsHistory comment);
    }
}