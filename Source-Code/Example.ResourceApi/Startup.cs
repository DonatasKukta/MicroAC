using System.Threading;

using MicroAC.Core.Auth;
using MicroAC.Core.Common;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Example.ResourceApi
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
            services.AddSingleton<IConfiguration>(_config); 
            services.AddSingleton(typeof(IJwtTokenHandler<AccessInternal>), new JwtTokenHandler<AccessInternal>(new AccessInternal()));
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
            /*
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/Action", async context =>
                {
                    Thread.Sleep(1000);
                    await context.Response.WriteAsync("Hello World! 1s long response.");
                });
            });*/
        }
    }
}
