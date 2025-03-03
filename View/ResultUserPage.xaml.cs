using Olympiad.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
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
    /// Логика взаимодействия для ResultUserPage.xaml
    /// </summary>
    public partial class ResultUserPage : Page
    {
        Core db = new Core();
        public ResultUserPage(int idUser)
        {
            InitializeComponent();

            var results = db.context.Registrations
          .Include(r => r.Users)
          .Include(r => r.Results)
          .Where(r => r.StudentUserId == idUser)
          .Select(r => new ResultUser
              {
                  RegistrationId = r.RegistrationId,
                  Name = r.Olympiads.Name,
                  Score = r.Results.FirstOrDefault() != null ? r.Results.FirstOrDefault().Score : 0,
                  Result = r.Results.FirstOrDefault().ResultType ?? "Participant"
              })
          .ToList();
            ResultsGrid.ItemsSource = results;
        }
    }
}
