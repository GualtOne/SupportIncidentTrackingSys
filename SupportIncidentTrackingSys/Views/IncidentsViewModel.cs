using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Threading;
using SupportIncidentTrackingSys.DBservice;
using SupportIncidentTrackingSys.Interfaces;
using SupportIncidentTrackingSys.Models;
using SupportIncidentTrackingSys.Role;

namespace SupportIncidentTrackingSys.Views
{
    public class IncidentsViewModel : INotifyPropertyChanged, IIncident
    {
        private ObservableCollection<Incident>? _incidents = [];

        private ObservableCollection<Status>? _status = [];
        public ObservableCollection<Incident> Incidents
        {
            get => _incidents ??= [];
            set
            {
                _incidents = value;
                OnPropertyChanged();
                InitIncidentsView();
            }
        }

        public ObservableCollection<Status> Statuses
        {
            get => _status ??= [];
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        private ICollectionView? _incidentsView;
        public ICollectionView? IncidentsView
        {
            get => _incidentsView;
            set { _incidentsView = value; OnPropertyChanged(); }
        }

        private string? _searchText = string.Empty;
        public string? SearchText
        {
            get => _searchText ??= string.Empty;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    RefreshFilter();
                }
            }
        }


        private Status? _selectedFilterStatus;
        public Status? SelectedFilterStatus
        {
            get => _selectedFilterStatus;
            set
            {
                _selectedFilterStatus = value;
                OnPropertyChanged();
                RefreshFilter();
            }
        }

        private void InitIncidentsView()
        {
            if (Incidents != null)
            {
                IncidentsView = CollectionViewSource.GetDefaultView(Incidents);
                RefreshFilter();
            }
        }

        public void RefreshFilter()
        {
            if (IncidentsView == null) return;

            IncidentsView.Filter = obj =>
            {
                if (obj is not Incident inc) return false;

                bool matchesSearch = string.IsNullOrWhiteSpace(SearchText) ||
                    inc.Id.ToString().Contains(SearchText) ||
                    (inc.Author?.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (inc.Description?.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (inc.Subdivision?.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0);

                bool matchesStatus = SelectedFilterStatus == null ||
                                     inc.Status == SelectedFilterStatus.StatusName;

                return matchesSearch && matchesStatus;
            };

            IncidentsView.Refresh();
        }




        private Incident? _selectedIncident;
        public Incident SelectedIncident
        {
            get => _selectedIncident ?? new Incident();
            set
            { 
                _selectedIncident = value;
                OnPropertyChanged();
                _ = LoadAttachmentsForCurrentIncidentAsync();
                _ = LoadCommentHistoryAsync();
            }
        }

        private async Task LoadAttachmentsForCurrentIncidentAsync()
        {
            if (SelectedIncident == null)
            {
                CurrentIncidentAttachments.Clear();
                return;
            }
            var data = await Task.Run(() => DBService.GetAttachmentsByIncidentId(SelectedIncident.Id).ToList());
            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                CurrentIncidentAttachments.Clear();
                foreach (var att in data) CurrentIncidentAttachments.Add(att);
            });
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
            get => _selectedCommentsHistory ?? new CommentsHistory();
            set 
            { _selectedCommentsHistory = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Attachment> _currentIncidentAttachments = [];
        public ObservableCollection<Attachment> CurrentIncidentAttachments
        {
            get => _currentIncidentAttachments;
            set { _currentIncidentAttachments = value; OnPropertyChanged(); }
        }

        private bool _canRead;
        public bool CanRead { get => _canRead; set { _canRead = value; OnPropertyChanged(); } }

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
            RefreshPermissions();
            InitIncidentsView();
        }

        public async Task LoadIncidentsAsync()
        {
            var data = await Task.Run(() => DBService.GetAllFrom<Incident>(DBService.tablename).ToList());
            var st = await Task.Run(() => DBService.GetAllFrom<Status>(DBService.tablenameS).ToList());
            Incidents.Clear();
            Statuses.Clear();
            foreach (var inc in data) Incidents.Add(inc);
            foreach (var s in st) Statuses.Add(s);
        }

        public async Task LoadCommentHistoryAsync()
        {
            if (SelectedIncident == null) 
            {
                CommentsHistories.Clear();
                return; 
            }
            var data = await Task.Run(() => DBService.GetCommentsByIncidentId(SelectedIncident.Id));
            CommentsHistories.Clear();
            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                foreach (var ch in data) CommentsHistories.Add(ch);
            });
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
            int newid = DBService.AddCommentHistory(Addcomment);
            Addcomment.Id = newid;
            CommentsHistories.Add(Addcomment);
        }

        public void UpdateIncident(Incident incident)
        {
            if (incident == null) return;
            DBService.Update(incident);
            Incident? existing = Incidents.FirstOrDefault(predicate: t => t.Id == incident.Id);
            if (existing != null)
            {
                int index = Incidents.IndexOf(existing);
                Incidents[index] = incident;
            }
            CommentsHistory Editcomment = new()
            {
                IncidentId = incident.Id,
                Comment = "Запись изменена",
                ActionType = CommentsHistory.ActionTypes.Изменен.ToString(),
                Timestamp = DateTime.Now
            };
            AddComment(Editcomment);
        }

        public void DeleteIncident(Incident incident)
        {
            if (incident == null) return;
            DBService.Delete(incident.Id);
            Incidents.Remove(incident);

            if (SelectedIncident?.Id == incident.Id)
            {
                CommentsHistories.Clear();
                DBService.DeleteCommentHistory(incident.Id);
                SelectedIncident = new Incident
                {
                    Id = -1
                };
            }
            else
            {
                var toRemove = CommentsHistories.Where(h => h.IncidentId == incident.Id).ToList();
                foreach (var item in toRemove)
                    CommentsHistories.Remove(item);
            }
        }

        public void DeleteCommentHistory(CommentsHistory comment)
        {
            if (comment == null) return;
            DBService.DeleteCommentHistoryA(comment.Id);
            CommentsHistories.Remove(comment);
        }

        public void EditCommentHistory(CommentsHistory comment)
        {
            if (comment == null) return;
            DBService.UpdateCommentHistory(comment);
            CommentsHistory? existing = CommentsHistories.FirstOrDefault(predicate: t => t.Id == comment.Id);
            if (existing != null)
            {
                int index = CommentsHistories.IndexOf(existing);
                CommentsHistories[index] = comment;
            }
        }

        public void ChangeStatus(Incident incident)
        {
            if (incident == null) return;
            string a = "Изменен статус";
            if (incident.DecisionDeadline.HasValue && incident.Status == "закрыт")
            {
                a = "Закрыт";
            }
            else
            {
                if (incident.DecisionDeadline.HasValue && incident.Status == "решен")
                {
                    a = "Решен";
                }
                else if (incident.Status == "отклонен")
                {
                    a = "Отклонен";
                }
            }
            DBService.ChangeStatus(incident);
            if (incident.ResponseTime.HasValue)
            {
                DBService.UpdateResposnseTime(incident);
            }
            if (incident.DecisionDeadline.HasValue) 
            {
                DBService.UpdateDisionDeadline(incident);
            }
            Incident? existing = Incidents.FirstOrDefault(predicate: t => t.Id == incident.Id);
            if (existing != null)
            {
                int index = Incidents.IndexOf(existing);
                Incidents[index] = incident;
            }
            CommentsHistory Editcomment = new()
            {
                IncidentId = incident.Id,
                Comment = a,
                ActionType = CommentsHistory.ActionTypes.Изменен.ToString(),
                Timestamp = DateTime.Now
            };
            AddComment(Editcomment);
        }

        public void CheckOverdue()
        {
            foreach (var inc in Incidents)
                inc.OnPropertyChanged(nameof(inc.IsOverdue));
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
            CanRead = AuthService.HasPermission(Permission.Read);
        }

        public void ChangeResponsible(Incident incident)
        {
            if (incident == null) return;
            DBService.ChangeResponsible(incident);
            Incident? existing = Incidents.FirstOrDefault(predicate: t => t.Id == incident.Id);
            if (existing != null)
            {
                int index = Incidents.IndexOf(existing);
                Incidents[index] = incident;
            }
            CommentsHistory Editcomment = new()
            {
                IncidentId = incident.Id,
                Comment = "Изменен отвественный",
                ActionType = CommentsHistory.ActionTypes.ДобавленОтвественный.ToString(),
                Timestamp = DateTime.Now
            };
            AddComment(Editcomment);
        }

        public async Task AddAttachmentAsync(int incidentId, string sourceFilePath)
        {
            if (!File.Exists(sourceFilePath)) return;
            string baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Supportsys");
            string attachmentsDir = Path.Combine(baseDir, "Attachments");
            Directory.CreateDirectory(attachmentsDir);
            string uniqueName = $"{incidentId}_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}{Path.GetExtension(sourceFilePath)}";
            string destPath = Path.Combine(attachmentsDir, uniqueName);
            await Task.Run(() => File.Copy(sourceFilePath, destPath));
            var attachment = new Attachment
            {
                IncidentId = incidentId,
                FileName = Path.GetFileName(sourceFilePath),
                FilePath = destPath,
                UploadedAt = DateTime.Now,
                UploadedBy = AuthService.CurrentUser?.Id
            };
            int newId = await Task.Run(() => DBService.AddAttachment(attachment));
            attachment.Id = newId;
            CurrentIncidentAttachments.Add(attachment);
            CommentsHistory Editcomment = new()
            {
                IncidentId = incidentId,
                Comment = "Прикреплён файл",
                ActionType = CommentsHistory.ActionTypes.ДобавленФайл.ToString(),
                Timestamp = DateTime.Now
            };
            AddComment(Editcomment);
        }

        public async Task DeleteAttachmentAsync(Attachment attachment)
        {
            if (attachment == null) return;
            await Task.Run(() =>
            {
                if (File.Exists(attachment.FilePath))
                    File.Delete(attachment.FilePath);
                DBService.DeleteAttachment(attachment.Id);
            });
            CurrentIncidentAttachments.Remove(attachment);
            CommentsHistory Editcomment = new()
            {
                IncidentId = attachment.IncidentId,
                Comment = "Удален файл",
                ActionType = CommentsHistory.ActionTypes.УдаленФайл.ToString(),
                Timestamp = DateTime.Now
            };
            AddComment(Editcomment);
        }
    }
}
