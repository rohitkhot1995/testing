using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Gaming.Predictor.Library.Dependency;
using Gaming.Predictor.Contracts.Configuration;
using System.IO;

namespace Gaming.Predictor.Daemon
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
               .UseEnvironment(EnvironmentName.Production)//Explicitly setting to Production
               .ConfigureAppConfiguration((hostContext, config) =>
               {
                   var env = hostContext.HostingEnvironment;

                   config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                   //.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                   config.SetBasePath(env.ContentRootPath);
                   config.AddEnvironmentVariables(prefix: "ASPNETCORE_");

                   if (args != null)
                   {
                       config.AddCommandLine(args);
                   }
               })
               .ConfigureServices((hostContext, services) =>
               {
                   services.AddOptions();
                   services.AddServices(hostContext.Configuration);
                   services.Configure<Contracts.Configuration.Daemon>(hostContext.Configuration.GetSection("Daemon"));

                   //Background Services
                   services.AddHostedService<BackgroundServices.PeriodicUpdate>();
                   services.AddHostedService<BackgroundServices.GameLocking>();
                   services.AddHostedService<BackgroundServices.MatchAnswerCalculation>();



                   //services.AddHostedService<BackgroundServices.PointsCalculation>();
                   //services.AddHostedService<BackgroundServices.PeriodicQuestionsUpdate>();

                   //services.AddHostedService<BackgroundServices.Abandon>();
                   //services.AddHostedService<BackgroundServices.Analytics>();

                   //CloudWatch logs
                   ILoggerFactory loggerFactory = new LoggerFactory();
                   loggerFactory.UseCloudWatch(hostContext.Configuration);
                   loggerFactory.CreateLogger<Program>();
                   services.AddSingleton(loggerFactory);
               });

            await builder.RunConsoleAsync();
        }
    }
}
