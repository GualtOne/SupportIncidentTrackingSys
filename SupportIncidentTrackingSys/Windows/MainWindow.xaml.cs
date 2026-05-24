using System.Windows;
using SupportIncidentTrackingSys.Interfaces;
using SupportIncidentTrackingSys.Models;
using SupportIncidentTrackingSys.Views;

namespace SupportIncidentTrackingSys
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel? MainVM => DataContext as MainViewModel;
        private IIncident? IncidentsVM => MainVM?.IncidentsVM;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddWindow { Owner = this };
            if (dialog.ShowDialog() == true && dialog.ResultIncident != null)
            {
                IncidentsVM?.AddIncident(dialog.ResultIncident);
            }
        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            if (MyDataGrid.SelectedItem is not Incident selected) return;

            var dialog = new EditWindow(selected) { Owner = this };
            if (dialog.ShowDialog() == true && dialog.ResultIncident != null)
            {
                IncidentsVM?.UpdateIncident(dialog.ResultIncident);
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (MyDataGrid.SelectedItem is not Incident selected) return;
            if (MessageBox.Show($"Удалить инцидент #{selected.Id}?", "Подтверждение", 
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                IncidentsVM?.DeleteIncident(selected);
            }
        }

        public void DeleteCommentItem_Click(object sender, RoutedEventArgs e)
        {
            //if (MyDataGrid.SelectedItem is Incident selected)
        }

        public void EditCommentItem_Click(object sender, RoutedEventArgs e)
        {
            //if (HistoryDataGrid.SelectedItem is not Incident selected) return;
        }
    }
}