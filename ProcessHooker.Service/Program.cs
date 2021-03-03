using System.IO;
using Serilog;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProcessHooker.Service {
    public static class Program {
        private static void Main(string[] args) {
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
                       .UseEnvironment("Development")
                       .ConfigureServices(
                           services => {
                               services.AddHostedService<Service>();
                               services.AddSingleton<IHooksSectionParser, HooksSectionParser>();
                               services.AddSingleton<IProcessProvider, ProcessProvider>();
                               services.AddSingleton<IProcessHooksHandler, ProcessHooksHandler>();
                           }
                       );
        }

        private static IConfigurationBuilder CreateConfigurationBuilder() {
            return new ConfigurationBuilder()
                   .AddEnvironmentVariables()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", false, true)
                   .AddJsonFile($"appsettings.Development.json", true);
        }
    }
}
