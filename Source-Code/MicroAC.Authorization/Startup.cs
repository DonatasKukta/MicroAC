using MicroAC.Core.Auth;
using MicroAC.Core.Common;
using MicroAC.Core.Persistence;
using MicroAC.Persistence;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MicroAC.Authorization
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
            services.AddRouting();
            services.AddControllers();

            services.AddSingleton(typeof(IJwtTokenHandler<AccessExternal>), new JwtTokenHandler<AccessExternal>(new AccessExternal()));
            services.AddSingleton(typeof(IJwtTokenHandler<AccessInternal>), new JwtTokenHandler<AccessInternal>(new AccessInternal()));
            services.AddSingleton(typeof(IClaimBuilder<AccessInternal>), new ClaimBuilder<AccessInternal>(new AccessInternal()));

            services.AddScoped<IPermissionsRepository, PermissionsRepository>();

            services.AddSingleton<IConfiguration>(_config);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
