using System;
using System.Collections.Generic;

#nullable disable

namespace MicroAC.Persistence.Entities
{
    public partial class UsersRole
    {
        public Guid User { get; set; }
        public string Role { get; set; }

        public virtual Role RoleNavigation { get; set; }
        public virtual User UserNavigation { get; set; }
    }
}
