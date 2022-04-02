using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.DependencyInjection;
using MicroAC.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace MicroAC.Core.Auth
{
    public class MicroAuthAttribute : ActionFilterAttribute
    {
        public const string PermissionsKey = "Permissions";

        public string ServiceName { get; set; }
        public string Action { get; set; }
        public string Value { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            var config = httpContext.RequestServices.GetService<IConfiguration>();

            var permissions = config.GetValue<bool>("CentralAuthorizationEnabled")
                ? GetPermissionsFromHeader(httpContext)
                : RetrievePermissionsFromAuthorizationService(httpContext);

            httpContext.Items.Add(PermissionsKey, permissions);

            if (!config.GetValue<bool>("StrictAuthozirationEnabled"))
                return;

            bool serviceFilter(Permission permission) => permission.ServiceName.Equals(ServiceName);
            bool actionFilter(Permission permission)  => permission.Action.Equals(Action);
            bool valueFilter(Permission permission)   => permission.Value.Equals(Value);


            if (!string.IsNullOrEmpty(ServiceName))
                permissions = Filter(permissions, serviceFilter);

            if (!string.IsNullOrEmpty(Action))
                permissions = Filter(permissions, actionFilter);

            if (!string.IsNullOrEmpty(Value))
                permissions = Filter(permissions, valueFilter);
        }

        IEnumerable<Permission> GetPermissionsFromHeader(HttpContext httpContext)
        {
            var containsToken = httpContext.Request.Headers.TryGetValue("MicroAC-JWT", out var tokenString);

            if (!containsToken || tokenString == StringValues.Empty)
                throw new UnauthorizedAccessException("Authorization token not provided");

            var tokenHandler = httpContext.RequestServices.GetService<IJwtTokenHandler<AccessInternal>>();
            return tokenHandler.GetValidatedPermissions(tokenString);
        }

        IEnumerable<Permission> RetrievePermissionsFromAuthorizationService(HttpContext httpContext)
        {
            // Send external request
            return null;
        }

        static IEnumerable<Permission> Filter(IEnumerable<Permission> permissions, Func<Permission, bool> filter)
        {
            var filtered = permissions.Where(filter);

            return filtered.Any()
                ? filtered
                : throw new UnauthorizedAccessException();
        }
    }
}
