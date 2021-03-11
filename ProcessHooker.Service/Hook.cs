using System;
using System.IO;

namespace ProcessHooker.Service {
    /// <summary>
    /// Maps hooks from the appsettings.json file
    /// </summary>
    // TODO - Change the properties names they are every misleading
    [Serializable]
    public record Hook(string HookedProcessName, string FilePath) {
        public string FileName {
            get {
                return this.FilePath
                           .Split(Path.PathSeparator)[0]
                           .Split(".")[0];
            }
        }
    }
}
