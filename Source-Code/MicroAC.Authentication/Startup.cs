using MicroAC.Core.Auth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroAC.Authentication
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
            services.AddControllers();

            services.AddSingleton(typeof(IJwtTokenHandler<AccessExternal>), new JwtTokenHandler<AccessExternal>(new AccessExternal()));
            services.AddSingleton(typeof(IJwtTokenHandler<RefreshExternal>), new JwtTokenHandler<RefreshExternal>(new RefreshExternal()));
            services.AddSingleton(typeof(IClaimBuilder<AccessExternal>), new ClaimBuilder<AccessExternal>(new AccessExternal()));
            services.AddSingleton(typeof(IClaimBuilder<RefreshExternal>), new ClaimBuilder<RefreshExternal>(new RefreshExternal()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
