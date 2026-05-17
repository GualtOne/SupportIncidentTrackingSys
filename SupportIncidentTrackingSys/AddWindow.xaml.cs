using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using SupportIncidentTrackingSys.DBservice;
using SupportIncidentTrackingSys.Models;

namespace SupportIncidentTrackingSys
{
    public partial class AddWindow : Window ,INotifyPropertyChanged
    {
        public Incident? ResultIncident { get; private set; }
        public ObservableCollection<Priority> Priorities { get; private set; } = [];
        public ObservableCollection<Category> Categories { get; private set; } = [];
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

        public async Task LoadDataAsync()
        {
            var priorities = await Task.Run(() => DBService.GetAllFrom<Priority>(DBService.tablenameP).ToList() ?? []);
            var categories = await Task.Run(() => DBService.GetAllFrom<Category>(DBService.tablenameC).ToList() ?? []);

            Application.Current.Dispatcher.Invoke(() =>
            {
                Priorities.Clear();
                foreach (var p in priorities) Priorities.Add(p);
                Categories.Clear();
                foreach(var c in categories) Categories.Add(c);
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
            Owner = Application.Current.MainWindow;
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

            if (SelectedPriority == null && SelectedCategory == null)
            {
                Messageb.ShowWarning("Выберите приоритет и категорию");
                return;
            }
            else if (SelectedPriority == null)
            {
                Messageb.ShowWarning("Выберите приоритет");
                return;
            }
            else if (SelectedCategory == null)
            {
                Messageb.ShowWarning("Выберите категорию");
                return;
            }

            ResultIncident = new Incident
            {
                Id = 0,
                Author = AuthorTextBox.Text,
                Subdivison = SubdivisionTextBox.Text,
                Category = SelectedCategory.Name,
                Description = CommentTextBox.Text,
                Priority = SelectedPriority.Name,
                Regdate = DateTime.Today,
                Status = "новый",
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