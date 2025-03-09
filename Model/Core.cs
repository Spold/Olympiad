using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympiad.Model
{
    public class Core : IDisposable
    {
        public OlympiadTrackingEntities context;

        public Core()
        {
            context = new OlympiadTrackingEntities();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
