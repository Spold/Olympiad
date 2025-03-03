using Olympiad.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Data.Entity;

namespace Olympiad.Controllers
{
    internal class RegistrationsController
    {
        Core db = new Core();
        List<Registrations> registrations = new List<Registrations>();

        public bool CheckUserRegistration(int userId, int olympiadId)
        {
            if (userId == 0 || olympiadId == 0)
            {
                throw new ArgumentException("Пользователь или олимпиада не найдена");
            }


            bool isRegistrationExists = db.context.Registrations
                .Any(r => r.StudentUserId == userId && r.OlympiadId == olympiadId);

            return !isRegistrationExists;
        }

        public void RegistrationOnOlimpiad(int userId, int olimpiadId)
        {
            Registrations newRegist = new Registrations()
            {
               StudentUserId = userId,
               OlympiadId = olimpiadId,
               RegistrationDate = DateTime.Now,
            };


            db.context.Registrations.Add(newRegist);
            db.context.SaveChanges();

        }

        public List<ParticipantInfo> ProtocolsEdit(int olympiadId)
        {
            var participants = db.context.Registrations
                  .Include(r => r.Users)
                  .Include(r => r.Results)
                  .Where(r => r.OlympiadId == olympiadId)
                  .Select(r => new ParticipantInfo
                  {
                      RegistrationId = r.RegistrationId,
                      FIO = r.Users.FirstName + " " + r.Users.LastName + " " + r.Users.Patronymic,
                      Score = r.Results.FirstOrDefault() != null ? r.Results.FirstOrDefault().Score : 0,
                      Result = r.Results.FirstOrDefault().ResultType ?? "Participant"
                  })
                  .ToList();

            return participants;
        }
    }
}
