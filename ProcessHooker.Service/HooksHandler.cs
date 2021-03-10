using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace ProcessHooker.Service {
    public class HooksHandler : IHooksHandler {
        private readonly IProcessProvider      _processProvider;
        private readonly ILogger<HooksHandler> _logger;

        public HooksHandler(IProcessProvider processProvider, ILogger<HooksHandler> logger) {
            _processProvider = processProvider;
            _logger          = logger;
        }

        public void Handle(IEnumerable<Hook> hooks) {
            var hooksToOpen =
                hooks
                    .Where(
                        hook => {
                            var isOpen = this.IsProcessOpen(hook.Name);

                            if(isOpen) _logger.LogInformation("{HookName} was open", hook.Name);

                            return isOpen;
                        }
                    );
            
            foreach(var hook in hooksToOpen) {
                // TODO - Change the behaviour of this if statement
                if(this.IsProcessOpen(hook.HookedFileName)) continue;

                _logger.LogInformation("Opening {HookedFileName}", hook.HookedFileName);
                
                _processProvider.Start(hook.HookedFilePath);
            }
        }

        private bool IsProcessOpen(string processName) {
            return _processProvider
                   .GetProcessesByName(processName)
                   .Any(process => process.IsOpen);
        }
    }
}
