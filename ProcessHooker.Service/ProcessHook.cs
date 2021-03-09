using System.IO;

namespace ProcessHooker.Service {
    /// <summary>
    /// Maps process hooks from the appsettings.json file
    /// </summary>
    public record ProcessHook(string Name, string HookedFilePath) {
        public string HookedFileName {
            get {
                return this.HookedFilePath
                           .Split(Path.PathSeparator)[0]
                           .Split(".")[0];
            }
        }
    }
}
