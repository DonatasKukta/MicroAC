using System;
using MicroAC.Core.Models;

namespace MicroAC.Core.Persistence
{
    public interface IUsersRepository
    {
        User GetUser(Guid guid);

        User GetUser(string email, string password);
    }
}
