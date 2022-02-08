using System;
using System.Threading.Tasks;

using MicroAC.Core.Models;

namespace MicroAC.Core.Persistence
{
    public interface IUsersRepository
    {
        Task<User> GetUser(Guid guid);

        Task<User> GetUser(string email, string password);
    }
}
