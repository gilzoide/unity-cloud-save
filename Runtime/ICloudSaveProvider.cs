using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gilzoide.CloudSave
{
    public interface ICloudSaveProvider
    {
        /// <summary>
        /// Checks whether cloud save is currently enabled.
        /// If the provider requires an external account and the user is not signed in, this would return false.
        /// </summary>
        bool IsCloudSaveEnabled { get; }

        /// <summary>
        /// Fetches a list of cloud save games metadata.
        /// </summary>
        Task<List<ICloudSaveGameMetadata>> FetchSavedGamesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Find a saved game with the specified name.
        /// Returns <see langword="null"/> if it cannot be found.
        /// </summary>
        Task<ICloudSaveGameMetadata> FindSavedGameAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Load raw data from a known saved game.
        /// </summary>
        Task<byte[]> LoadBytesAsync(ICloudSaveGameMetadata metadata, CancellationToken cancellationToken = default);

        /// <summary>
        /// Load text data from a known saved game.
        /// The default implementation is a wrapper for <see cref="LoadBytesAsync"/> using UTF8 encoding.
        /// </summary>
        async Task<string> LoadTextAsync(ICloudSaveGameMetadata metadata, CancellationToken cancellationToken = default) => Encoding.UTF8.GetString(await LoadBytesAsync(metadata, cancellationToken));

        /// <summary>
        /// Write raw data to a save game with the specified name.
        /// If a save with the same name already exists, its data is updated.
        /// </summary>
        /// <returns>The cloud saved game metadata</returns>
        Task<ICloudSaveGameMetadata> SaveBytesAsync(string name, byte[] bytes, CloudSaveGameMetadataUpdate metadata = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Write text data to a save game with the specified name.
        /// If a save with the same name already exists, its data is updated.
        /// The default implementation is a wrapper for <see cref="SaveBytesAsync"/> using UTF8 encoding.
        /// </summary>
        /// <returns>The cloud saved game metadata</returns>
        Task<ICloudSaveGameMetadata> SaveTextAsync(string name, string text, CloudSaveGameMetadataUpdate metadata = null, CancellationToken cancellationToken = default) => SaveBytesAsync(name, Encoding.UTF8.GetBytes(text), metadata, cancellationToken);

        /// <summary>
        /// Delete a cloud save game with the specified name.
        /// </summary>
        /// <returns></returns>
        Task<bool> DeleteGameAsync(string name, CancellationToken cancellationToken = default);
    }

    public static class ICloudSaveProviderExtensions
    {
        /// <summary>
        /// Load raw data from a known cloud save with the specified name.
        /// Returns <see langword="null"/> if it cannot be found.
        /// </summary>
        /// <seealso cref="ICloudSaveProvider.FindSavedGameAsync"/>
        /// <seealso cref="ICloudSaveProvider.LoadBytesAsync"/>
        public static async Task<byte[]> LoadBytesAsync(this ICloudSaveProvider cloudSaveProvider, string name, CancellationToken cancellationToken = default)
        {
            if (await cloudSaveProvider.FindSavedGameAsync(name, cancellationToken) is ICloudSaveGameMetadata metadata)
            {
                return await cloudSaveProvider.LoadBytesAsync(metadata, cancellationToken);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Load text data from a known cloud save with the specified name.
        /// Returns <see langword="null"/> if it cannot be found.
        /// </summary>
        /// <seealso cref="ICloudSaveProvider.FindSavedGameAsync"/>
        /// <seealso cref="ICloudSaveProvider.LoadTextAsync"/>
        public static async Task<string> LoadTextAsync(this ICloudSaveProvider cloudSaveProvider, string name, CancellationToken cancellationToken = default)
        {
            if (await cloudSaveProvider.FindSavedGameAsync(name, cancellationToken) is ICloudSaveGameMetadata metadata)
            {
                return await cloudSaveProvider.LoadTextAsync(metadata, cancellationToken);
            }
            else
            {
                return null;
            }
        }
    }
}
