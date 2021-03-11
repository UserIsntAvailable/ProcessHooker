using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace ProcessHooker.Service {
    // TODO - Add FluentValidation to validate if the hooks are valid
    public class HooksSectionParser : IHooksSectionParser {
        public IEnumerable<Hook> Parse(IConfigurationSection section) {
            return section
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
        }
    }
}
