using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

using MicroAC.Core.Persistence;

using Microsoft.EntityFrameworkCore;

using Domain = MicroAC.Core.Models;
using DTO = MicroAC.Persistence.Entities;

namespace MicroAC.Persistence
{
    public class PermissionsRepository : IPermissionsRepository
    {
        private readonly MicroACContext Context;

        public PermissionsRepository(MicroACContext context)
        {
            Context = context;
        }

        public async Task<IEnumerable<Domain.Permission>> GetRolePermissions(IEnumerable<string> roles) => 
            await Context.RolesPermissions.Where(rp => roles.Contains(rp.Role))
                                    .Select(rp => Map(rp.PermissionNavigation))
                                    .ToListAsync();


        //TODO: Implement Automapping
        private static Domain.Permission Map(DTO.Permission permission)
        {
            return new Domain.Permission
            {
                Action = permission.Action,
                Value = permission.Value,
                ServiceName = permission.ServiceName,
            };
        }
    }
}
