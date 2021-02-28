using System.Linq;
using Xunit;
using AutoFixture;
using FluentAssertions;

namespace ProcessHooker.Service.UnitTests {
    public class HooksSectionParserTests {
        private readonly HooksSectionParser _sut;
        private readonly Fixture            _fixture;

        public HooksSectionParserTests() {
            _fixture = new Fixture();
            _sut     = new HooksSectionParser();
        }

        [Fact]
        public void Parse_ShouldReturnIEnumerableOfProcessHooks_WhenIConfigurationSectionIsValid() {
            var actual =
                _fixture
                    .CreateMany<ProcessHook>(3)
                    .ToArray();

            var expected = _sut
                .Parse(Factory.CreateConfigurationSection(actual));

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
