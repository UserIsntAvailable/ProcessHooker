using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ProcessHooker.Service {
    public class HooksSectionParser : IHooksSectionParser {
        private readonly IValidator<Hook>            _validator;
        private readonly ILogger<HooksSectionParser> _logger;

        public HooksSectionParser(IValidator<Hook> validator, ILogger<HooksSectionParser> logger) {
            _validator = validator;
            _logger    = logger;
        }

        public IEnumerable<Hook> Parse(IConfigurationSection section) {
            var hooks = section
                        .AsEnumerable(true)
                        .GroupBy(pair => pair.Key.Split(":")[0])
                        .Select(
                            group => string.Join(
                                ",",
                                group.Where(pair => pair.Value is not null)
                                     .Select(pair => $"\"{pair.Key[2..]}\": \"{pair.Value}\"")
                            )
                        )
                        .Select(hooksProperties => JsonSerializer.Deserialize<Hook>($"{{{hooksProperties}}}"))
                        .ToList();

            return hooks.Where(
                hook => {
                    var validation = _validator.Validate(hook);

                    if(!validation.IsValid) {
                        _logger.LogInformation(
                            "Hook is invalid. Error: {Errors}",
                            string.Join("\n", validation.Errors)
                        );

                        return false;
                    }

                    return true;
                }
            );
        }
    }
}
