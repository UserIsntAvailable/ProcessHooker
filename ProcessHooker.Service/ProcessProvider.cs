using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ProcessHooker.Service {
    public class ProcessProvider : IProcessProvider {
        public void Start(string filename) {
            Process.Start(
                new ProcessStartInfo() {
                    FileName        = filename,
                    CreateNoWindow  = false,
                    UseShellExecute = true,
                }
            );
        }

        public IEnumerable<(string ProcessName, bool Responding)> GetProcessesByName(string processName) {
            return Process.GetProcessesByName(processName)
                          .Select(process => (process.ProcessName, process.Responding));
        }
    }
}
