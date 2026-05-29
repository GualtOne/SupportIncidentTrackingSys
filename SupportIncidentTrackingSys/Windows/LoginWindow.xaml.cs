using System.Windows;
using SupportIncidentTrackingSys.Role;

namespace SupportIncidentTrackingSys
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            bool flowControl = CheckIfDone();
            if (!flowControl)
            {
                return;
            }
            var user = AuthService.Login(LoginTextBox.Text, PasswordTextBox.Password);
            if (user != null)
            {
                AuthService.CurrentUser = user;
                DialogResult = true;
                Close();
            }
            else
                Messageb.ShowWarning("Неверный логин или пароль");
        }

        private bool CheckIfDone()
        {
            switch (LoginTextBox.Text.Length)
            {
                case <= 0 when PasswordTextBox.Password.Length <= 0:
                    Messageb.ShowWarning("Ввидите логин и пароль");
                    return false;
                case <= 0:
                    Messageb.ShowWarning("Ввидите логин");
                    return false;
                default:
                    if (PasswordTextBox.Password.Length <= 0)
                    {
                        Messageb.ShowWarning("Ввидите пароль");
                        return false;
                    }

                    break;
            }

            return true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
