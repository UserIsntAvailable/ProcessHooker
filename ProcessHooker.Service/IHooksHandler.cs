using System.Collections.Generic;

namespace ProcessHooker.Service {
    public interface IHooksHandler {
        public void Handle(IEnumerable<Hook> hooks);
    }
}
