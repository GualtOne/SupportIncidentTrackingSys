using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SupportIncidentTrackingSys.Models
{
    public class Incident : INotifyPropertyChanged
    {
        private int _id;
        private string? _author;
        private string? _subdivison;
        private string? _category;
        private string? _description;
        private string? _priority;
        private DateTime? _regdate;
        private DateTime? _resptime;
        private DateTime? _decisiondeadline;
        private string? _responsibale;
        private string? _status;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged();}
        }

        public string? Author
        {
            get => _author ?? "Нету";
            set { _author = value; OnPropertyChanged();}
        }

        public string? Subdivison
        {
            get => _subdivison ?? "Нету";
            set { _subdivison = value; OnPropertyChanged(); }
        }

        public string? Category
        {
            get => _category ?? "Нету";
            set { _category = value; OnPropertyChanged(); }
        }

        public string? Description
        {
            get => _description ?? "Нету";
            set { _description = value; OnPropertyChanged(); }
        }

        public string? Priority
        {
            get => _priority ?? "Нету";
            set { _priority = value; OnPropertyChanged(); }
        }

        public DateTime? Regdate
        {
            get => _regdate;
            set { _regdate = value; OnPropertyChanged(); }
        }

        public DateTime? ResponseTime
        {
            get => _resptime;
            set { _resptime = value; OnPropertyChanged(); }
        }

        public DateTime? DecisionDeadline
        {
            get => _decisiondeadline;
            set { _decisiondeadline = value; OnPropertyChanged(); }
        }

        public string? Responsibale
        {
            get => _responsibale ?? "Нету";
            set { _responsibale = value; OnPropertyChanged(); }
        }

        public string? Status
        {
            get => _status ?? "Нету";
            set { _status = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
