
using System.Windows;

namespace SupportIncidentTrackingSys
{
    public class Messageb
    {
        public static void ShowError(string txt)
        {
            MessageBox.Show(txt, "Ошибка" ,MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowWarning(string txt)
        {
            MessageBox.Show(txt, "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void ShowMessage(string txt)
        {
            MessageBox.Show(txt, "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
