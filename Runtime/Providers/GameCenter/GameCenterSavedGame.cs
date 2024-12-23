#if UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_TVOS || UNITY_VISIONOS
using System;
using Gilzoide.CloudSave.Providers.Internal;

namespace Gilzoide.CloudSave.Providers
{
    public class GameCenterSavedGame : ICloudSaveGameMetadata, IDisposable
    {
        internal GKSavedGameRef GKSavedGameRef { get; private set; }

        public string Name => GKSavedGameRef.Name;
        public string Description => null;
        public TimeSpan TotalPlayTime => default;
        public DateTime LastModifiedTimestamp => default;
        public string DeviceName => null;

        internal GameCenterSavedGame(GKSavedGameRef savedGame)
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
