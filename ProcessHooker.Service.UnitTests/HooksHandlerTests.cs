using System.Linq;
using AutoFixture;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace ProcessHooker.Service.UnitTests {
    public class HooksHandlerTests {
        private readonly HooksHandler          _sut;
        private readonly Fixture               _fixture         = new Fixture();
        private readonly IProcessProvider      _processProvider = Substitute.For<IProcessProvider>();
        private readonly ILogger<HooksHandler> _logger          = Substitute.For<ILogger<HooksHandler>>();

        public HooksHandlerTests() {
            _sut = new HooksHandler(
                _processProvider,
                _logger
            );
        }

        [Fact]
        public void Handle_ShouldStartProcesses_WhenHooksAreValidAndHooksAreNotOpenYet() {
            const int processesCount = 3;

            var hooks =
                _fixture
                    .CreateMany<Hook>(processesCount)
                    .ToArray();

            _processProvider
                .GetProcessesByName(Arg.Is<string>(s => s.Contains("HookedProcessName")))
                .Returns(hooks.Select(hook => (hook.HookedProcessName, true)));

            _processProvider
                .GetProcessesByName(Arg.Is<string>(s => s.Contains("FilePath")))
                .Returns(hooks.Select(hook => (hook.FilePath, false)));

            _sut.Handle(hooks);

            _processProvider
                .Received(processesCount)
                .GetProcessesByName(Arg.Is<string>(s => s.Contains("HookedProcessName")));

            _processProvider
                .Received(processesCount)
                .Start(Arg.Is<string>(s => s.Contains("FilePath")));
        }

        [Fact]
        public void Handle_ShouldNotStartProcesses_WhenHooksAreValidButHooksAreAlreadyOpen() {
            const int processesCount = 3;

            var hooks =
                _fixture
                    .CreateMany<Hook>(processesCount)
                    .ToArray();

            _processProvider
                .GetProcessesByName(Arg.Any<string>())
                .Returns(hooks.Select(hook => (hook.HookedProcessName, true)));

            _sut.Handle(hooks);

            _processProvider
                .Received(processesCount * 2)
                .GetProcessesByName(Arg.Any<string>());

            _processProvider
                .Received(processesCount - processesCount)
                .Start(Arg.Any<string>());
        }
    }
}
