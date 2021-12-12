using System;
using System.Collections.Generic;
using System.Linq;

using MicroAC.Core.Persistence;

using Domain = MicroAC.Core.Models;
using DTO = MicroAC.Persistence.DbDTOs;

namespace MicroAC.Persistence
{
    public class UsersRepository : IUsersRepository
    {
        private static DTO.MicroACContext Context;

        public UsersRepository()
        {
            //TODO: Implement DI
            Context = new DTO.MicroACContext();
        }

        public Domain.User GetUser(string email, string password)
        {
            var user = Context.Users.Where(u => u.Email == email).FirstOrDefault();
            var roles = GetUserRoles(user.Id);

            return Map(user, roles);
        }

        public Domain.User GetUser(Guid guid)
        {
            var user = Context.Users.Where(u => u.Id == guid).FirstOrDefault(); ;
            var roles = GetUserRoles(user.Id);

            return Map(user, roles);
        }

        private IEnumerable<string> GetUserRoles(Guid userId) => 
            Context.UsersRoles.Where(ur => ur.User == userId)
                              .Select(ur => ur.Role);

        //TODO: Implement Automapping
        private static Domain.User Map(DTO.User user, IEnumerable<string> roles)
        {
            return new Domain.User
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Phone = user.Phone,
                Organisation = user.Organisation,
                IsBlocked = user.Blocked,
                Roles = roles
            };
        }
    }
}
