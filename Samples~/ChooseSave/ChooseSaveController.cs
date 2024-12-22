using System;
using System.Collections;
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

        private ICloudSaveProvider _cloudSaveProvider;

        void Start()
        {
#if UNITY_EDITOR
            _cloudSaveProvider = new EditorCloudSaveProvider();
#else
            _cloudSaveProvider = new DummyCloudSaveProvider();
#endif
            Social.localUser.Authenticate(success =>
            {
                RefreshSavedGames();
            });
        }

        public async void RefreshSavedGames()
        {
            List<ISavedGame> games = await _cloudSaveProvider.FetchSavedGamesAsync();
            foreach (ISavedGame game in games)
            {
                foreach (CloudSaveCell cell in _cells.Where(c => c.CloudSaveFileName == game.Name))
                {
                    cell.SavedGame = game;
                }
            }
        }

        public async void CreateSaveGame(CloudSaveCell cell)
        {
            cell.SavedGame = await _cloudSaveProvider.SaveGameAsync(cell.CloudSaveFileName, "");
        }

        public async void LoadSavedGame(CloudSaveCell cell)
        {
            _dataInput.text = await cell.SavedGame.LoadTextAsync();
        }

        public async void SaveGame(CloudSaveCell cell)
        {
            cell.SavedGame = await _cloudSaveProvider.SaveGameAsync(cell.CloudSaveFileName, _dataInput.text);
        }

        public async void DeleteGame(CloudSaveCell cell)
        {
            await _cloudSaveProvider.DeleteGameAsync(cell.CloudSaveFileName);
            cell.SavedGame = null;
        }
    }
}
