using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace ProcessHooker.Service {
    public class ProcessHooksHandler : IProcessHooksHandler {
        private readonly IProcessProvider             _processProvider;
        private readonly ILogger<ProcessHooksHandler> _logger;

        public ProcessHooksHandler(IProcessProvider processProvider, ILogger<ProcessHooksHandler> logger) {
            _processProvider = processProvider;
            _logger          = logger;
        }

        public void Handle(IEnumerable<ProcessHook> processHooks) {
            var hooksToOpen =
                processHooks
                    .Where(
                        processHook => {
                            var isOpen =
                                _processProvider
                                    .GetProcessesByName(processHook.Name)
                                    .Any(process => process.Responding);

                            if(isOpen) _logger.LogInformation("{ProcessHookName} is open", processHook.Name);

                            return isOpen;
                        }
                    )
                    .Select(hook => hook.HookedFile);

            foreach(var hookFile in hooksToOpen) {
                _logger.LogInformation("Opening {HookFile}", hookFile);

                _processProvider.Start(hookFile);
            }
        }
    }
}
