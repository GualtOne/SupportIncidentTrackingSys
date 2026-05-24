using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using SupportIncidentTrackingSys.Models;

namespace SupportIncidentTrackingSys
{
    /// <summary>
    /// Логика взаимодействия для RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public IEnumerable<User>? users;
        public User? NewUser { get; private set; }

        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            bool flowControl = AuthEverything();
            if (!flowControl)
            {
                return;
            }

            string pass = Hash.MakeHash(PasswordTextBox.Password);

            NewUser = new User
            {
                Id = 0,
                Login = LoginTextBox.Text,
                Role = Role.Role.Заявитель,
                FullName = FullnameTextBox.Text,
                PasswordHash = pass,
                IsActive = true,
            };

            DialogResult = true;
            Close();
        }

        private bool AuthEverything()
        {
            users = DBservice.DBService.GetAllFrom<User>(DBservice.DBService.tablenameUS);
            if (LoginTextBox.Text == null || LoginTextBox.Text.Length == 0)
            {
                Messageb.ShowWarning("Ввидите логин");
                return false;
            }

            foreach (User user in users)
            {
                if (user.Login == LoginTextBox.Text)
                {
                    Messageb.ShowWarning("Пользователь с таким логином уже есть");
                    return false;
                }
            }

            if (FullnameTextBox.Text == null || FullnameTextBox.Text.Length == 0)
            {
                Messageb.ShowWarning("Ввидите ваше ФИО");
                return false;
            }

            if (!(PasswordTextBox.Password.Length > 0))
            {
                Messageb.ShowWarning("Напишите пароль.");
                return false;
            }
            else if (PasswordTextBox.Password.Length < 8)
            {
                Messageb.ShowWarning("Пароль должен быть длинее 8 символов");
                return false;
            }
            if (PasswordTextBox.Password != ConfirmPasswordTextBox.Password)
            {
                Messageb.ShowWarning("Пароли должны совпадать");
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
