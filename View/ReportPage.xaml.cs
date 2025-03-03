using Olympiad.Model;
using Olympiad.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Olympiad.View
{
    /// <summary>
    /// Логика взаимодействия для ReportPage.xaml
    /// </summary>
    public partial class ReportPage : Page
    {
        Core db = new Core();
        List <Olympiads> olimparr = new List<Olympiads> ();
        ExcelReportService excelReportService = new ExcelReportService();
        List<int> date = new List<int>();
        HashSet<int> uniqueYears = new HashSet<int>();
        public ReportPage()
        {
            olimparr = db.context.Olympiads.ToList();
            InitializeComponent();


            foreach (var item in olimparr)
            {
                int year = item.StartDate.Year;
                if (uniqueYears.Add(year))
                {
                    date.Add(year);
                }
            }
            OlympiadComboBox.ItemsSource = olimparr;
            OlympiadComboBox.DisplayMemberPath = "Name";
            OlympiadComboBox.SelectedValuePath = "OlympiadId";
            OlympiadComboBox.SelectedIndex = 0;

            YearComboBox.ItemsSource = date;
            YearComboBox.SelectedIndex = 0;

        }

        private void CreateYearReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int.TryParse(YearComboBox.SelectedValue.ToString(), out int year);

                excelReportService.ExportYearReport(year);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CreateOlympiadReport_Click(object sender, RoutedEventArgs e)
        {
            try {
                int.TryParse(OlympiadComboBox.SelectedValue.ToString(), out int olympiadId);

                excelReportService.ExportOlympiadReport(olympiadId);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void BackArrowlLink(object sender, RequestNavigateEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}
