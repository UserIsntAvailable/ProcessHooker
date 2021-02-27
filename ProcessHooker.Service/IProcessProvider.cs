using System.Collections.Generic;

namespace ProcessHooker.Service {
    public interface IProcessProvider {
        public void Start(string filename);

        public IEnumerable<(string ProcessName, bool Responding)> GetProcessesByName(string processName);
    }
}
