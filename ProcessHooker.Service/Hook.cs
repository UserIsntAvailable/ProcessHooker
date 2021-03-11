using System;
using System.IO;

namespace ProcessHooker.Service {
    /// <summary>
    /// Maps hooks from the appsettings.json file
    /// </summary>
    [Serializable]
    public record Hook(string HookedProcessName, string FilePath) {
        public string FileName => Path.GetFileName(this.FilePath);
    }
}
