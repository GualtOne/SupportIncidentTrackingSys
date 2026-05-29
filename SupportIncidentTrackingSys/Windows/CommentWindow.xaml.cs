using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SupportIncidentTrackingSys.DBservice;
using SupportIncidentTrackingSys.Models;
using SupportIncidentTrackingSys.Role;

namespace SupportIncidentTrackingSys.Windows
{
    /// <summary>
    /// Логика взаимодействия для CommentWindow.xaml
    /// </summary>
    public partial class CommentWindow : Window
    {
        private readonly CommentsHistory _originalcomment;
        public CommentsHistory? Resualtcomment { get; private set; }


        public CommentWindow(CommentsHistory Comment)
        {
            InitializeComponent();
            DataContext = this;
            Loaded += OnLoaded;
            _originalcomment = Comment;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadcommenttData();
            }
            catch (Exception ex)
            {
                Messageb.ShowError(ex.Message);
            }
        }

        private void LoadcommenttData()
        {
            CommentTextBox.Text = _originalcomment.Comment ?? "";
        }
          

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (AuthService.CurrentUser == null)
            {
                Messageb.ShowError("Пользователь не авторизован");
                DialogResult = false;
                Close();
                return;
            }


            if (string.IsNullOrWhiteSpace(CommentTextBox.Text))
            {
                Messageb.ShowWarning("Введите комментарий");
                return;
            }

            Resualtcomment = new CommentsHistory()
            {
                Id = _originalcomment.Id,
                IncidentId = _originalcomment.IncidentId,
                ActionType = _originalcomment.ActionType,
                Comment = CommentTextBox.Text,
                Timestamp = DateTime.Now,
            };

            DialogResult = true;
            Close();
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
