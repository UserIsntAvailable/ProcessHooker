using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace ProcessHooker.Service {
    public class ProcessProvider : IProcessProvider {
        public void Start(string filename) {
            Process.Start(
                // TODO - Been able to modify the ProcessStartInfo for each ProcessHook
                new ProcessStartInfo() {
                    FileName        = filename,
                    CreateNoWindow  = true,
                    UseShellExecute = false,
                }
            );
        }

        public IEnumerable<(string ProcessName, bool IsOpen)> GetProcessesByName(string processName) {
            return Process.GetProcessesByName(processName)
                          .Select(
                              process => {
                                  bool isOpen;

                                  try { isOpen = !process.HasExited; }
                                  catch(Win32Exception ex) {
                                      /* If the service is not running on administrator mode,
                                       * HasExited property will throw an exception. */
                                      if(!ex.Message.Contains("Access is denied.")) throw;

                                      isOpen = process.Responding;
                                  }

                                  return(process.ProcessName, isOpen);
                              }
                          );
        }
    }
}
