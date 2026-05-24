using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SupportIncidentTrackingSys.Models
{
    public class User
    {
        private int _id;
        private string? _login;
        private string? _passwordhash;
        private string? _fullname;
        private Role.Role? _role;
        private bool? _isactive;

        public int Id
        {
            get => _id; 
            set { _id = value; OnPropertyChanged(); }
        }

        public string? Login
        {
            get => _login;
            set { _login = value; OnPropertyChanged(); }
        }

        public string? PasswordHash
        {
            get => _passwordhash;
            set { _passwordhash = value; OnPropertyChanged(); }
        }

        public string? FullName
        {
            get => _fullname;
            set { _fullname = value; OnPropertyChanged(); }
        }

        public Role.Role? Role
        {
            get => _role;
            set { _role = value; OnPropertyChanged(); }
        }

        public bool? IsActive
        {
            get => _isactive;
            set { _isactive = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
