using System.IO;
using Serilog;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProcessHooker.Service {
    class Program {

        static void Main(string[] args) {

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(CreateConfigurationBuilder().Build())
                .CreateLogger();

            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) {
            return Host.CreateDefaultBuilder(args)
                .UseSystemd()
                .UseSerilog()
                .UseWindowsService()
                .ConfigureServices(services => {
                    services.AddHostedService<Service>();
                    services.AddSingleton<IHooksSectionParser, HooksSectionParser>();
                });
        }

        private static IConfigurationBuilder CreateConfigurationBuilder() {
             return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true);
        }
    }
}
