using System.Linq;
using AutoFixture;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace ProcessHooker.Service.UnitTests {
    public class HooksHandlerTests {
        private readonly ProcessHooksHandler          _sut;
        private readonly Fixture                      _fixture         = new Fixture();
        private readonly IProcessProvider             _processProvider = Substitute.For<IProcessProvider>();
        private readonly ILogger<ProcessHooksHandler> _logger          = Substitute.For<ILogger<ProcessHooksHandler>>();

        public HooksHandlerTests() {
            _sut = new ProcessHooksHandler(
                _processProvider,
                _logger
            );
        }

        [Fact]
        public void Handle_ShouldStartProcesses_WhenProcessHooksAreValidAndHooksAreNotOpenYet() {
            const int processesCount = 3;

            var processHooks =
                _fixture
                    .CreateMany<ProcessHook>(processesCount)
                    .ToArray();

            _processProvider
                .GetProcessesByName(Arg.Is<string>(s => s.Contains("Name")))
                .Returns(processHooks.Select(processHook => (processHook.Name, true)));

            _processProvider
                .GetProcessesByName(Arg.Is<string>(s => s.Contains("HookedFile")))
                .Returns(processHooks.Select(processHook => (processHook.HookedFile, false)));

            _sut.Handle(processHooks);

            _processProvider
                .Received(processesCount)
                .GetProcessesByName(Arg.Is<string>(s => s.Contains("Name")));

            _processProvider
                .Received(processesCount)
                .Start(Arg.Is<string>(s => s.Contains("HookedFile")));
        }

        [Fact]
        public void Handle_ShouldNotStartProcesses_WhenProcessHooksAreValidButHooksAreAlreadyOpen() {
            const int processesCount = 3;

            var processHooks =
                _fixture
                    .CreateMany<ProcessHook>(processesCount)
                    .ToArray();

            _processProvider
                .GetProcessesByName(Arg.Any<string>())
                .Returns(processHooks.Select(processHook => (processHook.Name, true)));

            _sut.Handle(processHooks);

            _processProvider
                .Received(processesCount * 2)
                .GetProcessesByName(Arg.Any<string>());

            _processProvider
                .Received(processesCount - processesCount)
                .Start(Arg.Any<string>());
        }
    }
}
