using Olympiad.Controllers;
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
    /// Логика взаимодействия для AddOlimpiadPage.xaml
    /// </summary>
    public partial class AddOlimpiadPage : Page
    {
        Core db = new Core();
        List<Users> users = new List<Users>();  
        OlympiadsController olympiadsController = new OlympiadsController();
        string name;
        DateTime startdate;
        DateTime enddate;

        public AddOlimpiadPage()
        {
            users = db.context.Users.Where(x => x.RoleType == 2).ToList();
            InitializeComponent();
            TeacherComboBox.ItemsSource = users;
            TeacherComboBox.DisplayMemberPath = "FIO";
            TeacherComboBox.SelectedValuePath = "UserId";
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                int.TryParse(TeacherComboBox.SelectedValue.ToString(), out int teacherId);
                name = NameTextBox.Text;
                startdate = StartDatePicker.SelectedDate.Value;
                enddate = EndDatePicker.SelectedDate.Value;

                if (olympiadsController.CheckNewOlimpiad(name, teacherId, startdate, enddate))
                {
                    olympiadsController.AddNewOlimpiad(name, teacherId, startdate, enddate);
                }
            }
            catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }
           
        }
    }
}
