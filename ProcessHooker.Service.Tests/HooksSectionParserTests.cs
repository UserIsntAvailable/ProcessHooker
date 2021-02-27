using System.IO;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Xunit;
using AutoFixture;
using FluentAssertions;

namespace ProcessHooker.Service.Tests {
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
