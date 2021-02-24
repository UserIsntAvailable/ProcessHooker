using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Xunit;
using FluentAssertions;

namespace ProcessHooker.Service.Tests {
    public class HooksSectionParserTests {
        private readonly HooksSectionParser _sut;

        public HooksSectionParserTests() { _sut = new HooksSectionParser(); }

        [Fact]
        public void Parse_ShouldReturnIEnumerableOfProcessHooks_WhenIConfigurationSectionIsValid() {
            IEnumerable<ProcessHook> actual = new[] {
                new ProcessHook("Rider", "git"),
                new ProcessHook("Hello", "World"),
            };

            var expected = _sut.Parse(CreateConfigurationSection());

            actual.Should().Equal(expected);
        }

        private static IConfigurationSection CreateConfigurationSection() {
            const string sample =
                @"{""Hooks"":[{""ProcessName"":""Hello"", ""FileToOpen"":""World""},{""ProcessName"":""Rider"", ""FileToOpen"":""git""}]}";

            return new ConfigurationBuilder()
               .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(sample)))
               .Build()
               .GetSection("Hooks");
        }
    }
}
