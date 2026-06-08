using ClosedXML.Excel;
using SupportIncidentTrackingSys.Models;

namespace SupportIncidentTrackingSys.Report
{
    public class ReportSystem
    {
        public static void ExportToExcel(IEnumerable<Incident> incidents, string filePath)
        {

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Incidents");

            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "Автор";
            worksheet.Cell(1, 3).Value = "Подразделение";
            worksheet.Cell(1, 4).Value = "Категория";
            worksheet.Cell(1, 5).Value = "Описание";
            worksheet.Cell(1, 6).Value = "Приоритет";
            worksheet.Cell(1, 7).Value = "Дата регистрации";
            worksheet.Cell(1, 8).Value = "Срок реакции";
            worksheet.Cell(1, 9).Value = "Cрок решения";
            worksheet.Cell(1, 10).Value = "Ответственный";
            worksheet.Cell(1, 11).Value = "Статус";

            int row = 2;
            foreach (var inc in incidents)
            {
                worksheet.Cell(row, 1).Value = inc.Id;
                worksheet.Cell(row, 2).Value = inc.Author;
                worksheet.Cell(row, 3).Value = inc.Subdivision;
                worksheet.Cell(row, 4).Value = inc.Category;
                worksheet.Cell(row, 5).Value = inc.Description;
                worksheet.Cell(row, 6).Value = inc.Priority;
                worksheet.Cell(row, 7).Value = inc.RegistrationDate?.ToString("dd.MM.yyyy HH:mm");
                worksheet.Cell(row, 8).Value = inc.ResponseTime?.ToString("dd.MM.yyyy HH:mm");
                worksheet.Cell(row, 9).Value = inc.DecisionDeadline?.ToString("dd.MM.yyyy HH:mm");
                worksheet.Cell(row, 10).Value = inc.Responsible;
                worksheet.Cell(row, 11).Value= inc.Status;
                row++;
            }

            worksheet.Columns().AdjustToContents();
            workbook.SaveAs(filePath);
        }

        public static void ExportCommentToExcel(IEnumerable<CommentsHistory> comments, string filepath) 
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("CommentsHistory");

            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "ID инцидента";
            worksheet.Cell(1, 3).Value = "Тип действия";
            worksheet.Cell(1, 4).Value = "Дата";
            worksheet.Cell(1, 5).Value = "Коментарий";

            int row = 2;
            foreach (var commnet in comments)
            {
                worksheet.Cell(row, 1).Value = commnet.Id;
                worksheet.Cell(row, 2).Value = commnet.IncidentId;
                worksheet.Cell(row, 3).Value = commnet.ActionType;
                worksheet.Cell(row, 4).Value = commnet.Timestamp?.ToString("dd.MM.yyyy HH:mm");
                worksheet.Cell(row, 5).Value = commnet.Comment;
                row++;
            }

            worksheet.Columns().AdjustToContents();
            workbook.SaveAs(filepath);
        }
    }
}
