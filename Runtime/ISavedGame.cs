using System;
using System.Threading;
using System.Threading.Tasks;

namespace Gilzoide.CloudSave
{
    public interface ISavedGame
    {
        string Name { get; }
        string Description { get; }
        TimeSpan TotalPlayTime { get; }
        DateTime LastModifiedTimestamp { get; }
        string DeviceName { get; }

        Task<byte[]> LoadDataAsync(CancellationToken cancellationToken = default);
    }
}
