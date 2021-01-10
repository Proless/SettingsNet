using Newtonsoft.Json;
using SettingsNet.Json;
using Xunit;

namespace SettingsNet.Test
{
    public class SettingsSectionTests
    {
        [Fact]
        public void Test01()
        {
            // Arrange
            var source = new JsonSettingsSource("Resources\\settings.json", new JsonSerializer());
            var provider = source.Load();
            var root = provider.GetRoot();

            // Act

            // Assert
        }
    }
}
