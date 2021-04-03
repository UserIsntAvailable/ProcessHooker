using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;

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
            try {
                return Process
                       .GetProcessesByName(processName)
                       .Any(p => p.StartTime > since);
            }
            catch(Win32Exception) { return true; }
        }

        public static void CleanupTest(
            IEnumerable<string> processesToKill,
            string              projectPath,
            DateTime            startTimeOfTest
        ) {
            KillTestsProcesses(processesToKill, startTimeOfTest);
            DeleteAppSettingsDevelopmentOnProjectIfExists(projectPath);
        }

        public static IEnumerable<Hook> GetHooksFromAppSettings(string userSecretsId) {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(TEST_APPSETTINGS_NAME, false, false);

            if(!string.IsNullOrWhiteSpace(userSecretsId)) configuration.AddUserSecrets(userSecretsId);

            return new HooksSectionParser(new HookValidator(), null)
                .Parse(
                    configuration
                        .Build()
                        .GetSection("ProcessHooker:Hooks")
                );
        }

        public static void OpenProcess(string filename) {
            Process.Start(
                new ProcessStartInfo() {
                    FileName        = filename,
                    UseShellExecute = false,
                }
            );
        }
        #endregion

        #region Private Methods
        private static void KillTestsProcesses(IEnumerable<string> processesToKill, DateTime since) {
            KillAppSettingsProcesses(processesToKill, since);
            KillDotNetProcesses(since);
        }

        private static void KillAppSettingsProcesses(IEnumerable<string> processesToKill, DateTime since) {
            foreach(var process in processesToKill) { KillProcess(process, since); }
        }

        private static void KillDotNetProcesses(DateTime since) {
            KillProcess(
                "dotnet",
                since
            );
        }

        private static void KillProcess(string processName, DateTime since) {
            foreach(var process in Process.GetProcessesByName(processName)) {
                try {
                    if(!process.HasExited && process.StartTime > since) process.Kill();
                }
                catch(Win32Exception) {
                    // ignored
                }
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
