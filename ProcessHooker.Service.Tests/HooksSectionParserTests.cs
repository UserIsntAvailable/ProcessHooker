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

            var jsonHooks =
                actual.Select(
                    hook => $"{{\"{nameof(hook.ProcessName)}\":\"{hook.ProcessName}\"," +
                            $" \"{nameof(hook.FileToOpen)}\":\"{hook.FileToOpen}\"}}"
                );

            var expected = _sut
                .Parse(CreateConfigurationSection(string.Join(",", jsonHooks)));

            actual.Should().BeEquivalentTo(expected);
        }

        private static IConfigurationSection CreateConfigurationSection(string sample) {
            sample = $"{{\"Hooks\":[{sample}]}}";

            return new ConfigurationBuilder()
                   .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(sample)))
                   .Build()
                   .GetSection("Hooks");
        }
    }
}
