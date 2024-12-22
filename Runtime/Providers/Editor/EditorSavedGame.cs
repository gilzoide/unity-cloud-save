#if UNITY_EDITOR
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Gilzoide.CloudSave
{
    public class EditorSavedGame : ISavedGame
    {
        private readonly FileInfo _file;

        public string Name => _file.Name;

        public string Description => null;

        public TimeSpan TotalPlayTime => default;

        public DateTime LastModifiedTimestamp => _file.LastWriteTimeUtc;

        public string DeviceName => null;

        public EditorSavedGame(FileInfo file)
        {
            _file = file;
        }

        public Task<byte[]> LoadDataAsync(CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                return File.ReadAllBytes(_file.FullName);
            }, cancellationToken);
        }
    }
}
#endif
