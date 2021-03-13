using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace ProcessHooker.Service.UnitTests {
    public static class Factory {
        public static IConfigurationSection CreateConfigurationSection(IEnumerable<Hook> hooks) {
            var jsonHooks = hooks
                .Select(hook => JsonSerializer.Serialize(hook));

            var jsonObject = $"{{\"ProcessHooker\": {{\"Hooks\":[{string.Join(",", jsonHooks)}]}}}}";

            return new ConfigurationBuilder()
                   .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(jsonObject)))
                   .Build()
                   .GetSection("ProcessHooker:Hooks");
        }
    }
}
