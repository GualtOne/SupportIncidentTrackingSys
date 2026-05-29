using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SupportIncidentTrackingSys.Models
{
    public class Attachment : INotifyPropertyChanged
    {
        private int _id;
        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        private int _incidentId;
        public int IncidentId
        {
            get => _incidentId;
            set { _incidentId = value; OnPropertyChanged(); }
        }

        private string _fileName = string.Empty;
        public string FileName
        {
            get => _fileName;
            set { _fileName = value; OnPropertyChanged(); }
        }

        private string _filePath = string.Empty;
        public string FilePath
        {
            get => _filePath;
            set { _filePath = value; OnPropertyChanged(); }
        }

        private DateTime _uploadedAt;
        public DateTime UploadedAt
        {
            get => _uploadedAt;
            set { _uploadedAt = value; OnPropertyChanged(); }
        }

        private int? _uploadedBy;
        public int? UploadedBy
        {
            get => _uploadedBy;
            set { _uploadedBy = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
