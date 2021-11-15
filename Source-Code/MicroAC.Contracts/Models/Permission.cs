using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroAC.Core.Models
{
    public partial class Permission
    {
        public string Action { get; set; }
        public string Value { get; set; }
        public string ServiceName { get; set; }
    }
}
