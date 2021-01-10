using System.Collections.Generic;

namespace SettingsNet.Abstractions
{
    public interface ISettingsSectionProvider
    {
        /// <summary>
        /// Get a settings sub-section with the specified key.
        /// </summary>
        /// <param name="key">The key of the settings section.</param>
        /// <returns>The <see cref="ISettingsSection"/>.</returns>
        /// <remarks>
        ///     This method will never return <c>null</c>. If no matching sub-section is found with the specified key,
        ///     an empty <see cref="ISettingsSection"/> will be returned.
        /// </remarks>
        ISettingsSection GetSection(string key);

        /// <summary>
        /// Get an <see cref="ISettingsSection"/> by key as the specified .Net type.
        /// </summary>
        /// <typeparam name="T">The .Net type.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>The .Net object.</returns>
        T GetSection<T>(string key) where T : class, new();

        /// <summary>
        /// Get the immediate descendant settings sub-sections.
        /// </summary>
        IEnumerable<ISettingsSection> Sections { get; }
    }
}
