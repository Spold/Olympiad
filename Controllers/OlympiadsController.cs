using Olympiad.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Olympiad.Controllers
{
    public class OlympiadsController
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
                throw new Exception("Дата начала олимпиады не может быть позже даты окончания.");
            }

            return true;
        }
        public bool CheckNewOlimpiad(int id, string name, int teacher, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Название олимпиады не веденно");
            }

            if (db.context.Olympiads.Any(x => x.Name == name && x.OlympiadId != id))
            {
                throw new Exception("Олимпиада с таким названием уже существует");
            }

            if (teacher == 0)
            {
                throw new Exception("Ответсвенный преподаватель не выбран");
            }


            if (startDate > endDate)
            {
                throw new Exception("Дата начала олимпиады не может быть позже даты окончания.");
            }

            return true;
        }


        public int AddNewOlimpiad(string name, int teacher, DateTime startDate, DateTime endDate)
        {
            Olympiads olympiads = new Olympiads
            {
                Name = name,
                ResponsibleTeacherUserId = teacher,
                StartDate = startDate,
                EndDate = endDate
            };
           
            db.context.Olympiads.Add(olympiads);
            return db.context.SaveChanges();

            
        }

        public int UpdateOlympiad(int olympiadId,
                         string name,
                         int teacherId,
                         DateTime startDate,
                         DateTime endDate)
        {
            try
            {
                var existingOlympiad = db.context.Olympiads
                    .FirstOrDefault(o => o.OlympiadId == olympiadId);

                if (existingOlympiad == null)
                {
                    throw new Exception("Олимпиада не найдена!");
                }

                existingOlympiad.Name = name;
                existingOlympiad.ResponsibleTeacherUserId = teacherId;
                existingOlympiad.StartDate = startDate;
                existingOlympiad.EndDate = endDate;

                return db.context.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении: {ex.Message}");
                return 0;
            }
        }


        public bool UpdateOlympiadData(int olympiadId,
        string positionDocument,
        string tasksArchive,
        string protocolFilePath,
        string protocolStatus)
        {

                var olympiad = db.context.Olympiads
                        .Include("Protocols")
                        .FirstOrDefault(x => x.OlympiadId == olympiadId);

                if (olympiad == null)
                    throw new Exception("Олимпиада не найдена");

                olympiad.PositionDocument = positionDocument;
                olympiad.TasksArchive = tasksArchive;

                var protocol = olympiad.Protocols.FirstOrDefault();

                if (protocol != null && protocol.IsPublished)
                {
                    MessageBox.Show("Протокол опубликован. Редактирование пути и статуса запрещено");
                }
                else
                {

                    if (protocol != null)
                    {
                        protocol.FilePath = protocolFilePath;
                        protocol.Status = protocolStatus;
                    }
                    else
                    {
                        olympiad.Protocols.Add(new Protocols
                        {
                            FilePath = protocolFilePath,
                            Status = protocolStatus,
                            IsPublished = false
                        });
                    }
                }

                db.context.SaveChanges();
                MessageBox.Show("Данные успешно обновлены");
                return true;
        }


        public List<Olympiads> LoadAllOlympiads()
        {
            return db.context.Olympiads.ToList();
        }

        public Olympiads LoadOlympiadsAndProtocols(int olympiadId)
        {

            if ( olympiadId <= 0 ) {
                throw new Exception("Олимпиада не найдена");
            }


            Olympiads olympiads = db.context.Olympiads.Include("Protocols").FirstOrDefault(x => x.OlympiadId == olympiadId);

            if (olympiads == null)
            {
                throw new Exception("Олимпиада не найдена");
            }

            return olympiads;
        }
    }
}
