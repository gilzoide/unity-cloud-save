using System;
using System.IO;

namespace Gilzoide.CloudSave
{
    internal class FileSavedGame : ICloudSaveGameMetadata
    {
        public FileInfo FileInfo { get; private set; }

        public string Name => FileInfo.Name;
        public string Description => null;
        public TimeSpan? TotalPlayTime => null;
        public DateTime? LastModifiedTimestamp => FileInfo.LastWriteTimeUtc;

        public FileSavedGame(FileInfo fileInfo)
        {
            FileInfo = fileInfo;
        }
    }
}
