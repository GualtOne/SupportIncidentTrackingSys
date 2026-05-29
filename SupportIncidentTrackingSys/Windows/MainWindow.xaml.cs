using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;
using SupportIncidentTrackingSys.Interfaces;
using SupportIncidentTrackingSys.Models;
using SupportIncidentTrackingSys.Views;
using SupportIncidentTrackingSys.Windows;

namespace SupportIncidentTrackingSys
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel? MainVM => DataContext as MainViewModel;
        private IIncident? IncidentsVM => MainVM?.IncidentsVM;
        private readonly DispatcherTimer _overdueTimer;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            _overdueTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(5)
            };
            _overdueTimer.Tick += OnOverdueCheckTick;
            _overdueTimer.Start();
        }

        private void OnOverdueCheckTick(object? sender, EventArgs e)
        {
            IncidentsVM?.CheckOverdue();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _overdueTimer?.Stop();
            base.OnClosing(e);
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddWindow { Owner = this };
            if (dialog.ShowDialog() == true && dialog.ResultIncident != null)
            {
                IncidentsVM?.AddIncident(dialog.ResultIncident);
            }
        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            if (MyDataGrid.SelectedItem is not Incident selected) return;

            var dialog = new EditWindow(selected) { Owner = this };
            if (dialog.ShowDialog() == true && dialog.ResultIncident != null)
            {
                IncidentsVM?.UpdateIncident(dialog.ResultIncident);
            }
        }

        private void EditCommentItem_Click(object? sender, RoutedEventArgs e)
        {
            if (HistoryDataGrid.SelectedItem is not CommentsHistory selected) return;
            var dialog = new CommentWindow(selected) { Owner = this };
            if (dialog.ShowDialog() == true && dialog.Resualtcomment != null)
            {
                IncidentsVM?.EditCommentHistory(dialog.Resualtcomment);
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (MyDataGrid.SelectedItem is not Incident selected) return;
            if (MessageBox.Show($"Удалить инцидент #{selected.Id}?", "Подтверждение", 
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                IncidentsVM?.DeleteIncident(selected);
            }
        }

        public void DeleteCommentItem_Click(object sender, RoutedEventArgs e)
        {
            if (HistoryDataGrid.SelectedItem is not CommentsHistory selected) return;
            if (MessageBox.Show($"Удалить запись #{selected.Id} из истории?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                IncidentsVM?.DeleteCommentHistory(selected);
            }
        }

        public void EditStatus_Click(object sender, RoutedEventArgs e)
        {
            if (MyDataGrid.SelectedItem is not Incident selected) return;

            var dialog = new ChangeStatusWindow(selected) { Owner = this };
            if (dialog.ShowDialog() == true && dialog.ResultIncident != null)
            {
                IncidentsVM?.ChangeStatus(dialog.ResultIncident);
            }
        }

        private void MyDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.DataContext is not Incident incident) return;

            if (incident.IsOverdue)
                e.Row.Background = new SolidColorBrush(Colors.LightCoral);
            else if (incident.IsRejected)
                e.Row.Background = new SolidColorBrush(Colors.LightGray);
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog { Filter = "Excel files|*.xlsx", DefaultExt = "xlsx" };
            if (dialog.ShowDialog() == true)
            {
                if (!string.IsNullOrEmpty(dialog.FileName) && IncidentsVM != null)
                    Report.ReportSystem.ExportToExcel(IncidentsVM.Incidents, dialog.FileName);
                    Messageb.ShowMessage("Отчет сохранен");
            }
        }

        private void EditResponsible_Click(object sender, RoutedEventArgs e)
        {
            if (MyDataGrid.SelectedItem is not Incident selected) return;

            var dialog = new AddResponsibleWindow(selected) { Owner = this };
            if (dialog.ShowDialog() == true && dialog.ResultIncident != null)
            {
                IncidentsVM?.ChangeResponsible(dialog.ResultIncident);
            }
        }

        private void ResetFilter_Click(object sender, RoutedEventArgs e)
        {
            if(IncidentsVM != null)
            {
                IncidentsVM.SearchText = string.Empty;
                IncidentsVM.SelectedFilterStatus = null;
            }

        }



        private async void AddAttachment_Click(object sender, RoutedEventArgs e)
        {
            if (MyDataGrid.SelectedItem is not Incident selected)
            {
                Messageb.ShowWarning("Сначала выберите инцидент.");
                return;
            }
            var openFileDialog = new OpenFileDialog { Filter = "Все файлы (*.*)|*.*" };
            if (openFileDialog.ShowDialog() == true)
            {
                await IncidentsVM!.AddAttachmentAsync(selected.Id, openFileDialog.FileName);
            }
        }

        private void MyDataGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (MyDataGrid.SelectedItem is not Incident selected)
            {
                AttachmentsSubMenu.Items.Clear();
                AttachmentsSubMenu.Items.Add(new MenuItem { Header = "(нет инцидента)", IsEnabled = false });
                return;
            }

            AttachmentsSubMenu.Items.Clear();

            var attachments = IncidentsVM?.CurrentIncidentAttachments;
            if (attachments == null || attachments.Count == 0)
            {
                AttachmentsSubMenu.Items.Add(new MenuItem { Header = "(нет файлов)", IsEnabled = false });
            }
            else
            {
                foreach (var att in attachments)
                {
                    var menuItem = new MenuItem
                    {
                        Header = att.FileName,
                        Tag = att,
                        ToolTip = att.FilePath
                    };
                    menuItem.Click += (s, ev) => OpenAttachment(att);
                    menuItem.ContextMenu = new ContextMenu();
                    var deleteItem = new MenuItem { Header = "Удалить", Tag = att };
                    deleteItem.Click += async (s, ev) => await DeleteAttachment(att);
                    menuItem.ContextMenu.Items.Add(deleteItem);
                    AttachmentsSubMenu.Items.Add(menuItem);
                }
            }
        }

        private static void OpenAttachment(Attachment att)
        {
            if (File.Exists(att.FilePath))
                Process.Start(new ProcessStartInfo(att.FilePath) { UseShellExecute = true });
            else
                Messageb.ShowWarning("Файл не найден.");
        }

        private async Task DeleteAttachment(Attachment att)
        {
            if (MessageBox.Show($"Удалить файл \"{att.FileName}\"?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                await IncidentsVM!.DeleteAttachmentAsync(att);
                MyDataGrid.ContextMenu.IsOpen = false;
            }
        }
    }
}