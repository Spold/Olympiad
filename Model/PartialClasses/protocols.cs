using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympiad.Model.PartialClasses
{
    public partial class protocols
    {
    }

    public partial class ProtocolViewModel
    {
        public int ProtocolId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string FilePath { get; set; }
    }
}
