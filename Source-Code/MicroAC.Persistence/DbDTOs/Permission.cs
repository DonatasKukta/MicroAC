using System;
using System.Collections.Generic;

#nullable disable

namespace MicroAC.Persistence.DbDTOs
{
    public partial class Permission
    {
        public Guid Id { get; set; }
        public string Action { get; set; }
        public string Value { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }

        public virtual Service ServiceNameNavigation { get; set; }
    }
}
