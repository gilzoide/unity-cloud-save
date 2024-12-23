#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Gilzoide.CloudSave.Providers
{
    public class EditorCloudSaveProvider : ICloudSaveProvider
    {
        public string CloudSaveDirectory = "Library/Gilzoide.CloudSave";

        public bool IsCloudSaveEnabled => true;

        public Task<List<ISavedGame>> FetchSavedGamesAsync(CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                var savedGames = new List<ISavedGame>();

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

        public Task<ISavedGame> LoadGameAsync(string name, CancellationToken cancellationToken = default)
        {
            return Task.Run<ISavedGame>(() =>
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

        public Task<ISavedGame> SaveGameAsync(string name, byte[] data, SaveGameMetadata metadata = null, CancellationToken cancellationToken = default)
        {
            return Task.Run<ISavedGame>(() =>
            {
                var file = new FileInfo(Path.Join(CloudSaveDirectory, name));
                file.Directory.Create();
                File.WriteAllBytes(file.FullName, data);
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
