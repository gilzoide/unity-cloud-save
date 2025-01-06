using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Gilzoide.CloudSave
{
    public class FileCloudSaveProvider : ICloudSaveProvider
    {
        public DirectoryInfo CloudSaveDirectory { get; protected set; }

        public bool IsCloudSaveEnabled => CloudSaveDirectory != null;

        public FileCloudSaveProvider(string directory)
        {
            CloudSaveDirectory = new DirectoryInfo(directory);
        }

        public FileCloudSaveProvider(DirectoryInfo directory)
        {
            CloudSaveDirectory = directory;
        }

        public Task<List<ICloudSaveGameMetadata>> FetchAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                ThrowIfCloudSaveNotEnabled();
                var savedGames = new List<ICloudSaveGameMetadata>();
                if (!CloudSaveDirectory.Exists)
                {
                    return savedGames;
                }

                foreach (FileInfo file in CloudSaveDirectory.EnumerateFiles())
                {
                    var game = new FileSavedGame(file);
                    savedGames.Add(game);
                }
                return savedGames;
            }, cancellationToken);
        }

        public Task<ICloudSaveGameMetadata> FindAsync(string name, CancellationToken cancellationToken = default)
        {
            return Task.Run<ICloudSaveGameMetadata>(() =>
            {
                ThrowIfCloudSaveNotEnabled();
                var file = new FileInfo(Path.Join(CloudSaveDirectory.FullName, name));
                if (file.Exists)
                {
                    return new FileSavedGame(file);
                }
                else
                {
                    return null;
                }
            }, cancellationToken);
        }

        public async Task<byte[]> LoadBytesAsync(ICloudSaveGameMetadata metadata, CancellationToken cancellationToken = default)
        {
            if (metadata is not FileSavedGame fileSavedGame)
            {
                return null;
            }
            ThrowIfCloudSaveNotEnabled();
            return await File.ReadAllBytesAsync(fileSavedGame.FileInfo.FullName, cancellationToken);
        }

        public async Task<string> LoadTextAsync(ICloudSaveGameMetadata metadata, CancellationToken cancellationToken = default)
        {
            if (metadata is not FileSavedGame fileSavedGame)
            {
                return null;
            }
            ThrowIfCloudSaveNotEnabled();
            return await File.ReadAllTextAsync(fileSavedGame.FileInfo.FullName, cancellationToken);
        }

        public async Task<ICloudSaveGameMetadata> SaveBytesAsync(string name, byte[] bytes, CloudSaveGameMetadataUpdate metadata = null, CancellationToken cancellationToken = default)
        {
            ThrowIfCloudSaveNotEnabled();
            var file = new FileInfo(Path.Join(CloudSaveDirectory.FullName, name));
            file.Directory.Create();
            await File.WriteAllBytesAsync(file.FullName, bytes, cancellationToken);
            return new FileSavedGame(file);
        }

        public async Task<ICloudSaveGameMetadata> SaveTextAsync(string name, string text, CloudSaveGameMetadataUpdate metadata = null, CancellationToken cancellationToken = default)
        {
            ThrowIfCloudSaveNotEnabled();
            var file = new FileInfo(Path.Join(CloudSaveDirectory.FullName, name));
            file.Directory.Create();
            await File.WriteAllTextAsync(file.FullName, text, cancellationToken);
            return new FileSavedGame(file);
        }

        public Task<bool> DeleteAsync(string name, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                ThrowIfCloudSaveNotEnabled();
                var file = new FileInfo(Path.Join(CloudSaveDirectory.FullName, name));
                if (file.Exists)
                {
                    file.Delete();
                    return true;
                }
                else
                {
                    return false;
                }
            }, cancellationToken);
        }

        protected void ThrowIfCloudSaveNotEnabled()
        {
            if (!IsCloudSaveEnabled)
            {
                throw new CloudSaveNotEnabledException("Cloud save is not enabled: no directory info");
            }
        }
    }
}
