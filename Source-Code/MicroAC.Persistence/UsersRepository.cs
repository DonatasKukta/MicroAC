using System;
using System.Linq;
using System.Security;

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

            return Map(user);
        }

        public Domain.User GetUser(Guid guid)
        {
            var user = Context.Users.Where(u => u.Id == guid).FirstOrDefault(); ;

            return Map(user);
        }

        //TODO: Implement Automapping
        private static Domain.User Map(DTO.User user)
        {
            return new Domain.User
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Phone = user.Phone,
                Organisation = user.Organisation,
                Role = user.Role,
                IsBlocked = user.Blocked,
            };
        }
    }
}
