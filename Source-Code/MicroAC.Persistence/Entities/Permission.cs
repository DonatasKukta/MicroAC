using System;
using System.Collections.Generic;

#nullable disable

namespace MicroAC.Persistence.Entities
{
    public partial class Permission
    {
        public Permission()
        {
            RolesPermissions = new HashSet<RolesPermission>();
        }

        public Guid Id { get; set; }
        public string Action { get; set; }
        public string Value { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }

        public virtual Service ServiceNameNavigation { get; set; }
        public virtual ICollection<RolesPermission> RolesPermissions { get; set; }
    }
}
