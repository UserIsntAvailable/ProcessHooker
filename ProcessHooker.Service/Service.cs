using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace ProcessHooker.Service {
    public class Service : BackgroundService {

        private readonly ILogger<Service> _logger;
        private readonly IConfiguration _configuration;

        public Service(ILogger<Service> logger, IConfiguration configuration) {

            _logger = logger;
            _configuration = configuration;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) {

            return Task.CompletedTask;
        }
    }
}