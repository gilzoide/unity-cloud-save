#if UNITY_ANDROID && HAVE_GOOGLE_PLAY_GAMES
using System;
using System.Threading;
using System.Threading.Tasks;
using GooglePlayGames;
using GooglePlayGames.BasicApi.SavedGame;

namespace Gilzoide.CloudSave.Providers
{
    public class PlayGamesSavedGame : ISavedGame
    {
        private ISavedGameMetadata _savedGameMetadata;

        public string Name => _savedGameMetadata.Filename;

        public string Description => _savedGameMetadata.Description;

        public TimeSpan TotalPlayTime => _savedGameMetadata.TotalTimePlayed;

        public DateTime LastModifiedTimestamp => _savedGameMetadata.LastModifiedTimestamp;

        public string DeviceName => null;

        internal PlayGamesSavedGame(ISavedGameMetadata savedGameMetadata)
        {
            _savedGameMetadata = savedGameMetadata;
        }

        public async Task<byte[]> LoadDataAsync(CancellationToken cancellationToken = default)
        {
            if (!_savedGameMetadata.IsOpen)
            {
                _savedGameMetadata = await PlayGamesCloudSaveProvider.OpenAsync(_savedGameMetadata.Filename, cancellationToken: cancellationToken);
            }

            var taskCompletionSource = new TaskCompletionSource<byte[]>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled(cancellationToken)))
            {
                PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(_savedGameMetadata, (status, bytes) =>
                {
                    if (bytes != null)
                    {
                        taskCompletionSource.TrySetResult(bytes);
                    }
                    else
                    {
                        taskCompletionSource.TrySetException(new CloudSaveException($"Load error: {status}"));
                    }
                });
                return await taskCompletionSource.Task;
            }
        }
    }
}
#endif
