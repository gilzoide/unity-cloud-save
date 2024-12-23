using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Gilzoide.CloudSave.Providers
{
    public class DummyCloudSaveProvider : ICloudSaveProvider
    {
        public bool IsCloudSaveEnabled => false;

        public Task<List<ISavedGame>> FetchSavedGamesAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<List<ISavedGame>>(cancellationToken);
            }
            else
            {
                return Task.FromResult(new List<ISavedGame>());
            }
        }

        public Task<ISavedGame> LoadGameAsync(string name, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<ISavedGame>(cancellationToken);
            }
            else
            {
                return Task.FromResult<ISavedGame>(null);
            }
        }

        public Task<ISavedGame> SaveGameAsync(string name, byte[] data, SaveGameMetadata metadata = null, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<ISavedGame>(cancellationToken);
            }
            else
            {
                return Task.FromResult<ISavedGame>(null);
            }
        }

        public Task<bool> DeleteGameAsync(string name, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<bool>(cancellationToken);
            }
            else
            {
                return Task.FromResult(false);
            }
        }
    }
}
