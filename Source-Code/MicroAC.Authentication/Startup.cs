using MicroAC.Core.Auth;
using MicroAC.Core.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MicroAC.Persistence;
using Microsoft.Extensions.Configuration;
using MicroAC.Core.Common;
using MicroAC.Persistence.DbDTOs;
using Microsoft.EntityFrameworkCore;

namespace MicroAC.Authentication
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
            services.AddControllers()
                    .AddNewtonsoftJson();
            //services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddScoped<AccessExternal>();
            services.AddScoped<RefreshExternal>();

            services.AddScoped(typeof(IJwtTokenHandler<AccessExternal>), typeof(JwtTokenHandler<AccessExternal>));
            services.AddScoped(typeof(IJwtTokenHandler<RefreshExternal>), typeof(JwtTokenHandler<RefreshExternal>));

            services.AddScoped(typeof(IClaimBuilder<AccessExternal>), typeof(ClaimBuilder<AccessExternal>));
            services.AddScoped(typeof(IClaimBuilder<RefreshExternal>), typeof(ClaimBuilder<RefreshExternal>));

            services.AddScoped<IUsersRepository, UsersRepository>();

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
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = string.Empty;
                });
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
