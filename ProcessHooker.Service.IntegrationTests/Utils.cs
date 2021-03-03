using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ProcessHooker.Service.IntegrationTests {
    public static class Utils {
        private const string TEST_APPSETTINGS_NAME = "appsettings.Development.json";

        #region Public Methods
        public static string GetProjectLocation(Type type) {
            var assembly = Assembly.GetAssembly(type);

            var projectName = assembly?.GetName().Name;
            var currentPath = assembly?.Location;

            var folderPath = FindProjectFolderPath(currentPath, projectName);

            return Path.Combine(folderPath, projectName ?? "");
        }

        public static void BuildAndRunProject(string projectPath, bool includeDevelopmentSettings) {
            DeleteAppSettingsDevelopmentOnProjectIfExists(projectPath);

            if(includeDevelopmentSettings) CopyAppSettingsDevelopmentToProject(projectPath, TEST_APPSETTINGS_NAME);

            CreateDotnetProcess(projectPath).Start();
        }

        public static bool IsProcessOpen(string processName, DateTime since) {
            return Process
                   .GetProcessesByName(processName)
                   .Any(p => p.StartTime > since);
        }

        public static void CleanupTest(string projectPath, DateTime startTimeOfTest) {
            KillTestsProcesses(startTimeOfTest);
            DeleteAppSettingsDevelopmentOnProjectIfExists(projectPath);
        }
        #endregion

        #region Private Methods
        private static void KillTestsProcesses(DateTime after) {
            KillAppSettingsProcesses(after);
            KillDotNetProcesses(after);
        }

        private static void KillAppSettingsProcesses(DateTime after) {
            /* I know that this destroys the point of being a util class,
             * I put on my To-do list an auto parser of the appsettings file */
            KillProcess(ProgramTests.FirstTestableFile, after);
            KillProcess(ProgramTests.SecondTestableFile, after);
        }

        private static void KillDotNetProcesses(DateTime after) {
            KillProcess(
                "dotnet",
                after
            );
        }

        private static void KillProcess(string processName, DateTime after) {
            foreach(var process in Process.GetProcessesByName(processName)) {
                if(!process.HasExited && process.StartTime > after) process.Kill();
            }
        }

        private static Process CreateDotnetProcess(string workingDirectory) {
            return new Process() {
                StartInfo = new ProcessStartInfo {
                    FileName         = "dotnet",
                    Arguments        = "run",
                    UseShellExecute  = false,
                    CreateNoWindow   = true,
                    WorkingDirectory = workingDirectory,
                },
            };
        }

        private static string FindProjectFolderPath(string currentDirectory, string projectName) {
            do {
                currentDirectory = Directory
                                   .GetParent(currentDirectory)
                                   ?.FullName;
            } while(Directory
                    .GetDirectories(currentDirectory ?? throw new NullReferenceException(nameof(currentDirectory)))
                    .All(dirFullName => dirFullName != Path.Combine(currentDirectory, projectName)));

            return currentDirectory;
        }

        private static void DeleteAppSettingsDevelopmentOnProjectIfExists(string projectPath) {
            // TODO - Get the assembly that the project is targeting automatically 
            var appsettingsBuildPath = Path
                .Combine(
                    projectPath,
                    "bin",
                    "Debug",
                    "net5.0",
                    TEST_APPSETTINGS_NAME
                );

            if(File.Exists(appsettingsBuildPath)) File.Delete(appsettingsBuildPath);

            var appsettingsProjectPath = Path.Combine(projectPath, TEST_APPSETTINGS_NAME);

            if(File.Exists(appsettingsProjectPath)) File.Delete(appsettingsProjectPath);
        }

        private static void CopyAppSettingsDevelopmentToProject(string projectPath, string appSettingsPath) {
            File.Copy(
                appSettingsPath,
                Path.Combine(projectPath, appSettingsPath)
            );
        }
        #endregion
    }
}
