using MicroAC.Core.Auth;
using MicroAC.Core.Common;
using MicroAC.Core.Exceptions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Example.ResourceApi
{
    public class Startup
    {
        IConfiguration _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.core.json")
                .AddEnvironmentVariables()
                .Build();

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMicroACProblemDetails();

            services.AddRouting();

            services.AddControllers(); ;

            services.AddScoped<AccessInternal>();

            services.AddScoped(typeof(IJwtTokenHandler<AccessInternal>), typeof(JwtTokenHandler<AccessInternal>));

            services.AddSingleton<IConfiguration>(_config); 

            services.AddSingleton(typeof(IJwtTokenHandler<AccessInternal>), new JwtTokenHandler<AccessInternal>(new AccessInternal()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMicroACProblemDetails();

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
