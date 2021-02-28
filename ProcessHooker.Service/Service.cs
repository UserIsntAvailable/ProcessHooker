using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace ProcessHooker.Service {
    public class Service : BackgroundService {
        private readonly ILogger<Service>     _logger;
        private readonly IConfiguration       _configuration;
        private readonly IHooksSectionParser  _sectionParser;
        private readonly IProcessHooksHandler _hooksHandler;

        public Service(
            ILogger<Service>     logger,
            IConfiguration       configuration,
            IHooksSectionParser  sectionParser,
            IProcessHooksHandler hooksHandler
        ) {
            _logger        = logger;
            _configuration = configuration;
            _sectionParser = sectionParser;
            _hooksHandler  = hooksHandler;
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

            // TODO - Log what processHooks were parsed in a json array format. 
            _logger.LogInformation("Hooks section parsed");

            while(!stoppingToken.IsCancellationRequested) {
                // TODO - Be able to configure the delay time from the appsettings.json.
                await Task.Delay(4000, stoppingToken);

                _hooksHandler.Handle(processHooks);
            }

            await Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken) {
            _logger.LogInformation("Service stopped");

            return base.StopAsync(cancellationToken);
        }
    }
}
