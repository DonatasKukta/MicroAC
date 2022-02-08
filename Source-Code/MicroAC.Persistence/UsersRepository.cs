using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MicroAC.Core.Persistence;

using Microsoft.EntityFrameworkCore;

using Domain = MicroAC.Core.Models;
using DTO = MicroAC.Persistence.DbDTOs;

namespace MicroAC.Persistence
{
    public class UsersRepository : IUsersRepository
    {
        private readonly DTO.MicroACContext Context;

        public UsersRepository(DTO.MicroACContext context)
        {
            Context = context;
        }

        public async Task<Domain.User> GetUser(string email, string password)
        {
            var user = await Context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();

            var roles = await GetUserRoles(user.Id);

            return Map(user, roles);
        }

        public async Task<Domain.User> GetUser(Guid guid)
        {
            var user = Context.Users.Where(u => u.Id == guid).FirstOrDefault();

            var roles = await GetUserRoles(user.Id);

            return Map(user, roles);
        }

        private async Task<IEnumerable<string>> GetUserRoles(Guid userId) => 
            await Context.UsersRoles.Where(ur => ur.User == userId)
                                    .Select(ur => ur.Role)
                                    .ToListAsync();

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
