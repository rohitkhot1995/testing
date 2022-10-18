using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gaming.Predictor.Contracts.Configuration;
using Gaming.Predictor.Library.Dependency;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Gaming.Predictor.Admin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            //Increasing Form/Value count limit. Default by ASP.NET Core is 1024.
            //Not including the below will throw exception on Form post of values which are huge in count.
            services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
            {
                options.ValueCountLimit = int.MaxValue;
            });

            //User-defined extension method
            services.AddServices(Configuration);
            //services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddSingleton<Interfaces.Admin.ISession, App_Code.Authorization>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime,
            Interfaces.Connection.IRedis redis, IOptions<Application> appSettings)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/Error");
                //app.UseHsts();
            }

            
            //app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions() { RequestPath = "/games/predictor/admin" });
            appLifetime.RegisterRedis(redis, appSettings);

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            //app.UseMvc(routes =>
            //{
            //    //routes.MapRoute(
            //    //    name: "default",
            //    //    template: "{controller=Home}/{action=Login}/{id?}");

            //    routes.MapRoute(
            //        name: "admin",
            //        template: "admin/{action}/{id?}",
            //        defaults: new { Controller = "Home" });
            //});

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "games/predictor/admin",
                    pattern: "games/predictor/admin/{action}/{id?}",
                    defaults: new { Controller = "Home" });
            });
        }
    }
}
