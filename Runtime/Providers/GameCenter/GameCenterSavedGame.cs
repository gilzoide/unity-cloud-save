#if UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_TVOS || UNITY_VISIONOS
using System;
using Gilzoide.CloudSave.Providers.Internal;

namespace Gilzoide.CloudSave.Providers
{
    internal class GameCenterSavedGame : ICloudSaveGameMetadata, IDisposable
    {
        public GKSavedGameRef GKSavedGameRef { get; private set; }

        public string Name => GKSavedGameRef.Name;
        public string Description => null;
        public TimeSpan? TotalPlayTime => null;
        public DateTime? LastModifiedTimestamp => GKSavedGameRef.LastModifiedTimestamp;
        public string DeviceName => null;

        public GameCenterSavedGame(GKSavedGameRef savedGame)
        {
            GKSavedGameRef = savedGame;
        }

        ~GameCenterSavedGame()
        {
            Dispose();
        }

        public void Dispose()
        {
            GKSavedGameRef.Dispose();
        }
    }
}
#endif
