using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SupportIncidentTrackingSys.DBservice;
using SupportIncidentTrackingSys.Interfaces;
using SupportIncidentTrackingSys.Models;
using SupportIncidentTrackingSys.Role;

namespace SupportIncidentTrackingSys.Views
{
    public class IncidentsViewModel : INotifyPropertyChanged, IIncident
    {
        private ObservableCollection<Incident>? _incidents;
        public ObservableCollection<Incident> Incidents
        {
            get => _incidents ??= [];
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
            { _selectedIncident = value; OnPropertyChanged(); }
        }

        private ObservableCollection<CommentsHistory>? _commentsHistory;
        public ObservableCollection<CommentsHistory> CommentsHistories
        {
            get => _commentsHistory ??= [];
            set { _commentsHistory = value; OnPropertyChanged(); }
        }

        private CommentsHistory? _selectedCommentsHistory;
        public CommentsHistory SelectedCommentsHistory
        {
            get => (_selectedCommentsHistory ?? SelectedCommentsHistory);
            set { _selectedCommentsHistory = value; OnPropertyChanged(); }
        }

        private bool _canEdit;
        public bool CanEdit { get => _canEdit; set { _canEdit = value; OnPropertyChanged(); } }

        private bool _canDelete;
        public bool CanDelete { get => _canDelete; set { _canDelete = value; OnPropertyChanged(); } }

        private bool _canAdmin;
        public bool CanAdmin { get => _canAdmin; set { _canAdmin = value; OnPropertyChanged(); } }

        private bool _canFullAccess;
        public bool CanFullAccess { get => _canFullAccess; set { _canFullAccess = value; OnPropertyChanged(); } }

        public IncidentsViewModel()
        {
            DBService.Connection();
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            await LoadIncidentsAsync();
            await LoadCommentHistoryAsync();
            RefreshPermissions();
        }

        public async Task LoadIncidentsAsync()
        {
            var data = await Task.Run(() => DBService.GetAllFrom<Incident>(DBService.tablename).ToList());
            Incidents.Clear();
            foreach (var inc in data) Incidents.Add(inc);
        }

        public async Task LoadCommentHistoryAsync()
        {
            var data = await Task.Run(() => DBService.GetAllFrom<CommentsHistory>(DBService.tablenameCH).ToList());
            CommentsHistories.Clear();
            foreach (var ch in data) CommentsHistories.Add(ch);
        }

        public void AddIncident(Incident incident)
        {
            if (incident == null) return;
            int newid = DBService.Add(incident);
            incident.Id = newid;
            Incidents.Add(incident);
            CommentsHistory Addcomment = new()
            {
                IncidentId = incident.Id,
                Comment = "Запись добавлена",
                ActionType = CommentsHistory.ActionTypes.Добавлен.ToString(),
                Timestamp = DateTime.Now
            };
            AddComment(Addcomment);
        }

        private void AddComment(CommentsHistory Addcomment)
        {
            DBService.AddCommentHistory(Addcomment);
            if (CommentsHistories.Count > 0)
                Addcomment.Id = CommentsHistories.Last().Id + 1;
            else
                Addcomment.Id = 1;
            CommentsHistories.Add(Addcomment);
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
            CommentsHistory Editcomment = new()
            {
                IncidentId = incident.Id,
                Comment = "Запись изменена",
                ActionType = CommentsHistory.ActionTypes.Изменена.ToString(),
                Timestamp = DateTime.Now
            };
            AddComment(Editcomment);
        }

        public void DeleteIncident(Incident incident)
        {
            if (incident == null) return;
            DBService.Delete(incident.Id);
            Incidents.Remove(incident);
            CommentsHistory comment = new()
            {
                IncidentId = incident.Id,
                Comment = "Запись удалена",
                ActionType = CommentsHistory.ActionTypes.Удален.ToString(),
                Timestamp = DateTime.Now
            };
            AddComment(comment);
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void RefreshPermissions()
        {
            CanEdit = AuthService.HasPermission(Permission.Edit);
            CanDelete = AuthService.HasPermission(Permission.Delete);
            CanAdmin = AuthService.HasPermission(Permission.Admin);
            CanFullAccess = AuthService.HasPermission(Permission.FullAccess);
        }
    }
}
