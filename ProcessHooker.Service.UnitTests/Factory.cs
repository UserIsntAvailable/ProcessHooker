using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ProcessHooker.Service.UnitTests {
    public static class Factory {
        public static IConfigurationSection CreateConfigurationSection(IEnumerable<ProcessHook> processHooks) {
            var jsonHooks =
                processHooks.Select(
                    hook => $"{{\"{hook.Name}\":\"{hook.HookedFilePath}\"}}"
                );

            var jsonObject = $"{{\"Hooks\":[{string.Join(",", jsonHooks)}]}}";

            return new ConfigurationBuilder()
                   .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(jsonObject)))
                   .Build()
                   .GetSection("Hooks");
        }
    }
}
