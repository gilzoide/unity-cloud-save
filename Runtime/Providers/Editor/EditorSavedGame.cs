#if UNITY_EDITOR
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Gilzoide.CloudSave
{
    internal class EditorSavedGame : ICloudSaveGameMetadata
    {
        public FileInfo File { get; private set; }

        public string Name => File.Name;

        public string Description => null;

        public TimeSpan? TotalPlayTime => null;

        public DateTime? LastModifiedTimestamp => File.LastWriteTimeUtc;

        public string DeviceName => null;

        public EditorSavedGame(FileInfo file)
        {
            File = file;
        }

        public Task<byte[]> LoadDataAsync(CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                return System.IO.File.ReadAllBytes(File.FullName);
            }, cancellationToken);
        }
    }
}
#endif
