#if UNITY_ANDROID && HAVE_GOOGLE_PLAY_GAMES
using System;
using System.Threading;
using System.Threading.Tasks;
using GooglePlayGames.BasicApi.SavedGame;

namespace Gilzoide.CloudSave.Providers
{
    internal class PlayGamesSavedGame : ICloudSaveGameMetadata
    {
        public ISavedGameMetadata SavedGameMetadata { get; private set; }

        public string Name => SavedGameMetadata.Filename;

        public string Description => SavedGameMetadata.Description;

        public TimeSpan? TotalPlayTime => SavedGameMetadata.TotalTimePlayed;

        public DateTime? LastModifiedTimestamp => SavedGameMetadata.LastModifiedTimestamp;

        public string DeviceName => null;

        public PlayGamesSavedGame(ISavedGameMetadata savedGameMetadata)
        {
            SavedGameMetadata = savedGameMetadata;
        }

        public async Task OpenAsync(CancellationToken cancellationToken = default)
        {
            if (!SavedGameMetadata.IsOpen)
            {
                SavedGameMetadata = await PlayGamesCloudSaveProvider.OpenAsync(SavedGameMetadata.Filename, cancellationToken: cancellationToken);
            }
        }
    }
}
#endif
