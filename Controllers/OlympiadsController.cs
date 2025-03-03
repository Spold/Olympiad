using Olympiad.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Olympiad.Controllers
{
    internal class OlympiadsController
    {
        Core db = new Core();
        public bool CheckNewOlimpiad(string name, int teacher, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Название олимпиады не веденно");
            }

            if (db.context.Olympiads.Any(x => x.Name == name))
            {
                throw new Exception("Олимпиада с таким названием уже существует");
            }

            if (teacher == 0)
            {
                throw new Exception("Ответсвенный преподаватель не выбран");
            }


            if (startDate > endDate)
            {
                throw new ArgumentException("Дата начала олимпиады не может быть позже даты окончания.");
            }

            return true;
        }

        public void AddNewOlimpiad(string name, int teacher, DateTime startDate, DateTime endDate)
        {
            Olympiads olympiads = new Olympiads
            {
                Name = name,
                ResponsibleTeacherUserId = teacher,
                StartDate = startDate,
                EndDate = endDate
            };
           
            db.context.Olympiads.Add(olympiads);
            db.context.SaveChanges();

            MessageBox.Show($"Олимпиада добавлена");
        }
    }
}
