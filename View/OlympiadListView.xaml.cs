using Olympiad.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        ObservableCollection<Olympiads> olimparr = new ObservableCollection<Olympiads>();
        List<Registrations> registrationsarr = new List<Registrations>();
        public OlympiadListView()
        {
            registrationsarr = db.context.Registrations.Where(x => x.StudentUserId == Properties.Settings.Default.UserId).ToList();
            InitializeComponent();
            LoadItems();
            AllOlympiads();
            OlympiadList.ItemsSource = olimparr;
            List<string> items = new List<string>() { "Все года", "2025", "2024", "2023" };
            YearComboBox.ItemsSource = items;
            YearComboBox.SelectedIndex = 0;
        }

        public void LoadItems()
        {
            if (Properties.Settings.Default.UserRole == 0)
            {
                TutorRadioButton.Visibility = Visibility.Collapsed;
                ParticipantRadioButton.Visibility = Visibility.Collapsed;
                AllOlympiadsRadio.Visibility = Visibility.Collapsed;
            }
            else if(Properties.Settings.Default.UserRole == 1 || Properties.Settings.Default.UserRole == 3)
            {
                ParticipantRadioButton.Visibility = Visibility.Visible;
                AllOlympiadsRadio.Visibility = Visibility.Visible;
            }
            else if(Properties.Settings.Default.UserRole == 2)
            {
                TutorRadioButton.Visibility = Visibility.Visible;
                ParticipantRadioButton.Visibility = Visibility.Visible;
                AllOlympiadsRadio.Visibility = Visibility.Visible;
            }
        }

        public void TutorOlympiads()
        {

                olimparr.Clear();
                foreach (var item in db.context.Olympiads.Where(x => x.ResponsibleTeacherUserId == Properties.Settings.Default.UserId).ToList())
                {
                    olimparr.Add(item);
                }
                YearSortOlimpiads();
            

        }


        public void AllOlympiads()
        {
            olimparr.Clear();
            foreach (var item in db.context.Olympiads.ToList())
            {
                olimparr.Add(item);
            }
            YearSortOlimpiads();
        }

        public void UserOlympiads()
        {
            olimparr.Clear();
            List<int> olympiadIds = registrationsarr.Select(x => x.OlympiadId).ToList();
            foreach (var item in db.context.Olympiads.Where(x => olympiadIds.Contains(x.OlympiadId)).ToList())
            {
                olimparr.Add(item);
            }
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

                    olimparr = new ObservableCollection<Olympiads>(
     olimparr.Where(x => x.StartDate.Year == year).ToList()
 );
                    OlympiadList.ItemsSource = olimparr;
                    OlympiadList.Items.Refresh(); // Добавлено
                }


            }
        }



        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (NavigationService == null) return; // Добавлена проверка

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
