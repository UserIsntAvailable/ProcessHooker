using System.Collections.Generic;

namespace ProcessHooker.Service {
    public interface IProcessProvider {
        public void Start(string filename);

        public IEnumerable<(string ProcessName, bool IsOpen)> GetProcessesByName(string processName);
    }
}
