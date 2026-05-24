
using System.ComponentModel;

namespace SupportIncidentTrackingSys.Models
{
    public class CommentsHistory
    {
        public int Id { get; set; }
        public int IncidentId { get; set; }
        public string? Comment { get; set; }
        public string? ActionType { get; set; }
        public DateTime? Timestamp { get; set; } = DateTime.Now;

        public enum ActionTypes
        {
            Добавлен,
            [Description ("Добавлен Комментарий")]
            ДобавленКомментарий,
            Изменена,
            Удален,
            [Description("Добавлен Отвественный")]
            ДобавленОтвественный,
        }
    }
}
