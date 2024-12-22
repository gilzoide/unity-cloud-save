using System.Collections.Generic;
using System.Linq;
using Gilzoide.CloudSave.Providers;
using UnityEngine;
using UnityEngine.UI;

namespace Gilzoide.CloudSave.Samples.ChooseSave
{
    public class ChooseSaveController : MonoBehaviour
    {
        [SerializeField] private List<CloudSaveCell> _cells;
        [SerializeField] private InputField _dataInput;
        [SerializeField] private GameObject _loadingOverlay;

        private ICloudSaveProvider _cloudSaveProvider;

        private void Start()
        {
#if UNITY_EDITOR
            _cloudSaveProvider = new EditorCloudSaveProvider();
#elif UNITY_ANDROID && HAVE_GOOGLE_PLAY_GAMES
            GooglePlayGames.PlayGamesPlatform.Activate();
            _cloudSaveProvider = new PlayGamesCloudSaveProvider();
#elif UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_TVOS || UNITY_VISIONOS
            _cloudSaveProvider = new GameCenterCloudSaveProvider();
#else
            _cloudSaveProvider = new DummyCloudSaveProvider();
#endif
            _loadingOverlay.SetActive(true);
            if (Social.localUser.authenticated)
            {
                RefreshSavedGames();
            }
            else
            {
                Social.localUser.Authenticate((success, message) =>
                {
                    if (!success)
                    {
                        Debug.LogError($"Social authenticate error: {message}");
                    }
                    RefreshSavedGames();
                });
            }
        }

        public async void RefreshSavedGames()
        {
            try
            {
                List<ISavedGame> games = await _cloudSaveProvider.FetchSavedGamesAsync();
                foreach (ISavedGame game in games)
                {
                    foreach (CloudSaveCell cell in _cells.Where(c => c.CloudSaveFileName == game.Name))
                    {
                        cell.SavedGame = game;
                    }
                }
                Debug.Log("[ChooseSaveController] Fetched existing games");
            }
            finally
            {
                _loadingOverlay.SetActive(false);
            }
        }

        public async void CreateSaveGame(CloudSaveCell cell)
        {
            cell.SavedGame = await _cloudSaveProvider.SaveGameAsync(cell.CloudSaveFileName, ".");
            Debug.Log($"[ChooseSaveController] Game created: {cell.CloudSaveFileName}");
        }

        public async void LoadSavedGame(CloudSaveCell cell)
        {
            _dataInput.text = await cell.SavedGame.LoadTextAsync();
            Debug.Log($"[ChooseSaveController] Game loaded: {cell.CloudSaveFileName}");
        }

        public async void SaveGame(CloudSaveCell cell)
        {
            cell.SavedGame = await _cloudSaveProvider.SaveGameAsync(cell.CloudSaveFileName, _dataInput.text);
            Debug.Log($"[ChooseSaveController] Game saved: {cell.CloudSaveFileName}");
        }

        public async void DeleteGame(CloudSaveCell cell)
        {
            await _cloudSaveProvider.DeleteGameAsync(cell.CloudSaveFileName);
            cell.SavedGame = null;
            Debug.Log($"[ChooseSaveController] Game deleted: {cell.CloudSaveFileName}");
        }
    }
}
