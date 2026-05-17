using System.Windows;
using System.Windows.Controls;
using SupportIncidentTrackingSys.Models;


namespace SupportIncidentTrackingSys
{
    public partial class EditWindow : Window
    {
        private readonly Incident _originalIncident;

        public Incident? ResultIncident { get; private set; }

        public EditWindow(Incident Incident)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            _originalIncident = Incident;
            LoadIncidentData();
        }

        private void LoadIncidentData()
        {
            DatePicker.SelectedDate = _originalIncident.Regdate;


            CommentTextBox.Text = _originalIncident.Description ?? "";
        }
        //TO DO: Многа чево
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime date = DatePicker.SelectedDate ?? DateTime.Today;


            string comment = CommentTextBox.Text;

            ResultIncident = new Incident
            {
                Id = _originalIncident.Id,
                Regdate = date,
                Description = comment
            };

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}