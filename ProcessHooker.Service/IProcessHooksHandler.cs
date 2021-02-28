using System.Collections.Generic;

namespace ProcessHooker.Service {
    public interface IProcessHooksHandler {
        public void Handle(IEnumerable<ProcessHook> hooks);
    }
}
