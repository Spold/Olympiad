using Olympiad.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Olympiad.Controllers
{
    public class CertificatesController
    {
        Core db = new Core();

        public List<Certificates> LoadAllCertificates(int _olympiadId)
        {
            return db.context.Certificates
                .Where(x => x.OlympiadId == _olympiadId)
                .ToList();

        }

        public bool CheckNewCertificate(int olimpid, string filepath, string desc)
        {
            if (olimpid <= 0)
            {
                throw new Exception("Олимпиада не найдена");
            }

            if (string.IsNullOrEmpty(filepath))
            {
                throw new Exception("Сыылка на файл не введена");
            }

            if (string.IsNullOrEmpty(desc))
            {
                throw new Exception("Описание не заполнено");
            }

            return true;
        }

        public int AddNewCertificate(int olimpid, string filepath, string desc)
        {
            Certificates certificates = new Certificates
            {
                OlympiadId = olimpid,
                FilePath = filepath,
                Description = desc
            };

            db.context.Certificates.Add(certificates);
            return db.context.SaveChanges();
           
        }

        public bool UpdateDataCertificate(int sertid, int olimpid, string filepath, string desc)
        {
            Certificates certificate = db.context.Certificates.Where(x => x.CertificateId == sertid).FirstOrDefault();

            if (certificate == null)
            {
                AddNewCertificate(olimpid, filepath, desc);
                MessageBox.Show("Сертификат добавлен");
                return true;
            }
            else
            {
                certificate.FilePath = filepath;
                certificate.Description = desc;
                db.context.SaveChanges();
                MessageBox.Show("Сертификат обновлен");
                return true;
            }

        }


    }
}
