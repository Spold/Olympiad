using Olympiad.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Data.Entity;
using System.Windows;

namespace Olympiad.Controllers
{
    internal class RegistrationsController
    {
        Core db = new Core();
        List<Registrations> registrations = new List<Registrations>();
        ProtocolsController protocolsController = new ProtocolsController();

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

        public bool SaveResults(List<ParticipantInfo> updatedParticipants)
        {
            if (updatedParticipants == null)
            {
                MessageBox.Show("Нет данных для сохранения.");
                return false;
            }

            foreach (var participant in updatedParticipants)
            {
                var registration = db.context.Registrations
                    .Include(r => r.Results)
                    .FirstOrDefault(r => r.RegistrationId == participant.RegistrationId);

                if (registration == null)
                {
                    MessageBox.Show($"Регистрация с ID {participant.RegistrationId} не найдена.");
                    continue;
                }

                var result = registration.Results.FirstOrDefault();
                if (result == null)
                {
                    result = new Results
                    {
                        RegistrationId = participant.RegistrationId,
                        ProtocolId = protocolsController.GetOrCreateProtocolId(registration.OlympiadId)
                    };
                    db.context.Results.Add(result);
                }

                result.Score = participant.Score ?? 0;
                result.ResultType = participant.Result;
            }

            db.context.SaveChanges();
            return true;
        }
    }
}
