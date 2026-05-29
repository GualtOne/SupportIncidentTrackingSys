using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SupportIncidentTrackingSys.Models
{
    public class Incident : INotifyPropertyChanged
    {
        private int _id;
        private string? _author;
        private string? _subdivision;
        private string? _category;
        private string? _description;
        private string? _priority;
        private DateTime? _regdate;
        private DateTime? _resptime;
        private DateTime? _decisiondeadline;
        private string? _responsible;
        private string? _status;
        private int _responsibleid;
        private int _createdbyid;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        public string? Author
        {
            get => _author ?? "Нету";
            set { _author = value; OnPropertyChanged();}
        }

        public string? Subdivision
        {
            get => _subdivision ?? "Нету";
            set { _subdivision = value; OnPropertyChanged(); }
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
            set 
            { 
                _priority = value; 
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsOverdue));
            }
        }

        public DateTime? RegistrationDate
        {
            get => _regdate;
            set { _regdate = value; OnPropertyChanged(); }
        }

        public DateTime? ResponseTime
        {
            get => _resptime;
            set 
            { 
                _resptime = value; 
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsOverdue));
            }
        }

        public DateTime? DecisionDeadline
        {
            get => _decisiondeadline;
            set { _decisiondeadline = value; OnPropertyChanged(); }
        }

        public string? Responsible
        {
            get => _responsible ?? "Нету";
            set { _responsible = value; OnPropertyChanged(); }
        }

        public string? Status
        {
            get => _status ?? "Нету";
            set 
            { 
                _status = value; 
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsOverdue));
                OnPropertyChanged(nameof(IsRejected));
            }
        }

        public int ResponsibleId
        {
            get => _responsibleid;
            set { _responsibleid = value; OnPropertyChanged(); }
        }

        public int CreatedById
        {
            get => _createdbyid;
            set { _createdbyid = value; OnPropertyChanged(); }
        }

        public bool IsOverdue => IsRealyOverdue();

        public bool IsRejected => Status == "отклонен";

        private bool IsRealyOverdue()
        {
            if (Status == "закрыт" || Status == "решен" || Status == "отклонен")
                return false;

            if (!ResponseTime.HasValue)
                return false;

            int hours = Priority switch
            {
                "Низкий" => 24,
                "Средний" => 8,
                "Высокий" => 4,
                "Критический" => 1,
                _ => 0
            };

            DateTime deadline = ResponseTime.Value.AddHours(hours);
            return DateTime.Now > deadline;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
