using System;
using System.Collections.Generic;

#nullable disable

namespace MicroAC.Persistence.Entities
{
    public partial class Role
    {
        public Role()
        {
            RolesPermissions = new HashSet<RolesPermission>();
            UsersRoles = new HashSet<UsersRole>();
        }

        public string Name { get; set; }

        public virtual ICollection<RolesPermission> RolesPermissions { get; set; }
        public virtual ICollection<UsersRole> UsersRoles { get; set; }
    }
}
