using Gaming.Predictor.Contracts.Configuration;
using Gaming.Predictor.Interfaces.Connection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace Gaming.Predictor.Library.Dependency
{
    public static class ApplicationExtension
    {
        public static IApplicationLifetime RegisterRedis(this IApplicationLifetime app, IRedis redis, IOptions<Application> appSettings)
        {
            if (appSettings.Value.Connection.Redis.Apply)
            {
                app.ApplicationStarted.Register(redis.RedisConnectMultiplexer);
                app.ApplicationStopped.Register(redis.RedisConnectDisposer);
            }

            return app;
        }

        public static IApplicationBuilder RegisterSwagger(this IApplicationBuilder app, IHostingEnvironment env,
                                                    IOptions<Application> appSettings)
        {
            app.UseSwagger();

            string swaggerConfig = appSettings.Value.CustomSwaggerConfig.BasePath + "/services/config/swagger/Gaming.Predictor.API.json";
            //string swaggerConfig = "config/swagger/Gaming.Predictor.API.json";

            if (env.IsDevelopment())
                swaggerConfig = "/swagger/v1/swagger.json";

            app.UseSwaggerUI(c =>
            {
                //#TOREMEMBER
                //reading swagger json from website's directory location.
                c.SwaggerEndpoint(swaggerConfig, "Gaming.Predictor.API V1");
                c.RoutePrefix = "services";
            });

            return app;
        }
    }
}