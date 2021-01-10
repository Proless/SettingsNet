namespace SettingsNet.Abstractions
{
    /// <summary>
    /// Represents a section of application setting values.
    /// </summary>
    public interface ISettingsSection : ISettingsSectionProvider
    {
        /// <summary>
        /// Get this <see cref="ISettingsSection"/> as the specified .Net type.
        /// </summary>
        /// <typeparam name="T">The .Net type.</typeparam>
        /// <returns>A .Net object or null.</returns>
        T Get<T>() where T : class, new();

        /// <summary>
        /// Get the setting value by key as the specified .Net type.
        /// </summary>
        /// <typeparam name="T">The .Net type.</typeparam>
        /// <returns>The .Net object or a default value.</returns>
        T Get<T>(string key);

        /// <summary>
        /// Get the key this section occupies in its parent.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Get the full path to this section within the <see cref="ISettingsSectionProvider"/>.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Get the section value.
        /// </summary>
        string Value { get; }

        /// <summary>
        /// The count of  key/value pair settings.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Get or set a setting value.
        /// </summary>
        /// <param name="key">The setting key.</param>
        /// <returns>The setting value.</returns>
        object this[string key] { get; set; }
    }
}
