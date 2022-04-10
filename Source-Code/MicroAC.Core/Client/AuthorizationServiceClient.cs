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

        readonly IEndpointResolver EndpointResolver;

        public AuthorizationServiceClient(
            HttpClient httpClient, 
            IJwtTokenHandler<AccessInternal> tokenHandler,
            IEndpointResolver endpointResolver)
        {
            HttpClient = httpClient;
            TokenHandler = tokenHandler;
            EndpointResolver = endpointResolver;
        }

        public async Task<(IEnumerable<Permission> permissions, IEnumerable<string> timestamps)> Authorize(string externalAccessToken)
        {
            var authorizationUrl = EndpointResolver.GetServiceEndpoint(MicroACServices.Authorization);
            
            using var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(authorizationUrl + "/Authorize"),
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
