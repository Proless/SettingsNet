using System;
using Newtonsoft.Json;
using SettingsNet.Abstractions;
using SettingsNet.Json;
using Xunit;

namespace SettingsNet.Test
{
    public class SettingsSectionTests
    {
        private readonly ISettingsSource _source;
        private readonly ISettingsProvider _provider;
        private readonly ISettingsSection _root;

        public SettingsSectionTests()
        {
            _source = new JsonSettingsSource("Resources\\settings.json", new JsonSerializer());
            _provider = _source.Load();
            _root = _provider.GetRoot();
        }

        [Fact]
        public void TestStringValue()
        {
            // Arrange

            // Act
            var name = _root.Get<string>("name");

            // Assert
            Assert.Equal("Morrowind", name);
        }

        [Fact]
        public void TestIntValue()
        {
            // Arrange

            // Act
            var downloads = _root.Get<int>("downloads",56);

            // Assert
            Assert.Equal(27161472, downloads);
        }

        [Fact]
        public void TestUriValue()
        {
            // Arrange

            // Act
            var url = _root.Get<Uri>("forum_url");
            var expected = new Uri("https://forums.nexusmods.com/index.php?/forum/111-morrowind/");

            // Assert
            Assert.Equal(expected, url);
        }
    }
}
