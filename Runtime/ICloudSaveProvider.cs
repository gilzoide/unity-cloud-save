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
        Task<ICloudSaveGameMetadata> FindSavedGameAsync(string name, CancellationToken cancellationToken = default);
        Task<byte[]> LoadBytesAsync(ICloudSaveGameMetadata metadata, CancellationToken cancellationToken = default);
        async Task<string> LoadTextAsync(ICloudSaveGameMetadata metadata, CancellationToken cancellationToken = default) => Encoding.UTF8.GetString(await LoadBytesAsync(metadata, cancellationToken));
        Task<ICloudSaveGameMetadata> SaveBytesAsync(string name, byte[] bytes, SaveGameMetadata metadata = null, CancellationToken cancellationToken = default);
        Task<ICloudSaveGameMetadata> SaveTextAsync(string name, string text, SaveGameMetadata metadata = null, CancellationToken cancellationToken = default) => SaveBytesAsync(name, Encoding.UTF8.GetBytes(text), metadata, cancellationToken);
        Task<bool> DeleteGameAsync(string name, CancellationToken cancellationToken = default);
    }

    public static class ICloudSaveProviderExtensions
    {
        public static async Task<byte[]> LoadBytesAsync(this ICloudSaveProvider cloudSaveProvider, string name, CancellationToken cancellationToken = default)
        {
            ICloudSaveGameMetadata metadata = await cloudSaveProvider.FindSavedGameAsync(name, cancellationToken);
            if (metadata != null)
            {
                return await cloudSaveProvider.LoadBytesAsync(metadata, cancellationToken);
            }
            else
            {
                return null;
            }
        }

        public static async Task<string> LoadTextAsync(this ICloudSaveProvider cloudSaveProvider, string name, CancellationToken cancellationToken = default)
        {
            ICloudSaveGameMetadata metadata = await cloudSaveProvider.FindSavedGameAsync(name, cancellationToken);
            if (metadata != null)
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
