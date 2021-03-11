using System;
using System.IO;

namespace ProcessHooker.Service {
    /// <summary>
    /// Maps hooks from the appsettings.json file
    /// </summary>
    // TODO - Change the properties names they are every misleading
    [Serializable]
    public record Hook(string Name, string HookedFilePath) {
        
        public string HookedFileName {
            get {
                return this.HookedFilePath
                           .Split(Path.PathSeparator)[0]
                           .Split(".")[0];
            }
        }
    }
}
