﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ProcessHooker.Service.Tests {
    public static class Factory {
        public static IConfigurationSection CreateConfigurationSection(IEnumerable<ProcessHook> processHooks) {
            var jsonHooks =
                processHooks.Select(
                    hook => $"{{\"{nameof(hook.Name)}\":\"{hook.Name}\"," +
                            $" \"{nameof(hook.HookedFile)}\":\"{hook.HookedFile}\"}}"
                );

            var jsonObject = $"{{\"Hooks\":[ {string.Join(",", jsonHooks)} ]}}";

            return new ConfigurationBuilder()
                   .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(jsonObject)))
                   .Build()
                   .GetSection("Hooks");
        }
    }
}
