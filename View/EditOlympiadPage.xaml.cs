
using Olympiad.Controllers;
using Olympiad.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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
    /// Логика взаимодействия для EditOlympiadPage.xaml
    /// </summary>
    public partial class EditOlympiadPage : Page
    {
        Core db = new Core();
        Olympiads olympiad = new Olympiads();
        List<Users> users = new List<Users>();
        OlympiadsController olympiadsController = new OlympiadsController();
        string name;
        DateTime startdate;
        DateTime enddate;
        int olympiadId;
        public EditOlympiadPage(int id)
        {
            olympiadId = id;
            users = db.context.Users.Where(x => x.RoleType == 2).ToList();
            InitializeComponent();
            TeacherComboBox.ItemsSource = users;
            olympiad = db.context.Olympiads.FirstOrDefault(x => x.OlympiadId == id);
            LoadData();
        }

        private void LoadData()
        {
            NameTextBox.Text = olympiad.Name;
            TeacherComboBox.DisplayMemberPath = "FIO";
            TeacherComboBox.SelectedValuePath = "UserId";
            TeacherComboBox.SelectedItem = db.context.Users.FirstOrDefault(x => x.UserId == olympiad.ResponsibleTeacherUserId);
            StartDatePicker.Text = olympiad.StartDate.ToString();
            EndDatePicker.Text = olympiad.EndDate.ToString();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int.TryParse(TeacherComboBox.SelectedValue.ToString(), out int teacherId);
                name = NameTextBox.Text;
                startdate = StartDatePicker.SelectedDate.Value;
                enddate = EndDatePicker.SelectedDate.Value;
                if (olympiadsController.CheckNewOlimpiad(olympiadId, name, teacherId, startdate, enddate))
                {
                    olympiadsController.UpdateOlympiad(olympiadId,name, teacherId, startdate, enddate);
                    MessageBox.Show("Данные олимпиады обновлены");
                }
            }
           catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}
