
using System.Windows;
using SupportIncidentTrackingSys.Models;

namespace SupportIncidentTrackingSys
{
    /// <summary>
    /// Логика взаимодействия для StartingWindow.xaml
    /// </summary>
    public partial class StartingWindow : Window
    {
        public StartingWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e) 
        {
            LoginWindow login = new();
            if (login.ShowDialog() == true)
            {
                MainWindow main = new();
                main.Show();
                Close();
            }
        }

        private void ReggisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow register = new();
            if (register.ShowDialog() == true)
            {
                User? user = register.NewUser ?? new User();
                DBservice.DBService.AddUser(user);
                Messageb.ShowMessage("Вы зарегистриованы! Теперь можете войти");
            }
        }
    }
}
