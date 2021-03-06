using System;
using System.Collections.Generic;

#nullable disable

namespace MicroAC.Persistence.Entities
{
    public partial class User
    {
        public User()
        {
            UsersRoles = new HashSet<UsersRole>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Organisation { get; set; }
        public bool Blocked { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] Salt { get; set; }

        public virtual Organisation OrganisationNavigation { get; set; }
        public virtual ICollection<UsersRole> UsersRoles { get; set; }
    }
}
