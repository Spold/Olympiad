using Olympiad.Model;
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
    /// Логика взаимодействия для OlympiadListView.xaml
    /// </summary>
    public partial class OlympiadListView : Page
    {
        Core db = new Core();
        List<Olympiads> olimparr = new List<Olympiads>();
        List<Registrations> registrationsarr = new List<Registrations>();
        public OlympiadListView()
        {
            registrationsarr = db.context.Registrations.Where(x => x.StudentUserId == Properties.Settings.Default.UserId).ToList();
            InitializeComponent();
            AllOlympiads();
            OlympiadList.ItemsSource = olimparr;
            List<string> items = new List<string>() { "Все года", "2025", "2024", "2023" };
            YearComboBox.ItemsSource = items;
            YearComboBox.SelectedIndex = 0;
        }

        public void TutorOlympiads()
        {
            olimparr.Clear();
            olimparr = db.context.Olympiads.Where(x => x.ResponsibleTeacherUserId == Properties.Settings.Default.UserId).ToList();
            YearSortOlimpiads();
        }


        public void AllOlympiads()
        {
            olimparr.Clear();
            olimparr = db.context.Olympiads.ToList();
            YearSortOlimpiads();
        }

        public void UserOlympiads()
        {
            olimparr.Clear();
            List<int> olympiadIds = registrationsarr.Select(x => x.OlympiadId).ToList();
            olimparr = db.context.Olympiads.Where(x => olympiadIds.Contains(x.OlympiadId)).ToList();
            YearSortOlimpiads();
        }

        public void YearSortOlimpiads()
        {
            string selectedYear = YearComboBox.SelectedItem as string;
            if (selectedYear == "Все года")
            {

                OlympiadList.ItemsSource = olimparr;
                return;
            }
            else
            {
                if (int.TryParse(selectedYear, out int year)) 
                {
                 
                    olimparr = olimparr.Where(x => x.StartDate.Year == year).ToList();
                    OlympiadList.ItemsSource = olimparr;
                }


            }
        }



        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StackPanel stackPanel = sender as StackPanel;
            Olympiads olympiads = stackPanel.DataContext as Olympiads;
            this.NavigationService.Navigate(new OlympiadDetailPage(olympiads.OlympiadId));
        }

        private void AllOlympiadsRadio_Checked(object sender, RoutedEventArgs e)
        {
            AllOlympiads();
        }

        private void TutorRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            TutorOlympiads();
        }

        private void ParticipantRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            UserOlympiads();
        }

        private void YearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TutorRadioButton.IsChecked == true)
            {
                TutorOlympiads();
            }
            else if (ParticipantRadioButton.IsChecked == true)
            {
                UserOlympiads();
            }
            else
            {
                AllOlympiads();
            }
        }


    }
}
