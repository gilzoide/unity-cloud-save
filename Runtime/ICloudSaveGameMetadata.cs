using System;

namespace Gilzoide.CloudSave
{
    public interface ICloudSaveGameMetadata
    {
        string Name { get; }
        string Description { get; }
        TimeSpan? TotalPlayTime { get; }
        DateTime? LastModifiedTimestamp { get; }
        string DeviceName { get; }
    }
}
