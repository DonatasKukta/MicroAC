using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MicroAC.Core.Persistence;

using Microsoft.EntityFrameworkCore;

using Domain = MicroAC.Core.Models;
using DTO = MicroAC.Persistence.Entities;

namespace MicroAC.Persistence
{
    public class UsersRepository : IUsersRepository
    {
        readonly MicroACContext _context;

        readonly IPasswordHandler _passwordHandler;

        public UsersRepository(MicroACContext context, IPasswordHandler passwordHandler)
        {
            _context = context;
            _passwordHandler = passwordHandler;
        }

        public async Task<Domain.User> GetUser(string email, string password)
        {
            var user = await _context.Users.Where(u => u.Email == email)
                                          .FirstOrDefaultAsync();
            if (!_passwordHandler.ConfirmPassword(user, password))
                return null;

            var roles = await GetUserRoles(user.Id);

            return Map(user, roles);
        }

        public async Task<Domain.User> GetUser(Guid guid)
        {
            var user = _context.Users.Where(u => u.Id == guid).FirstOrDefault();

            var roles = await GetUserRoles(user.Id);

            return Map(user, roles);
        }

        private async Task<IEnumerable<string>> GetUserRoles(Guid userId) => 
            await _context.UsersRoles.Where(ur => ur.User == userId)
                                    .Select(ur => ur.Role)
                                    //.AsNoTracking()
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
