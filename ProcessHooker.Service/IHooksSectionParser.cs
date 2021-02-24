using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace ProcessHooker.Service {
    public interface IHooksSectionParser {
        public IEnumerable<ProcessHook> Parse(IConfigurationSection section);
    }
}
