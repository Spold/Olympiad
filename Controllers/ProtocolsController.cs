using Olympiad.Model;
using Olympiad.Model.PartialClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Olympiad.Controllers
{
    public class ProtocolsController
    {

        Core db = new Core();
       public bool PublishProtocol(int id)
        {
            var protocol = db.context.Protocols
                      .FirstOrDefault(p => p.ProtocolId == id);

            if (protocol != null)
            {
                protocol.IsPublished = true;
                db.context.SaveChanges();
                MessageBox.Show("Протокол успешно опубликован");
                return true;
            }

            return false;
        }

        public List<ProtocolViewModel> LoadPreparedProtocols()
        {
            return db.context.Protocols
                .Where(p => !p.IsPublished && p.Status == "prepared")
                .Join(db.context.Olympiads,
                p => p.OlympiadId,
                o => o.OlympiadId,
                (p, o) => new ProtocolViewModel
                {
                    ProtocolId = p.ProtocolId,
                    Name = o.Name,
                    Status = p.Status,
                    FilePath = p.FilePath
                })
                .ToList();
        }

        public int GetOrCreateProtocolId(int olympiadId)
        {
            var protocol = db.context.Protocols
               .FirstOrDefault(p => p.OlympiadId == olympiadId);

            if (protocol == null)
            {
                protocol = new Protocols
                {
                    OlympiadId = olympiadId,
                    Status = "draft",
                    FilePath = null,
                    IsPublished = false
                };
                db.context.Protocols.Add(protocol);
                db.context.SaveChanges();
            }

            return protocol.ProtocolId;
        }
    }
}
