using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympiad.Model
{
    public partial class Users
    {
        public string FIO => LastName + " " + FirstName[0] + "." + Patronymic[0] + ". ";
    }

    public partial class ParticipantInfo
    {
        public int RegistrationId { get; set; }
        public string FIO { get; set; }
        public int? Score { get; set; }
        public string Result { get; set; }
    }

    public partial class ResultUser
    {
        public int RegistrationId { get; set; }
        public string Name { get; set; }
        public int? Score { get; set; }
        public string Result { get; set; }
    }
}
