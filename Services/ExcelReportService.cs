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
                ExportReportOlympiadExcel(saveFileDialog.FileName, participants);
            }
        }
        private void ExportReportOlympiadExcel(string filePath, List<ParticipantInfo> participants)
        {

            Excel.Application excelApp = new Excel.Application();
            excelApp.Visible = false;

            Excel.Workbook workbook = excelApp.Workbooks.Add();
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];

            worksheet.Cells[1, 1] = "ФИО";
            worksheet.Cells[1, 2] = "Баллы";
            worksheet.Cells[1, 3] = "Результат";


            int rowIndex = 2;
            foreach (var reference in participants)
            {
                worksheet.Cells[rowIndex, 1] = reference.FIO;
                worksheet.Cells[rowIndex, 2] = reference.Score;
                worksheet.Cells[rowIndex, 3] = reference.Result;


                rowIndex++;
            }

            workbook.SaveAs(filePath);
            workbook.Close();
            excelApp.Quit();


            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);

            MessageBox.Show($"Протокол сохранен {filePath}", "Сохранение успешно", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void ExportReportYearExcel(string filePath, int year)
        {
            List<Olympiads> olympiadarr = db.context.Olympiads.Where(x => x.StartDate.Year == year).ToList();
            List<Results> results = new List<Results> { };
            foreach (var item in olympiadarr)
            {
                results = db.context.Results.Include("Registrations").Where(x => x.Registrations.OlympiadId == item.OlympiadId).ToList();
            }


            Excel.Application excelApp = new Excel.Application();
            excelApp.Visible = false;

            Excel.Workbook workbook = excelApp.Workbooks.Add();
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];

            worksheet.Cells[1, 1] = $"Олимпиады за {year} год";
            worksheet.Cells[1, 2] = "Общее кл-во участников";
            worksheet.Cells[1, 3] = "Кол-во призеров";
            worksheet.Cells[1, 4] = "Кол-во победителей";


            int rowIndex = 2;
            foreach (var reference in results)
            {
                worksheet.Cells[rowIndex, 1] = db.context.Olympiads.FirstOrDefault(x => x.OlympiadId == reference.Registrations.OlympiadId);
                worksheet.Cells[rowIndex, 2] = db.context.Results.Count(x => x.RegistrationId == reference.Registrations.RegistrationId);
                worksheet.Cells[rowIndex, 3] = db.context.Results.Count(x => x.ResultType == "PrizeWinner");
                worksheet.Cells[rowIndex, 4] = db.context.Results.Count(x => x.ResultType == "Winner");


                rowIndex++;
            }

            workbook.SaveAs(filePath);
            workbook.Close();
            excelApp.Quit();


            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);

            MessageBox.Show($"Протокол сохранен {filePath}", "Сохранение успешно", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
