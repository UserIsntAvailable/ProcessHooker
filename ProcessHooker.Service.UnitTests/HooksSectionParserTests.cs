using System.Linq;
using Xunit;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Serilog;

namespace ProcessHooker.Service.UnitTests {
    public class HooksSectionParserTests {
        private readonly HooksSectionParser          _sut;
        private readonly Fixture                     _fixture;
        private readonly ILogger<HooksSectionParser> _logger = Substitute.For<ILogger<HooksSectionParser>>();

        public HooksSectionParserTests() {
            _fixture = new Fixture();
            _sut     = new HooksSectionParser(new HookValidator(), _logger);
        }

        [Fact]
        public void Parse_ShouldReturnIEnumerableOfHooks_WhenIConfigurationSectionIsValid() {
            var actual =
                _fixture
                    .CreateMany<Hook>(3)
                    .ToArray();

            var expected = _sut
                .Parse(Factory.CreateConfigurationSection(actual));

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
