using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace ProcessHooker.Service {
    public interface IHooksSectionParser {
        // TODO - Create a method parse for IEnumerable<KeyValuePair<string,string>>
        public IEnumerable<Hook> Parse(IConfigurationSection section);
    }
}
