using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.Win32;
using SupportIncidentTrackingSys.DBservice;
using SupportIncidentTrackingSys.Models;
using SupportIncidentTrackingSys.Role;

namespace SupportIncidentTrackingSys
{
    public partial class AddWindow : Window ,INotifyPropertyChanged
    {
        public Incident? ResultIncident { get; private set; }
        public ObservableCollection<Priority> Priorities { get; private set; } = [];
        public ObservableCollection<Category> Categories { get; private set; } = [];
        public ObservableCollection<Subdivision> Subdivisions { get; private set; } = [];
        private Priority? _selectedPriority;
        public Priority? SelectedPriority
        {
            get => _selectedPriority;
            set { _selectedPriority = value; OnPropertyChanged(); }
        }

        private Category? _selectedCategory;
        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set { _selectedCategory = value; OnPropertyChanged(); }
        }

        private Subdivision? _selectedSubdivision;
        public Subdivision? SelectedSubdivision
        {
            get => _selectedSubdivision;
            set { _selectedSubdivision = value; OnPropertyChanged(); }
        }

        public string? resualts;

        private (bool IsValid, string Message) ValidateSelection()
        {
            var missing = new List<string>();
            if (SelectedSubdivision == null) missing.Add("подразделение");
            if (SelectedPriority == null) missing.Add("приоритет");
            if (SelectedCategory == null) missing.Add("категорию");

            if (missing.Count == 0) return (true, string.Empty);

            string message = "Выберите " + string.Join(" и ", missing);
            return (false, message);
        }

        public async Task LoadDataAsync()
        {
            var priorities = await Task.Run(() => DBService.GetAllFrom<Priority>(DBService.tablenameP).ToList() ?? []);
            var categories = await Task.Run(() => DBService.GetAllFrom<Category>(DBService.tablenameC).ToList() ?? []);
            var subdivisions = await Task.Run(() => DBService.GetAllFrom<Subdivision>(DBService.tablenameSUB).ToList() ?? []);

            Application.Current.Dispatcher.Invoke(() =>
            {
                Priorities.Clear();
                foreach (var p in priorities)
                    Priorities.Add(p);
                Categories.Clear();
                foreach (var c in categories)
                    Categories.Add(c);
                Subdivisions.Clear();
                foreach (Subdivision? sub in subdivisions)
                    Subdivisions.Add(sub);
            });
            IsDataLoaded = true;
        }

        private bool _isDataLoaded;
        public bool IsDataLoaded
        {
            get => _isDataLoaded;
            set { _isDataLoaded = value; OnPropertyChanged(); }
        }

        public AddWindow()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                Messageb.ShowError(ex.Message);
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (AuthService.CurrentUser == null)
            {
                Messageb.ShowError("Пользователь не авторизован");
                DialogResult = false;
                Close();
                return;
            }

            var (isValid, message) = ValidateSelection();
            if (!isValid)
            {
                Messageb.ShowWarning(message);
                return;
            }

            if (string.IsNullOrWhiteSpace(CommentTextBox.Text))
            {
                Messageb.ShowWarning("Введите описание инцидента");
                return;
            }

            ResultIncident = new Incident
            {
                Id = 0,
                Author = AuthService.CurrentUser.FullName,
                Subdivision = SelectedSubdivision!.Name,
                Category = SelectedCategory!.Name,
                Description = CommentTextBox.Text,
                Priority = SelectedPriority!.Name,
                RegistrationDate = DateTime.Now,
                Status = "новый",
                CreatedById = AuthService.CurrentUser.Id,
            };

            DialogResult = true;
            Close();
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