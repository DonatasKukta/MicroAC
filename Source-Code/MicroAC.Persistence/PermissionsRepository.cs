using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;

using MicroAC.Core.Persistence;

using Domain = MicroAC.Core.Models;
using DTO = MicroAC.Persistence.DbDTOs;

namespace MicroAC.Persistence
{
    public class PermissionsRepository : IPermissionsRepository
    {
        private static DTO.MicroACContext Context;

        public PermissionsRepository()
        {
            //TODO: Implement DI
            Context = new DTO.MicroACContext();
        }

        public IEnumerable<Domain.Permission> GetRolePermissions(IEnumerable<string> roles) => 
            Context.RolesPermissions.Where(rp => roles.Contains(rp.Role))
                                    .Select(rp => Map(rp.PermissionNavigation));


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
