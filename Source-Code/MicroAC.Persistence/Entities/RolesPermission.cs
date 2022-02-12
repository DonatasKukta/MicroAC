using System;
using System.Collections.Generic;

#nullable disable

namespace MicroAC.Persistence.Entities
{
    public partial class RolesPermission
    {
        public string Role { get; set; }
        public Guid Permission { get; set; }

        public virtual Permission PermissionNavigation { get; set; }
        public virtual Role RoleNavigation { get; set; }
    }
}
