using System.Linq;
using AutoFixture;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace ProcessHooker.Service.Tests {
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
        public void Handle_ShouldStartProcesses_WhenProcessHooksAreValid() {
            const int processCount = 3;

            var hooks =
                _fixture
                    .CreateMany<ProcessHook>(processCount)
                    .ToArray();

            _processProvider
                .GetProcessesByName(Arg.Any<string>())
                .Returns(hooks.Select(hook => (hook.Name, true)));

            _sut.Handle(hooks);

            _processProvider
                .Received(processCount)
                .GetProcessesByName(Arg.Any<string>());

            _processProvider
                .Received(processCount)
                .Start(Arg.Any<string>());
        }
    }
}
