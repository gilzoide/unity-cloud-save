#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Gilzoide.CloudSave.Providers
{
    public class EditorCloudSaveProvider : ICloudSaveProvider
    {
        public string CloudSaveDirectory { get; set; }

        public bool IsCloudSaveEnabled => true;

        public EditorCloudSaveProvider() : this("Library/Gilzoide.CloudSave")
        {
        }

        public EditorCloudSaveProvider(string cloudSaveDirectory)
        {
            CloudSaveDirectory = cloudSaveDirectory;
        }

        public Task<List<ICloudSaveGameMetadata>> FetchSavedGamesAsync(CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                var savedGames = new List<ICloudSaveGameMetadata>();

                var dir = new DirectoryInfo(CloudSaveDirectory);
                if (!dir.Exists)
                {
                    return savedGames;
                }

                foreach (FileInfo file in dir.EnumerateFiles())
                {
                    var game = new EditorSavedGame(file);
                    savedGames.Add(game);
                }
                return savedGames;
            }, cancellationToken);
        }

        public Task<ICloudSaveGameMetadata> FindSavedGameAsync(string name, CancellationToken cancellationToken = default)
        {
            return Task.Run<ICloudSaveGameMetadata>(() =>
            {
                var file = new FileInfo(Path.Join(CloudSaveDirectory, name));
                if (file.Exists)
                {
                    return new EditorSavedGame(file);
                }
                else
                {
                    return null;
                }
            }, cancellationToken);
        }

        public Task<byte[]> LoadBytesAsync(ICloudSaveGameMetadata metadata, CancellationToken cancellationToken = default)
        {
            if (metadata is EditorSavedGame editorSavedGame)
            {
                return File.ReadAllBytesAsync(editorSavedGame.File.FullName, cancellationToken);
            }
            else
            {
                return null;
            }
        }

        public Task<string> LoadTextAsync(ICloudSaveGameMetadata metadata, CancellationToken cancellationToken = default)
        {
            if (metadata is EditorSavedGame editorSavedGame)
            {
                return File.ReadAllTextAsync(editorSavedGame.File.FullName, cancellationToken);
            }
            else
            {
                return null;
            }
        }

        public Task<ICloudSaveGameMetadata> SaveBytesAsync(string name, byte[] bytes, CloudSaveGameMetadataUpdate metadata = null, CancellationToken cancellationToken = default)
        {
            return Task.Run<ICloudSaveGameMetadata>(() =>
            {
                var file = new FileInfo(Path.Join(CloudSaveDirectory, name));
                file.Directory.Create();
                File.WriteAllBytes(file.FullName, bytes);
                return new EditorSavedGame(file);
            }, cancellationToken);
        }

        public Task<ICloudSaveGameMetadata> SaveTextAsync(string name, string text, CloudSaveGameMetadataUpdate metadata = null, CancellationToken cancellationToken = default)
        {
            return Task.Run<ICloudSaveGameMetadata>(() =>
            {
                var file = new FileInfo(Path.Join(CloudSaveDirectory, name));
                file.Directory.Create();
                File.WriteAllText(file.FullName, text);
                return new EditorSavedGame(file);
            }, cancellationToken);
        }

        public Task<bool> DeleteGameAsync(string name, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                var file = new FileInfo(Path.Join(CloudSaveDirectory, name));
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
    }
}
#endif
