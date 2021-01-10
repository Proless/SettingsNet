using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SettingsNet.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SettingsNet.Json
{
    public class JsonSettingsSection : ISettingsSection
    {
        /* Fields */
        private readonly Dictionary<string, ISettingsSection> _settingsSections;
        private readonly JsonSerializer _jsonSerializer;

        /* Properties */
        public JObject SettingsToken { get; }
        public IEnumerable<ISettingsSection> Sections => GetSections();
        public string Key => PathHelpers.GetKey(Path);
        public string Path => SettingsToken.Path;
        public string Value => SettingsToken.ToString();
        public int Count => SettingsToken.Children<JProperty>().Count(p => p.Value.Type != JTokenType.Object);

        /* Indexer */
        public object this[string key]
        {
            get => InternalGet(key);
            set => InternalSet(key, value);
        }

        /* Constructors */
        public JsonSettingsSection(JsonSettingsProvider settingsProvider, JsonSerializer jsonSerializer)
            : this(settingsProvider.Document, jsonSerializer) { }
        private JsonSettingsSection(JObject token, JsonSerializer jsonSerializer)
        {
            _settingsSections = new Dictionary<string, ISettingsSection>();
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));

            SettingsToken = token ?? throw new ArgumentNullException(nameof(token));

            // Populate sections.
            GetSections();
        }

        /* Methods */
        public ISettingsSection GetSection(string key)
        {
            if (key is null) throw new ArgumentNullException(nameof(key));

            return _settingsSections.TryGetValue(key, out var existingSection) ? existingSection : InternalGetSection(key);
        }
        public T GetSection<T>(string key) where T : class, new()
        {
            if (!(GetSection(key) is JsonSettingsSection section))
                throw new InvalidOperationException(
                    $"Unable to determine the type of the section with the path: {Path}.{key}");


            if (section.SettingsToken.HasValues) return section.Get<T>();


            // At this point we know that this is an new empty section
            // we then serialize an object of the type argument
            // to json and store that json as the new section structure.
            _settingsSections.Remove(key);
            SettingsToken.Remove(key);

            var instance = Activator.CreateInstance<T>();
            var newSectionObject = JObject.FromObject(instance, _jsonSerializer);
            SettingsToken[key] = newSectionObject;
            _settingsSections[key] = new JsonSettingsSection(newSectionObject, _jsonSerializer);
            return instance;
        }
        public T Get<T>() where T : class, new()
        {
            return SettingsToken.ToObject<T>(_jsonSerializer);
        }
        public T Get<T>(string key)
        {
            if (!(InternalGet(key) is string str)) return default;
            using (var stringReader = new StringReader(str))
            using (var reader = new JsonTextReader(stringReader))
            {
                return _jsonSerializer.Deserialize<T>(reader);
            }
        }

        /* Helpers */

        #region Sections
        private IEnumerable<ISettingsSection> GetSections()
        {
            return SettingsToken.Children<JProperty>()
                .Where(p => p.Value.Type == JTokenType.Object)
                .Select(p => p.Name)
                .Select(GetSection);
        }
        private JsonSettingsSection InternalGetSection(string key)
        {
            var existingSection = SettingsToken.Children<JProperty>()
                .FirstOrDefault(p => p.Value.Type == JTokenType.Object && p.Name == key);

            JsonSettingsSection newSection;
            if (existingSection is null)
            {
                var newSectionObject = new JObject();
                SettingsToken[key] = newSectionObject;
                newSection = new JsonSettingsSection(newSectionObject, _jsonSerializer);
            }
            else
            {
                newSection = new JsonSettingsSection(existingSection.Value as JObject, _jsonSerializer);
            }

            _settingsSections[key] = newSection;
            return newSection;
        }
        #endregion

        #region Settings
        private void InternalSet(string key, object value)
        {
            if (key is null) throw new ArgumentNullException(nameof(key));

            // update sections
            GetSections();

            if (value is null)
            {
                SettingsToken[key]?.Remove();
                _settingsSections.Remove(key);
                // update sections
                GetSections();
                return;
            }

            var property = SettingsToken.Children<JProperty>().FirstOrDefault(p => p.Name == key);
            var valueToken = JToken.FromObject(value, _jsonSerializer);

            // we remove the property regardless because we need to replace it
            // either with a section or a single setting value (primitive type)
            property?.Remove();


            // differentiate between JObject and other J-Types
            if (valueToken.Type == JTokenType.Object)
            {
                var newSection = new JsonSettingsSection(valueToken as JObject, _jsonSerializer);
                _settingsSections[key] = newSection;
            }
            else
            {
                // we know this is a section and is being replaced with a single setting value
                if (_settingsSections.ContainsKey(key))
                {
                    _settingsSections.Remove(key);
                }
            }

            SettingsToken[key] = valueToken;
        }
        private object InternalGet(string key)
        {
            if (key is null) throw new ArgumentNullException(nameof(key));

            return SettingsToken.Children<JProperty>()
                .Where(p => p.Value.Type != JTokenType.Object)
                .Where(p => p.Name == key)
                .Select(p => p.Value.ToString())
                .FirstOrDefault();
        }
        private string Serialize(object value)
        {
            if (value is null) return null;

            using (var stream = new StringWriter())
            using (var writer = new JsonTextWriter(stream))
            {
                _jsonSerializer.Serialize(writer, value);
                var json = stream.ToString();
                return json;
            }
        }
        #endregion
    }
}
