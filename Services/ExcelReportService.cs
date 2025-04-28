using Olympiad.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using Olympiad.Controllers;
using System.Runtime.Remoting.Contexts;
using System.Runtime.InteropServices;

namespace Olympiad.Services
{
    public class ExcelReportService
    {

        Core db = new Core();
        Olympiads olympiad = new Olympiads();
        RegistrationsController registrations = new RegistrationsController();

        public void ExportOlympiadReport(int olympid)
        {
            var participants = registrations.ProtocolsEdit(olympid);
            olympiad = db.context.Olympiads.FirstOrDefault(x => x.OlympiadId == olympid);
            string fileName = $"Протокол для {olympiad.Name}.xlsx";
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                FileName = fileName
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                Olympiads olympiads = db.context.Olympiads.FirstOrDefault(x => x.OlympiadId == olympid);
                ExportReportOlympiadExcel(saveFileDialog.FileName, olympiads, participants);
            }
        }
        private void ExportReportOlympiadExcel(string filePath, Olympiads olympiad, List<ParticipantInfo> participants)
        {
            Excel.Application excelApp = new Excel.Application();
            excelApp.Visible = false;

            Excel.Workbook workbook = excelApp.Workbooks.Add();
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];

            // Заголовок отчета
            int currentRow = 1;
            worksheet.Cells[currentRow, 1] = "Название олимпиады:";
            worksheet.Cells[currentRow, 2] = olympiad.Name;
            currentRow++;
            worksheet.Cells[currentRow, 1] = "Дата проведения:";
            worksheet.Cells[currentRow, 2] = $"{olympiad.StartDate:dd.MM.yyyy} - {olympiad.EndDate:dd.MM.yyyy}";
            currentRow += 2;

            // Заголовки таблицы (только доступные поля)
            var headers = new[] { "ФИО", "Баллы", "Результат" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[currentRow, i + 1] = headers[i];
                worksheet.Cells[currentRow, i + 1].Font.Bold = true;
                worksheet.Cells[currentRow, i + 1].Borders.Weight = Excel.XlBorderWeight.xlThin;
            }
            currentRow++;

            // Данные участников
            foreach (var participant in participants)
            {
                worksheet.Cells[currentRow, 1] = participant.FIO;
                worksheet.Cells[currentRow, 2] = participant.Score?.ToString() ?? "0"; // обработка null
                worksheet.Cells[currentRow, 3] = participant.Result;

                for (int i = 1; i <= 3; i++)
                {
                    worksheet.Cells[currentRow, i].Borders.Weight = Excel.XlBorderWeight.xlThin;
                }
                currentRow++;
            }

            // Статистика (с проверкой null)
            worksheet.Cells[currentRow + 1, 1] = "Итоговая статистика:";
            worksheet.Cells[currentRow + 1, 1].Font.Bold = true;

            var validScores = participants.Where(p => p.Score.HasValue).Select(p => p.Score.Value).ToList();
            var averageScore = validScores.Any() ? validScores.Average() : 0;

            worksheet.Cells[currentRow + 2, 1] = "Всего участников:";
            worksheet.Cells[currentRow + 2, 2] = participants.Count;
            worksheet.Cells[currentRow + 3, 1] = "Победителей:";
            worksheet.Cells[currentRow + 3, 2] = participants.Count(p => p.Result == "Winner");
            worksheet.Cells[currentRow + 4, 1] = "Призеров:";
            worksheet.Cells[currentRow + 4, 2] = participants.Count(p => p.Result == "PrizeWinner");
            worksheet.Cells[currentRow + 5, 1] = "Средний балл:";
            worksheet.Cells[currentRow + 5, 2] = Math.Round(averageScore, 2);

            // Форматирование
            worksheet.Columns.AutoFit();
            worksheet.Rows.AutoFit();

            // Сохранение
            workbook.SaveAs(filePath);
            workbook.Close();
            excelApp.Quit();

            // Освобождение ресурсов
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);

            MessageBox.Show($"Отчет сохранен: {filePath}", "Сохранение успешно",
                           MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ExportYearReport(int year)
        {
            string fileName = $"Протокол за {year} год.xlsx";
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                FileName = fileName
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                ExportReportYearExcel(saveFileDialog.FileName, year);
            }
        }
        //results = db.context.Results.Include("Registrations").Where(x => x.Registrations.OlympiadId == item.OlympiadId).ToList();
        private void ExportReportYearExcel(string filePath, int year)
        {
            var olympiads = db.context.Olympiads
                .Include("Registrations")
                .Include("Registrations.Results") // Загружаем связанные результаты
                .Where(x => x.StartDate.Year == year)
                .ToList();

            Excel.Application excelApp = new Excel.Application();
            excelApp.Visible = false;

            Excel.Workbook workbook = excelApp.Workbooks.Add();
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];

            // Заголовок
            int currentRow = 1;
            worksheet.Cells[currentRow, 1] = $"Олимпиады за {year} год";
            worksheet.Cells[currentRow, 1].Font.Bold = true;
            currentRow += 2;

            // Заголовки таблицы
            var headers = new[]
            {
        "Название олимпиады",
        "Дата начала",
        "Дата окончания",
        "Участников",
        "Победителей",
        "Призеров"
    };

            // Стили для заголовков
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[currentRow, i + 1] = headers[i];
                worksheet.Cells[currentRow, i + 1].Font.Bold = true;
                worksheet.Cells[currentRow, i + 1].Interior.Color = Excel.XlRgbColor.rgbLightGray;
            }
            currentRow++;

            // Данные
            foreach (var olympiad in olympiads)
            {
                // Получаем все результаты для олимпиады
                var results = olympiad.Registrations
                    .SelectMany(r => r.Results)
                    .ToList();

                // Считаем статистику
                var winners = results.Count(r => r.ResultType == "Winner");
                var prizeWinners = results.Count(r => r.ResultType == "PrizeWinner");

                worksheet.Cells[currentRow, 1] = olympiad.Name;
                worksheet.Cells[currentRow, 2] = olympiad.StartDate.ToString("dd.MM.yyyy");
                worksheet.Cells[currentRow, 3] = olympiad.EndDate.ToString("dd.MM.yyyy");
                worksheet.Cells[currentRow, 4] = olympiad.Registrations.Count;
                worksheet.Cells[currentRow, 5] = winners;
                worksheet.Cells[currentRow, 6] = prizeWinners;

                currentRow++;
            }

            // Итоговая статистика
            var allResults = olympiads
                .SelectMany(o => o.Registrations)
                .SelectMany(r => r.Results)
                .ToList();

            worksheet.Cells[currentRow + 2, 1] = "Всего олимпиад:";
            worksheet.Cells[currentRow + 2, 2] = olympiads.Count;
            worksheet.Cells[currentRow + 3, 1] = "Всего участников:";
            worksheet.Cells[currentRow + 3, 2] = olympiads.Sum(o => o.Registrations.Count);
            worksheet.Cells[currentRow + 4, 1] = "Всего победителей:";
            worksheet.Cells[currentRow + 4, 2] = allResults.Count(r => r.ResultType == "Winner");
            worksheet.Cells[currentRow + 5, 1] = "Всего призеров:";
            worksheet.Cells[currentRow + 5, 2] = allResults.Count(r => r.ResultType == "PrizeWinner");

            // Форматирование
            worksheet.Columns.AutoFit();
            worksheet.Range["A:F"].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

            workbook.SaveAs(filePath);
            workbook.Close();
            excelApp.Quit();

            // Освобождение ресурсов
            Marshal.ReleaseComObject(worksheet);
            Marshal.ReleaseComObject(workbook);
            Marshal.ReleaseComObject(excelApp);

            MessageBox.Show($"Отчет сохранен: {filePath}", "Успешно",
                           MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
