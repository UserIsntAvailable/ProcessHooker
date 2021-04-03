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
                            var isOpen = this.IsProcessOpen(hook.HookedProcessName);

                            if(isOpen)
                                _logger.LogInformation("{HookHookedProcessName} was open", hook.HookedProcessName);

                            return isOpen;
                        }
                    );

            foreach(var hook in hooksToOpen) {
                // TODO - Change the behaviour of this if statement
                if(this.IsProcessOpen(hook.Filename)) continue;

                _logger.LogInformation("Opening {HookFileName}", hook.Filename);

                _processProvider.Start(hook.FilePath);
            }
        }

        private bool IsProcessOpen(string processName) {
            return _processProvider
                   .GetProcessesByName(processName)
                   .Any(process => process.IsOpen);
        }
    }
}
