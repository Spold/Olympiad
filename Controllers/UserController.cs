using Olympiad.Model;
using Olympiad.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace Olympiad.Controllers
{
    internal class UserController
    {
        Core db = new Core();
        public bool CheckLogin(string login, string password)
        {
            if (string.IsNullOrEmpty(login))
            {
                throw new Exception("Вы не ввели логин");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new Exception("Вы не ввели пароль");
            }

            if (!db.context.Users.Any(x => x.Login == login)) {
                throw new Exception("Пользователь не найден");
            }
            else
            {
                
                return true;
            }
        }

        public void LoginUser(string login, string password)
        {
            Users user = db.context.Users.Where(x => x.Login == login).FirstOrDefault();

            if (login == user.Login && password == user.PasswordHash)
            {
                Properties.Settings.Default.UserId = user.UserId;
                Properties.Settings.Default.UserRole = user.RoleType;
                Properties.Settings.Default.Save();
            }
            else
            {
                throw new Exception("Не верный пароль");
            }
        }

        public bool CheckNewUser(string login, string password, int roletype, string firstname, string lastname, string patronymic, string email, DateTime? BirthDate,
                                   string educationalInstitution, string educationLevel, int coursenumber, string speciality)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.(?:[a-zA-Z]{2,63})$";

            if (string.IsNullOrEmpty(login))
            {
                throw new Exception("Логин не введен");
            }

            if (db.context.Users.Any(x => x.Login == login))
            {
                throw new Exception("Такой логин уже существует");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new Exception("Пароль не введен");
            }

            if (roletype == 0)
            {
                throw new Exception("Роль не введена");
            }

            if (string.IsNullOrEmpty(firstname))
            {
                throw new Exception("Имя не введено");
            }

            if (string.IsNullOrEmpty(lastname))
            {
                throw new Exception("Фамилия не введена");
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new Exception("Email не введен");
            }

            if (string.IsNullOrEmpty(educationalInstitution))
            {
                throw new Exception("Учебное заведение не введено");
            }

            if (string.IsNullOrEmpty(educationLevel))
            {
                throw new Exception("Уровень образования не введен");
            }

            if (string.IsNullOrEmpty(speciality))
            {
                throw new Exception("Специальность не введена");
            }

            if (login.Contains(" "))
            {
                throw new Exception("В логине есть пробелы");
            }

            if (password.Contains(" "))
            {
                throw new Exception("В пароле есть пробелы");
            }

            if (!Regex.IsMatch(email, pattern))
            {
                throw new Exception("Некорректный Email");
            }

            if (!Regex.IsMatch(lastname, "^[a-zA-Zа-яА-Я]+$"))
            {
                throw new Exception("Некорректная фамилия");
            }

            if (!Regex.IsMatch(firstname, "^[a-zA-Zа-яА-Я]+$"))
            {
                throw new Exception("Некорректное имя");
            }

            if (!BirthDate.HasValue)
            {
                throw new Exception("Дата рождения не выбрана");
            }

            return true;

        }

        public void AddNewUser(string login, string password, int roletype, string firstname, string lastname, string patronymic, string email, DateTime? BirthDate, 
                               string educationalInstitution, string educationLevel, int coursenumber, string speciality)
        {
            Users newUser = new Users()
            {
                Login = login,
                PasswordHash = password,
                RoleType = roletype,
                FirstName = firstname,
                LastName = lastname,
                Patronymic = patronymic,
                Email = email,
                DateOfBirth = BirthDate,
                EducationalInstitution = educationalInstitution,
                EducationLevel = educationLevel,
                CourseNumber = coursenumber,
                Specialty = speciality
            };


            MessageBox.Show($"Пользователь по имени {firstname} {lastname} добавлен, роль равна {roletype}.");
            db.context.Users.Add(newUser);
            db.context.SaveChanges();

        }
    }
}
