using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace ProcessHooker.Service {
    class HooksSectionParser : IHooksSectionParser {

        public IEnumerable<ProcessHook> Parse(IConfigurationSection section) {
            throw new System.NotImplementedException();
        }
    }
}
