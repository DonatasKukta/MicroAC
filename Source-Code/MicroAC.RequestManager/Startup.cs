using System.Net.Http;

using MicroAC.Core.Client;
using MicroAC.Core.Common;
using MicroAC.Core.Exceptions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicroAC.RequestManager
{
    public class Startup
    {
        IConfiguration _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMicroACProblemDetails();

            services.AddCors();

            services.AddRouting();

            services.AddControllers();

            services.AddSingleton<HttpClient>(new HttpClient());

            services.AddSingleton<IConfiguration>(_config);

            services.AddSingleton<IEndpointResolver, FabricEndpointResolver>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseMicroACProblemDetails();

            //TODO: Apply CORS policies
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .WithExposedHeaders(
                    _config.GetValue<string>("Timestamp:Header"), 
                    "X-ServiceFabricRequestId"));

            if (_config.GetValue<bool>("Timestamp:Enabled"))
            {
                app.UseMiddleware<TimestampMiddleware>();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
