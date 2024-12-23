using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Gilzoide.CloudSave
{
    public interface ICloudSaveProvider
    {
        bool IsCloudSaveEnabled { get; }
        Task<List<ISavedGame>> FetchSavedGamesAsync(CancellationToken cancellationToken = default);
        Task<ISavedGame> LoadGameAsync(string name, CancellationToken cancellationToken = default);
        Task<ISavedGame> SaveGameAsync(string name, byte[] data, SaveGameMetadata metadata = null, CancellationToken cancellationToken = default);
        Task<bool> DeleteGameAsync(string name, CancellationToken cancellationToken = default);
    }
}
