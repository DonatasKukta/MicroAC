using System;
using System.Collections.Generic;

#nullable disable

namespace MicroAC.Persistence.DbDTOs
{
    public partial class Service
    {
        public Service()
        {
            Permissions = new HashSet<Permission>();
        }

        public string Name { get; set; }

        public virtual ICollection<Permission> Permissions { get; set; }
    }
}
