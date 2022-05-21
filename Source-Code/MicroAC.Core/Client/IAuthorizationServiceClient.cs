using System.Collections.Generic;
using System.Threading.Tasks;

using MicroAC.Core.Models;

namespace MicroAC.Core.Client
{
    public interface IAuthorizationServiceClient
    {
        Task<(IEnumerable<Permission> permissions,
              IEnumerable<string> roles, 
              IEnumerable<string> timestamps, 
              string internalAccessToken)> 
                   Authorize(string externalAccessToken);
    }
}
