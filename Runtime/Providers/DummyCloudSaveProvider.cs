using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Gilzoide.CloudSave.Providers
{
    public class DummyCloudSaveProvider : ICloudSaveProvider
    {
        public bool IsCloudSaveEnabled => false;

        public Task<List<ICloudSaveGameMetadata>> FetchSavedGamesAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<List<ICloudSaveGameMetadata>>(cancellationToken);
            }
            else
            {
                return Task.FromResult(new List<ICloudSaveGameMetadata>());
            }
        }

        public Task<ICloudSaveGameMetadata> FindSavedGameAsync(string name, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<ICloudSaveGameMetadata>(cancellationToken);
            }
            else
            {
                return Task.FromResult<ICloudSaveGameMetadata>(null);
            }
        }

        public Task<byte[]> LoadBytesAsync(ICloudSaveGameMetadata metadata, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<byte[]>(cancellationToken);
            }
            else
            {
                return Task.FromResult<byte[]>(null);
            }
        }

        public Task<ICloudSaveGameMetadata> SaveBytesAsync(string name, byte[] bytes, SaveGameMetadata metadata = null, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<ICloudSaveGameMetadata>(cancellationToken);
            }
            else
            {
                return Task.FromResult<ICloudSaveGameMetadata>(null);
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
