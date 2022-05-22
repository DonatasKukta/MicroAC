using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.DependencyInjection;
using MicroAC.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using MicroAC.Core.Auth;
using System.Threading.Tasks;
using MicroAC.Core.Constants;
using MicroAC.Core.Common;
using MicroAC.Core.Exceptions;

namespace MicroAC.Core.Client
{
    public class MicroAuthAttribute : ActionFilterAttribute
    {
        public string ServiceName { get; set; }
        public string Action { get; set; }
        public string Value { get; set; }

        public override async Task OnActionExecutionAsync(
            ActionExecutingContext filterContext,
            ActionExecutionDelegate next)
        {
            var httpContext = filterContext.HttpContext;
            var config = httpContext.RequestServices.GetService<IConfiguration>();
            var permissions = config.GetValue<bool>(ConfigKeys.CentralAuthorizationEnabled)
                ? GetPermissionsFromHeader(httpContext)
                : await RetrievePermissionsFromAuthorizationService(httpContext);

            httpContext.Items.Add(HttpContextKeys.Permissions, permissions);

            if (!config.GetValue<bool>(ConfigKeys.StrictAuthorizationEnabled)
                && httpContext.GetValidatedRoles().Any(r => r.Contains("SeedTestData")))
            {
                await next();
                return;
            }

            bool serviceFilter(Permission permission) => permission.ServiceName.Equals(ServiceName);
            bool actionFilter(Permission permission) => permission.Action.Equals(Action);
            bool valueFilter(Permission permission) => permission.Value.Equals(Value);

            if (!string.IsNullOrEmpty(ServiceName))
                permissions = Filter(permissions, serviceFilter);

            if (!string.IsNullOrEmpty(Action))
                permissions = Filter(permissions, actionFilter);

            if (!string.IsNullOrEmpty(Value))
                permissions = Filter(permissions, valueFilter);

            await next();
        }

        IEnumerable<Permission> GetPermissionsFromHeader(HttpContext httpContext)
        {
            var internalAccessToken = GetToken(HttpHeaders.InternalJWT, httpContext);

            var tokenHandler = httpContext.RequestServices.GetService<IJwtTokenHandler<AccessInternal>>();
            var validatedPermissions = tokenHandler.GetValidatedPermissions(internalAccessToken);
            var roles = tokenHandler.GetValidatedRoles(internalAccessToken);

            httpContext.Items.Add(HttpContextKeys.InternalAccessToken, internalAccessToken);
            httpContext.Items.Add(HttpContextKeys.Roles, roles);

            return validatedPermissions;
        }

        async Task<IEnumerable<Permission>> RetrievePermissionsFromAuthorizationService(HttpContext httpContext)
        {
            httpContext.RequestServices.GetService<IEndpointResolver>().InitialiseEndpoints();
            var authorization = httpContext.RequestServices.GetService<IAuthorizationServiceClient>();

            var externalAccessToken = GetToken(HttpHeaders.Authorization, httpContext);

            (var permissions, var roles, var timestamps, var internalAccessToken) = await authorization.Authorize(externalAccessToken);

            httpContext.Response.Headers.Append(HttpHeaders.Timestamps, timestamps.ToArray());
            httpContext.Items.Add(HttpContextKeys.InternalAccessToken, internalAccessToken);
            httpContext.Items.Add(HttpContextKeys.Roles, roles);

            return permissions;
        }

        static string GetToken(string header, HttpContext httpContext)
        {
            var containsToken = httpContext.Request.Headers.TryGetValue(header, out var tokenString);

            if (!containsToken || tokenString == StringValues.Empty)
                throw new UnauthorizedAccessException($"Authorization token {header} not provided");

            return tokenString;
        }

        static IEnumerable<Permission> Filter(IEnumerable<Permission> permissions, Func<Permission, bool> filter)
        {
            var filtered = permissions.Where(filter);

            return filtered.Any()
                ? filtered
                : throw new AuthorizationFailedException();
        }
    }
}
