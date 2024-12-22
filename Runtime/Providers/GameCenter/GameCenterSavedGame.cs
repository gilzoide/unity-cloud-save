#if UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_TVOS || UNITY_VISIONOS
using System;
using System.Threading;
using System.Threading.Tasks;
using Gilzoide.CloudSave.Providers.Internal;

namespace Gilzoide.CloudSave.Providers
{
    public class GameCenterSavedGame : ISavedGame, IDisposable
    {
        private readonly GKSavedGameRef _savedGame;

        public string Name => _savedGame.Name;
        public string Description => "";
        public TimeSpan TotalPlayTime => default;
        public DateTime LastModifiedTimestamp => default;
        public string DeviceName => "";

        internal GameCenterSavedGame(GKSavedGameRef savedGame)
        {
            _savedGame = savedGame;
        }

        ~GameCenterSavedGame()
        {
            Dispose();
        }

        public Task<byte[]> LoadDataAsync(CancellationToken cancellationToken = default)
        {
            return _savedGame.LoadAsync(cancellationToken);
        }

        public void Dispose()
        {
            _savedGame.Dispose();
        }
    }
}
#endif
