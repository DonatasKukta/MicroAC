using MicroAC.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroAC.Core.Persistence
{
    public interface IPermissionsRepository
    {
        public Task<IEnumerable<Permission>> GetRolePermissions(IEnumerable<string> role);
    }
}
