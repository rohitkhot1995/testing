using Gaming.Predictor.Contracts.Configuration;
using Gaming.Predictor.Library.Dependency;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using System.IO;
using System;

namespace Gaming.Predictor.API
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

            //User-defined extension method
            services.AddServices(Configuration);

            services.AddCors();
            services.AddMvc().AddNewtonsoftJson();

            //services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddSwagger();
            //services.ConfigureSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            //    {
            //        Title = "Gaming.Predictor.API",
            //        Version = "v1",

            //    });
            //    var xmlFile = "Gaming.Predictor.API.xml";
            //    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            //    c.IncludeXmlComments(xmlPath);

            //});

            //services.AddSwaggerGen();
			
            //services.AddSwaggerGen(options =>
            //{
            //    options.DocumentFilter<SwaggerDocumentFilter>();
            //});
           
            
            //services.AddControllers().AddNewtonsoftJson(options =>
            //{
            //    // Use the default property (Pascal) casing
            //    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                

            //});
            services.AddControllers().AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
            } );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime,
            Interfaces.Connection.IRedis redis, IOptions<Application> appSettings)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            //else
            //    app.UseHsts();

            //app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions() { RequestPath = "/services" });
            appLifetime.RegisterRedis(redis, appSettings);
            app.RegisterSwagger(env, appSettings);
            //Added CORS to work on localhost
            //app.UseCors(options => options.WithOrigins("https://localhost:3000").AllowAnyMethod().AllowCredentials());

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            //app.UseMvc();

			app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }

}