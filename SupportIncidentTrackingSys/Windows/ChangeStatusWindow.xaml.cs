using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using SupportIncidentTrackingSys.DBservice;
using SupportIncidentTrackingSys.Models;
using SupportIncidentTrackingSys.Role;

namespace SupportIncidentTrackingSys.Windows
{
    /// <summary>
    /// Логика взаимодействия для ChangeStatus.xaml
    /// </summary>
    public partial class ChangeStatusWindow : Window
    {
        private readonly Incident _originalIncident;

        public Incident? ResultIncident { get; private set; }

        public ObservableCollection<Status> Statuses { get; private set; } = [];

        private Status? _selectedStatus;
        public Status? SelectedStatus
        {
            get => _selectedStatus;
            set { _selectedStatus = value; OnPropertyChanged(); }
        }

        private DateTime? _responseTime;
        public DateTime? ResponseTime
        {
            get => _responseTime;
            set { _responseTime = value; OnPropertyChanged(); }
        }

        private DateTime? _deadlineDate;
        public DateTime? DeadlineDate
        {
            get => _deadlineDate;
            set { _deadlineDate = value; OnPropertyChanged(); }
        }

        public async Task LoadDataAsync()
        {
            var statuses = await Task.Run(() => DBService.GetAllFrom<Status>(DBService.tablenameS).ToList() ?? []);

            Application.Current.Dispatcher.Invoke(() =>
            {
                Statuses.Clear();
                foreach (var s in statuses) Statuses.Add(s);
            });
            IsDataLoaded = true;
        }

        private bool _isDataLoaded;
        public bool IsDataLoaded
        {
            get => _isDataLoaded;
            set { _isDataLoaded = value; OnPropertyChanged(); }
        }

        public ChangeStatusWindow(Incident Incident)
        {
            InitializeComponent();
            DataContext = this;
            Loaded += OnLoaded;
            _originalIncident = Incident;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await LoadDataAsync();
                LoadIncidentData();
            }
            catch (Exception ex)
            {
                Messageb.ShowError(ex.Message);
            }
        }

        private void LoadIncidentData()
        {
            SelectedStatus = Statuses.FirstOrDefault(s => s.StatusName == _originalIncident.Status);
            DeadlineDate = _originalIncident.DecisionDeadline;
            ResponseTime = _originalIncident.ResponseTime;

        }

        public string? resualts;

        private (bool IsValid, string Message) ValidateSelection()
        {
            var missing = new List<string>();
            if (SelectedStatus == null) missing.Add("статус");

            if (missing.Count == 0) return (true, string.Empty);

            string message = "Выберите " + string.Join(" и ", missing);
            return (false, message);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            bool flowControl = ValidateAll();
            if (!flowControl)
            {
                return;
            }

            ResultIncident = new Incident
            {
                Id = _originalIncident.Id,
                Author = _originalIncident.Author,
                Category = _originalIncident.Category,
                DecisionDeadline = DeadlineDate,
                Description = _originalIncident.Description,
                RegistrationDate = _originalIncident.RegistrationDate,
                CreatedById = _originalIncident.CreatedById,
                Priority = _originalIncident.Priority,
                ResponseTime = ResponseTime,
                Responsible = _originalIncident.Responsible,
                ResponsibleId = _originalIncident.ResponsibleId,
                Subdivision = _originalIncident.Subdivision,
                Status = SelectedStatus!.StatusName,
            };

            DialogResult = true;
            Close();
        }

        private bool ValidateAll()
        {
            if (AuthService.CurrentUser == null)
            {
                Messageb.ShowError("Пользователь не авторизован");
                DialogResult = false;
                Close();
                return false;
            }

            var (isValid, message) = ValidateSelection();
            if (!isValid)
            {
                Messageb.ShowWarning(message);
                return false;
            }

            if (SelectedStatus!.StatusName == "закрыт" && _originalIncident.Status != "решен")
            {
                Messageb.ShowWarning("Нельзя закрыть без решения");
                return false;
            }

            if (SelectedStatus!.StatusName == "в работе" && _originalIncident.Status == "решен")
            {
                DeadlineDate = null;
            }


            if (SelectedStatus!.StatusName == "в работе" && _originalIncident.Status == "новый" && !_originalIncident.ResponseTime.HasValue)
            {
                ResponseTime = DateTime.Now;
            }

            if (SelectedStatus!.StatusName == "решен" && _originalIncident.ResponseTime.HasValue && !_originalIncident.DecisionDeadline.HasValue)
            {
                DeadlineDate = DateTime.Now;
            }
            else if(SelectedStatus!.StatusName == "решен" && !_originalIncident.ResponseTime.HasValue)
            {
                Messageb.ShowWarning("Нельзя сделать решеным без работы над ним");
                return false;
            }

            return true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}
