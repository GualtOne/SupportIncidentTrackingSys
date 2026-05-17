
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SupportIncidentTrackingSys.Models
{
    public class Priority : INotifyPropertyChanged
    {
        private int _id;
        private string? _name;
        public int ID
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }
        public string? Name
        {
            get => _name;
            set { _name = value ?? "нету"; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
