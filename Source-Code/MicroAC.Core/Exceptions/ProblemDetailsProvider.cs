using System.Net;

using Hellang.Middleware.ProblemDetails;

using MicroAC.Core.Common;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace MicroAC.Core.Exceptions
{
    public static class ProblemDetailsProvider
    {
        public static IServiceCollection AddMicroACProblemDetails(this IServiceCollection services)
        {
            services.AddProblemDetails(options =>
            {
                options.Map<AuthenticationFailedException>(exception => new ProblemDetails
                {
                    Title = "Request authentication failed.",
                    Detail = exception.Message,
                    Status = (int)HttpStatusCode.Unauthorized
                });
                options.Map<AuthorizationFailedException>(exception => new ProblemDetails
                {
                    Title = "Request authorization failed.",
                    Detail = exception.Message,
                    Status = (int)HttpStatusCode.Forbidden
                });
                options.OnBeforeWriteDetails = (context, problemDetails) => 
                {
                    context.AddActionMessage("MicroAC-Timestamp", "ErrorMiddleware", $"Error {problemDetails.Status}");
                };
            });
            return services;
        }

        public static void UseMicroACProblemDetails(this IApplicationBuilder app)
        {
            app.UseProblemDetails();
        }
    }
}
