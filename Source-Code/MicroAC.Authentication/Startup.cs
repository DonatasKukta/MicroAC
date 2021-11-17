using MicroAC.Core.Auth;
using MicroAC.Core.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MicroAC.Persistence;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json.Converters;

namespace MicroAC.Authentication
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
            services.AddControllers()
                    .AddNewtonsoftJson();
            //services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddSingleton(typeof(IJwtTokenHandler<AccessExternal>), new JwtTokenHandler<AccessExternal>(new AccessExternal()));
            services.AddSingleton(typeof(IJwtTokenHandler<RefreshExternal>), new JwtTokenHandler<RefreshExternal>(new RefreshExternal()));
            services.AddSingleton(typeof(IClaimBuilder<AccessExternal>), new ClaimBuilder<AccessExternal>(new AccessExternal()));
            services.AddSingleton(typeof(IClaimBuilder<RefreshExternal>), new ClaimBuilder<RefreshExternal>(new RefreshExternal()));

            services.AddScoped< IUsersRepository, UsersRepository>();
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
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
