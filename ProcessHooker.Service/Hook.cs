using System;
using System.IO;

namespace ProcessHooker.Service {
    /// <summary>
    /// Maps hooks from the appsettings.json file
    /// </summary>
    [Serializable]
    public record Hook(string HookedProcessName, string FilePath) {
        private string _filename = "";
        public string FileName {
            get {
                return string.IsNullOrEmpty(_filename)
                    ? Path.GetFileName(this.FilePath)
                    : _filename;
            }
            set { _filename = value; }
        }
    }
}
