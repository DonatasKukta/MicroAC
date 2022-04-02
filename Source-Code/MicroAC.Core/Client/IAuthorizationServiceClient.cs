using System.Collections.Generic;
using System.Threading.Tasks;

using MicroAC.Core.Models;

namespace MicroAC.Core.Client
{
    public interface IAuthorizationServiceClient
    {
        Task<(IEnumerable<Permission> permissions, IEnumerable<string> timestamps)> Authorize(string externalAccessToken);
    }
}
