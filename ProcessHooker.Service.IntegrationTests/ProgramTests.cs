using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ProcessHooker.Service.IntegrationTests {
    /// <summary>
    /// <see>
    ///     <cref>https://github.com/fortumoslovarme/realintegrationtests/tree/dbe3ee50b4aad00278014532e7687beb39dc8bfd</cref>
    /// </see>
    /// </summary>
    public class ProgramTests {
        private const string USER_SECRETS_ID = "b2724fc0-027d-4f62-a59e-5f3b0216f25c";

        [Fact]
        public async void RunningProgramWithDotnet_ShouldOpenTwoProcesses_WhenServiceIsStarted() {
            var startTimeOfTest = DateTime.Now;

            Task dotnetTask              = null;
            var  cancellationTokenSource = new CancellationTokenSource();

            var projectPath = Utils.GetProjectLocation(typeof(Program));
            var testTimeout = TimeSpan.FromSeconds(15);

            var hooks = Utils.GetHooksFromAppSettings(USER_SECRETS_ID).ToArray();
            var processesToOpen   = hooks.Select(p => p.HookedProcessName).ToArray();
            var processesToMonitor = hooks.Select(p => p.FileName).ToArray();

            OpenProcessesIfNeeded(processesToOpen);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try {
                dotnetTask = Task.Run(
                    () => Utils.BuildAndRunProject(
                        projectPath,
                        true
                    ), cancellationTokenSource.Token
                );

                do {
                    await Task.Delay(1000, cancellationTokenSource.Token);

                    if(stopwatch.Elapsed > testTimeout)
                        throw new TimeoutException("The test has been running too long without a response");
                } while(!VerifyIfProcessesWereOpened(processesToMonitor, startTimeOfTest));
            }
            finally {
                try {
                    cancellationTokenSource.Cancel();

                    try { dotnetTask?.Wait(cancellationTokenSource.Token); }
                    finally { cancellationTokenSource.Dispose(); }
                }
                catch(Exception) {
                    // ignored
                }

                Utils.CleanupTest(processesToOpen.Concat(processesToMonitor), projectPath, startTimeOfTest);
            }
        }

        private static void OpenProcessesIfNeeded(IEnumerable<string> processesToOpen) {
            foreach(var process in processesToOpen) {
                if(!Utils.IsProcessOpen(process, DateTime.UnixEpoch)) { Utils.OpenProcess(process); }
            }
        }

        private static bool VerifyIfProcessesWereOpened(IEnumerable<string> processesToMonitor, DateTime since) {
            return processesToMonitor
                .All(
                    process =>
                        Utils.IsProcessOpen(process.Replace(".exe", ""), since)
                );
        }
    }
}
