#if UNITY_ANDROID && HAVE_GOOGLE_PLAY_GAMES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

namespace Gilzoide.CloudSave.Providers
{
    public class PlayGamesCloudSaveProvider : ICloudSaveProvider
    {
        private static List<ISavedGameMetadata> _savedGames;

        #region ICloudSaveProvider

        public bool IsCloudSaveEnabled => PlayGamesPlatform.Instance.SavedGame != null;

        public async Task<List<ICloudSaveGameMetadata>> FetchSavedGamesAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfCloudSaveNotEnabled();
            List<ISavedGameMetadata> savedGamesMetadata = await FetchSavedGamesMetadataAsync(cancellationToken: cancellationToken);
            _savedGames ??= savedGamesMetadata;
            return new List<ICloudSaveGameMetadata>(savedGamesMetadata.Select(m => new PlayGamesSavedGame(m)));
        }

        public async Task<ICloudSaveGameMetadata> FindSavedGameAsync(string name, CancellationToken cancellationToken = default)
        {
            ThrowIfCloudSaveNotEnabled();
            ISavedGameMetadata savedGame = await OpenExistingAsync(name, cancellationToken: cancellationToken);
            if (savedGame != null)
            {
                return new PlayGamesSavedGame(savedGame);
            }
            else
            {
                return null;
            }
        }

        public async Task<byte[]> LoadBytesAsync(ICloudSaveGameMetadata metadata, CancellationToken cancellationToken = default)
        {
            if (metadata is not PlayGamesSavedGame playGamesSavedGame)
            {
                return null;
            }

            ThrowIfCloudSaveNotEnabled();
            await playGamesSavedGame.OpenAsync(cancellationToken);

            var taskCompletionSource = new TaskCompletionSource<byte[]>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled(cancellationToken)))
            {
                PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(playGamesSavedGame.SavedGameMetadata, (status, bytes) =>
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

        public async Task<ICloudSaveGameMetadata> SaveBytesAsync(string name, byte[] bytes, CloudSaveGameMetadataUpdate metadataUpdate = null, CancellationToken cancellationToken = default)
        {
            ThrowIfCloudSaveNotEnabled();
            ISavedGameMetadata metadata = await OpenAsync(name, cancellationToken: cancellationToken);

            var taskCompletionSource = new TaskCompletionSource<ICloudSaveGameMetadata>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled(cancellationToken)))
            {
                var update = new SavedGameMetadataUpdate.Builder();
                if (metadataUpdate?.Description is string description)
                {
                    update.WithUpdatedDescription(description);
                }
                if (metadataUpdate?.TotalPlayTime is TimeSpan totalPlayTime)
                {
                    update.WithUpdatedPlayedTime(totalPlayTime);
                }
                PlayGamesPlatform.Instance.SavedGame.CommitUpdate(metadata, update.Build(), bytes, (status, metadata) =>
                {
                    if (metadata != null)
                    {
                        taskCompletionSource.TrySetResult(new PlayGamesSavedGame(metadata));
                    }
                    else
                    {
                        taskCompletionSource.TrySetException(new CloudSaveException($"Save error: {status}"));
                    }
                });
                return await taskCompletionSource.Task;
            }
        }

        public async Task<bool> DeleteGameAsync(string name, CancellationToken cancellationToken = default)
        {
            ThrowIfCloudSaveNotEnabled();
            ISavedGameMetadata savedGame = await GetExistingSavedGameAsync(name, cancellationToken);
            if (savedGame != null)
            {
                PlayGamesPlatform.Instance.SavedGame.Delete(savedGame);
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        private void ThrowIfCloudSaveNotEnabled()
        {
            if (!IsCloudSaveEnabled)
            {
                throw new CloudSaveNotEnabledException("Cloud save is not enabled: user is not logged in to Play Games");
            }
        }

        internal async Task<ISavedGameMetadata> GetExistingSavedGameAsync(string name, CancellationToken cancellationToken = default)
        {
            _savedGames ??= await FetchSavedGamesMetadataAsync(cancellationToken: cancellationToken);
            return _savedGames?.FirstOrDefault(x => x.Filename == name);
        }

        internal async Task<ISavedGameMetadata> OpenExistingAsync(string name, DataSource dataSource = DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy conflictResolutionStrategy = ConflictResolutionStrategy.UseMostRecentlySaved, CancellationToken cancellationToken = default)
        {
            ISavedGameMetadata savedGame = await GetExistingSavedGameAsync(name, cancellationToken);
            if (savedGame != null)
            {
                return await OpenAsync(name, dataSource, conflictResolutionStrategy, cancellationToken);
            }
            else
            {
                return null;
            }
        }

        internal static async Task<List<ISavedGameMetadata>> FetchSavedGamesMetadataAsync(DataSource dataSource = DataSource.ReadCacheOrNetwork, CancellationToken cancellationToken = default)
        {
            var taskCompletionSource = new TaskCompletionSource<List<ISavedGameMetadata>>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled(cancellationToken)))
            {
                PlayGamesPlatform.Instance.SavedGame.FetchAllSavedGames(dataSource, (status, savedGamesMetadata) =>
                {
                    if (savedGamesMetadata != null)
                    {
                        taskCompletionSource.TrySetResult(savedGamesMetadata);
                    }
                    else
                    {
                        taskCompletionSource.TrySetException(new CloudSaveException($"Fetch error: {status}"));
                    }
                });
                return await taskCompletionSource.Task;
            }
        }

        internal static async Task<ISavedGameMetadata> OpenAsync(string name, DataSource dataSource = DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy conflictResolutionStrategy = ConflictResolutionStrategy.UseMostRecentlySaved, CancellationToken cancellationToken = default)
        {
            var taskCompletionSource = new TaskCompletionSource<ISavedGameMetadata>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled(cancellationToken)))
            {
                PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(name, dataSource, conflictResolutionStrategy, (status, metadata) =>
                {
                    if (metadata != null)
                    {
                        taskCompletionSource.TrySetResult(metadata);
                    }
                    else
                    {
                        taskCompletionSource.TrySetException(new CloudSaveException($"Open error: {status}"));
                    }
                });
                return await taskCompletionSource.Task;
            }
        }
    }
}
#endif
