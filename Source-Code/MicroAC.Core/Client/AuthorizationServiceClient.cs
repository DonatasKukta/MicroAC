using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using MicroAC.Core.Auth;
using MicroAC.Core.Constants;
using MicroAC.Core.Models;

namespace MicroAC.Core.Client
{
    public class AuthorizationServiceClient : IAuthorizationServiceClient
    {
        readonly HttpClient HttpClient;

        readonly IJwtTokenHandler<AccessInternal> TokenHandler;

        //TODO: Make URL dynamic with Service Fabric discovery
        const string AuthorizationURL 
            = "http://localhost:19081/MicroAC.ServiceFabric/MicroAC.Authorization/Authorize";

        public AuthorizationServiceClient(HttpClient httpClient, IJwtTokenHandler<AccessInternal> tokenHandler)
        {
            HttpClient = httpClient;
            TokenHandler = tokenHandler;
        }

        public async Task<(IEnumerable<Permission> permissions, IEnumerable<string> timestamps)> Authorize(string externalAccessToken)
        {
            using var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(AuthorizationURL),
                Method = HttpMethod.Post,
            };
            request.Headers.Add(HttpHeaders.Authorization, externalAccessToken);

            using var response = await HttpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();
            var accessInternalToken = await response.Content.ReadAsStringAsync();

            var permissions = TokenHandler.GetValidatedPermissions(accessInternalToken);
            var timestamps = response.Headers.GetValues(HttpHeaders.Timestamps);

            return (permissions, timestamps);
        }
    }
}
