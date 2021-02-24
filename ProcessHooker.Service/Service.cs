using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace ProcessHooker.Service {
    public class Service : BackgroundService {
        private readonly ILogger<Service>    _logger;
        private readonly IConfiguration      _configuration;
        private readonly IHooksSectionParser _sectionParser;

        public Service(ILogger<Service> logger, IConfiguration configuration, IHooksSectionParser sectionParser) {
            _logger        = logger;
            _configuration = configuration;
            _sectionParser = sectionParser;
        }

        public override Task StartAsync(CancellationToken cancellationToken) {
            _logger.LogInformation("Service started");

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            var processHooks =
                _sectionParser
                    .Parse(_configuration.GetSection("Hooks"))
                    .ToArray();

            while(!stoppingToken.IsCancellationRequested) {
                await Task.Delay(4000, stoppingToken);

                var filesToOpen =
                    processHooks
                        .Where(
                            hook => Process.GetProcessesByName(hook.ProcessName)
                                           .Any(process => process.Responding)
                        )
                        .Select(hook => hook.FileToOpen);

                _logger.LogInformation("Hooks section parsed");

                StartProcesses(filesToOpen);

                _logger.LogInformation("Processes opened");
            }

            await Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken) {
            _logger.LogInformation("Service stopped");

            return base.StopAsync(cancellationToken);
        }

        private static void StartProcesses(IEnumerable<string> filesToOpen) {
            foreach(var file in filesToOpen) {
                Process.Start(
                    new ProcessStartInfo() {
                        FileName        = file,
                        CreateNoWindow  = false,
                        UseShellExecute = true,
                    }
                );
            }
        }
    }
}
