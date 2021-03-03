using System;
using System.Diagnostics;
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
        // TODO - Parse appsettings automatically and get the processName's to monitor
        public const string FirstTestableFile  = "cmd";
        public const string SecondTestableFile = "powershell";
        
        [Fact]
        public async void RunningProgramWithDotnet_ShouldOpenTwoProcesses_WhenServiceIsStarted() {
            var projectPath = Utils.GetProjectLocation(typeof(Program));
            var testTimeout = TimeSpan.FromSeconds(value: 15);

            Task dotnetTask              = null;
            var  cancellationTokenSource = new CancellationTokenSource();
            var  startTimeOfDotnetRun    = DateTime.Now;
            var  stopwatch               = new Stopwatch();
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
                } while(!VerifyIfProcessesWereOpened(startTimeOfDotnetRun));
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

                Utils.CleanupTest(projectPath, startTimeOfDotnetRun);
            }
        }

        private static bool VerifyIfProcessesWereOpened(DateTime testStarted) {
            return Utils.IsProcessOpen(FirstTestableFile, testStarted)
                   &&
                   Utils.IsProcessOpen(SecondTestableFile, testStarted);
        }
    }
}
