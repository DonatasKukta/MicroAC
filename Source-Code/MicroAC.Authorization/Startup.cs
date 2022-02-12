using MicroAC.Core.Auth;
using MicroAC.Core.Common;
using MicroAC.Core.Persistence;
using MicroAC.Persistence;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MicroAC.Authorization
{
    public class Startup
    {
        IConfiguration _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.persistance.json")
                .AddEnvironmentVariables()
                .Build();

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
            services.AddControllers();

            services.AddScoped<AccessExternal>();
            services.AddScoped<AccessInternal>();

            services.AddScoped(typeof(IClaimBuilder<AccessInternal>), typeof(ClaimBuilder<AccessInternal>));

            services.AddScoped(typeof(IJwtTokenHandler<AccessExternal>), typeof(JwtTokenHandler<AccessExternal>));
            services.AddScoped(typeof(IJwtTokenHandler<AccessInternal>), typeof(JwtTokenHandler<AccessInternal>));

            services.AddScoped<IPermissionsRepository, PermissionsRepository>();

            services.AddSingleton<IConfiguration>(_config);

            services.AddDbContext<MicroACContext>(
                options => options.UseSqlServer(_config.GetConnectionString("MicroACDatabase")));
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
