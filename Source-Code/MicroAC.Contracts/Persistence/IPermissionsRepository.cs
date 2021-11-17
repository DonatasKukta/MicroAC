using MicroAC.Core.Models;
using System.Collections.Generic;

namespace MicroAC.Core.Persistence
{
    public interface IPermissionsRepository
    {
        public IEnumerable<Permission> GetRolePermissions(string role);
    }
}
