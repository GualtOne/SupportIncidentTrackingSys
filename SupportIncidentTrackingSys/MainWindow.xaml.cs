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
        private MainViewModel MainVM => (MainViewModel)DataContext;
        private IIncident IncidentsVM => MainVM.IncidentsVM;
        public void AddItem_Click(object sender, RoutedEventArgs e)
        {
            AddWindow dialog = new()
            {
                Owner = this
            };
            if (dialog.ShowDialog() == true)
            {
                var newIncident = dialog.ResultIncident;
                if (newIncident != null)
                {
                    IncidentsVM.AddIncident(newIncident);
                }
            }
        }

        public void EditItem_Click(object sender, RoutedEventArgs e)
        {
            if (MyDataGrid.SelectedItem is not Incident selected) return;

            EditWindow dialog = new(selected)
            {
                Owner = this
            };
            if (dialog.ShowDialog() == true)
            {
                var newIncident = dialog.ResultIncident;
                if (newIncident != null)
                    IncidentsVM.UpdateIncident(newIncident);
            }
        }

        public void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (MyDataGrid.SelectedItem is Incident selected)
            {
                IncidentsVM.DeleteIncident(selected);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}