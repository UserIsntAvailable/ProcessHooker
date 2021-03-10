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

                            if(isOpen) _logger.LogInformation("{HookName} is open", hook.Name);

                            return isOpen;
                        }
                    )
                    .Select(hook => hook.HookedFileName);
            
            foreach(var hookFile in hooksToOpen) {
                // TODO - Change the behaviour of this if statement
                if(this.IsProcessOpen(hookFile)) continue;

                _logger.LogInformation("Opening {HookFile}", hookFile);
                
                // BUG - I'm taking the name of the file instead of the file path
                _processProvider.Start(hookFile);
            }
        }

        private bool IsProcessOpen(string processName) {
            return _processProvider
                   .GetProcessesByName(processName)
                   .Any(process => process.IsOpen);
        }
    }
}
