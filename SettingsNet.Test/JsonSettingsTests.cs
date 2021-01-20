using System;
using Newtonsoft.Json;
using SettingsNet.Abstractions;
using SettingsNet.Json;
using Xunit;

namespace SettingsNet.Test
{
    public class JsonSettingsTests
    {
        private readonly ISettingsSource _source;
        private readonly ISettingsProvider _provider;
        private readonly ISettingsSection _root;

        public JsonSettingsTests()
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
            var value = _root.GetSection("ConnectionStrings").Get<string>("Default");

            // Assert
            Assert.Equal("Data Source=c:\\database.db;Version=3;", value);
        }

        [Fact]
        public void TestBooleanValue()
        {
            // Arrange

            // Act
            var value = _root.Get<bool>("UseRecaptcha");

            // Assert
            Assert.False(value);
        }

        [Fact]
        public void TestIntValue()
        {
            // Arrange

            // Act
            var value = _root.GetSection("AppIdentitySettings").GetSection("Password").Get<int>("RequiredLength");

            // Assert
            Assert.Equal(6, value);
        }

        [Fact]
        public void TestUriValue()
        {
            // Arrange
            var expected = new Uri("http://www.foo.com/bar.html");

            // Act
            var value = _root.Get<Uri>("HomePage");

            // Assert
            Assert.Equal(expected, value);
        }

        [Fact]
        public void TestArrayValue()
        {
            // Arrange
            var expected = new[] { "!", "$", "%", "?" };

            // Act
            var value = _root.GetSection("AppIdentitySettings").GetSection("Password").Get<string[]>("SpecialChars");

            // Assert
            Assert.Equal(expected, value);
        }
    }
}
