using System;
using System.Collections.Generic;

#nullable disable

namespace MicroAC.Persistence.Entities
{
    public partial class Organisation
    {
        public Organisation()
        {
            Users = new HashSet<User>();
        }

        public string Name { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
