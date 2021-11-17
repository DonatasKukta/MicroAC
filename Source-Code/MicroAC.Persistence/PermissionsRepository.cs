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

        public IEnumerable<Domain.Permission> GetRolePermissions(string role)
        {
            var result = Context.RolePermissions.Where(rp => rp.Role.Equals(role)).Select(rp => Map(rp.PermissionNavigation));
            return result;
        }

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
