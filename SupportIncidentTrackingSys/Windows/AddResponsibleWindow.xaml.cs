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
    /// Логика взаимодействия для AddResponsibleWindow.xaml
    /// </summary>
    public partial class AddResponsibleWindow : Window
    {
        private readonly Incident _originalIncident;

        public Incident? ResultIncident { get; private set; }

        public ObservableCollection<User> Users { get; private set; } = [];

        private User? _selectedUser;
        public User? SelectedUser
        {
            get => _selectedUser;
            set { _selectedUser = value; OnPropertyChanged(); }
        }
        public async Task LoadDataAsync()
        {
            var users = await Task.Run(() => DBService.GetAllFrom<User>(DBService.tablenameUS).ToList() ?? []);

            Application.Current.Dispatcher.Invoke(() =>
            {
                Users.Clear();
                foreach (var u in users)
                    if(u.IsActive.HasValue && u.IsActive == true)
                        Users.Add(u);
            });
            IsDataLoaded = true;
        }

        private bool _isDataLoaded;
        public bool IsDataLoaded
        {
            get => _isDataLoaded;
            set { _isDataLoaded = value; OnPropertyChanged(); }
        }

        public AddResponsibleWindow(Incident Incident)
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
            SelectedUser = Users.FirstOrDefault(s => s.FullName == _originalIncident.Responsible);

        }

        public string? resualts;

        private (bool IsValid, string Message) ValidateSelection()
        {
            var missing = new List<string>();
            if (SelectedUser == null) missing.Add("отвественого");

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
                DecisionDeadline = _originalIncident.DecisionDeadline,
                Description = _originalIncident.Description,
                RegistrationDate = _originalIncident.RegistrationDate,
                CreatedById = _originalIncident.CreatedById,
                Priority = _originalIncident.Priority,
                ResponseTime = _originalIncident.ResponseTime,
                Responsible = SelectedUser!.FullName,
                ResponsibleId = SelectedUser!.Id,
                Subdivision = _originalIncident.Subdivision,
                Status = _originalIncident.Status,
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
