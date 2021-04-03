using System.Linq;
using Xunit;
using AutoFixture;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Serilog;

namespace ProcessHooker.Service.UnitTests {
    public class HooksSectionParserTests {
        private readonly HooksSectionParser          _sut;
        private readonly Fixture                     _fixture;
        private readonly IValidator<Hook>            _validator = Substitute.For<IValidator<Hook>>();
        private readonly ILogger<HooksSectionParser> _logger    = Substitute.For<ILogger<HooksSectionParser>>();

        public HooksSectionParserTests() {
            _fixture = new Fixture();
            _sut     = new HooksSectionParser(_validator, _logger);
        }

        [Fact]
        public void Parse_ShouldReturnIEnumerableOfHooks_WhenIConfigurationSectionIsValid() {
            var actual =
                _fixture
                    .CreateMany<Hook>(3)
                    .ToArray();

            var hookValidation = new ValidationResult();

            _validator.Validate(Arg.Any<Hook>()).Returns(hookValidation);

            var expected = _sut
                .Parse(Factory.CreateConfigurationSection(actual));

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
