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
            while(!stoppingToken.IsCancellationRequested) {
                await Task.Delay(4000, stoppingToken);

                var processHooks = _sectionParser.Parse(_configuration.GetSection("Hooks"));
            }

            await Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken) {
            _logger.LogInformation("Service stopped");

            return base.StopAsync(cancellationToken);
        }
    }
}
