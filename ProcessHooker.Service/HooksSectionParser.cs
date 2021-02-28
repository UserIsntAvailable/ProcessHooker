using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace ProcessHooker.Service {
    // TODO - Add FluentValidation to validate if the ProcessHooks are valid
    public class HooksSectionParser : IHooksSectionParser {
        public IEnumerable<ProcessHook> Parse(IConfigurationSection section) {
            return section
                   .AsEnumerable(true)
                   .GroupBy(pair => pair.Key.Split(":")[0])
                   .Select(
                       group => string.Join(
                           ",",
                           group.Where(pair => pair.Value is not null)
                                .Select(pair => $"\"{pair.Key[2..]}\":\"{pair.Value}\"")
                       )
                   )
                   .Select(jsonString => JsonSerializer.Deserialize<ProcessHook>($"{{{jsonString}}}"))
                   .ToList();
        }
    }
}
