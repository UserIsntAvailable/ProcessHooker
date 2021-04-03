using System.Threading.Tasks;
using FluentValidation;
using Serilog;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ProcessHooker.Service {
    public class Program {
        private static async Task Main(string[] args) {

            await CreateHostBuilder(args).Build().RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) {
            return Host.CreateDefaultBuilder(args)
                       .UseSystemd()
                       .UseSerilog()
                       .UseWindowsService()
                       .UseEnvironment("Development")
                       .ConfigureLogging(
                           (ctx, _) => {
                               Log.Logger = new LoggerConfiguration()
                                            .ReadFrom.Configuration(ctx.Configuration)
                                            .CreateLogger();
                           }
                       )
                       .ConfigureServices(
                           services => {
                               services.AddHostedService<Service>();
                               services.AddSingleton<IHooksSectionParser, HooksSectionParser>();
                               services.AddSingleton<IProcessProvider, ProcessProvider>();
                               services.AddSingleton<IHooksHandler, HooksHandler>();
                               services.AddValidatorsFromAssemblyContaining<Program>();
                           }
                       );
        }
    }
}
