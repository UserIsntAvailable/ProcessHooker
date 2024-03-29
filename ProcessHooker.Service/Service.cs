﻿using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace ProcessHooker.Service {
    public class Service : BackgroundService {
        private readonly ILogger<Service>    _logger;
        private readonly IConfiguration      _configuration;
        private readonly IHooksSectionParser _hooksSectionParser;
        private readonly IHooksHandler       _hooksHandler;
        
        // TODO - Change IConfiguration to IConfigurationRoot to reload the configuration
        // TODO - Move _hooksSectionParser to StartAsync
        public Service(
            ILogger<Service>    logger,
            IConfiguration      configuration,
            IHooksSectionParser hooksSectionParser,
            IHooksHandler       hooksHandler
        ) {
            _logger             = logger;
            _configuration      = configuration;
            _hooksSectionParser = hooksSectionParser;
            _hooksHandler       = hooksHandler;
        }

        public override Task StartAsync(CancellationToken cancellationToken) {
            _logger.LogInformation("Service started");

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            var hooks =
                _hooksSectionParser
                    .Parse(_configuration.GetSection("ProcessHooker:Hooks"))
                    .ToArray();

            _logger.LogInformation(
                "Hooks section parsed {Hooks}",
                hooks.Select(hook => JsonSerializer.Serialize(hook))
            );

            var scanDelayOnMilliseconds =
                _configuration.GetValue("ProcessHooker:ScanDelay", 5)
                *
                1000;

            while(!stoppingToken.IsCancellationRequested) {
                await Task.Delay(scanDelayOnMilliseconds, stoppingToken);

                _hooksHandler.Handle(hooks);
            }

            await Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken) {
            _logger.LogInformation("Service stopped");

            return base.StopAsync(cancellationToken);
        }
    }
}
