using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroAC.Core.Models
{
    public class User
    {
        public Guid   Id            { get; set; }
        public string Name          { get; set; }
        public string Surname       { get; set; }
        public string Email         { get; set; }
        public string Phone         { get; set; }
        public string Organisation  { get; set; }
        public bool   IsBlocked     { get; set; }

        public IEnumerable<string> Roles;
    }
}
