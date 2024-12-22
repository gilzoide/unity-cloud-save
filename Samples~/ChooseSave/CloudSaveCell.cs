using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gilzoide.CloudSave.Samples.ChooseSave
{
    public class CloudSaveCell : MonoBehaviour
    {
        [SerializeField] private string _cloudSaveFileName;

        [Header("UI")]
        [SerializeField] private Text _titleText;
        [SerializeField] private Button _createButton;
        [SerializeField] private Button _loadButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _deleteButton;

        [Header("Events")]
        public UnityEvent<CloudSaveCell> OnCreateSave;
        public UnityEvent<CloudSaveCell> OnLoad;
        public UnityEvent<CloudSaveCell> OnSave;
        public UnityEvent<CloudSaveCell> OnDeleteSave;

        private ISavedGame _savedGame;

        public ISavedGame SavedGame
        {
            get => _savedGame;
            set
            {
                _savedGame = value;
                RefreshLayout();
            }
        }

        public string CloudSaveFileName
        {
            get => _cloudSaveFileName;
            set
            {
                _cloudSaveFileName = value;
                _titleText.text = value;
            }
        }

        private void Start()
        {
            _titleText.text = _cloudSaveFileName;
            _createButton.onClick.AddListener(() => OnCreateSave.Invoke(this));
            _loadButton.onClick.AddListener(() => OnLoad.Invoke(this));
            _saveButton.onClick.AddListener(() => OnSave.Invoke(this));
            _deleteButton.onClick.AddListener(() => OnDeleteSave.Invoke(this));
        }

        public void RefreshLayout()
        {
            bool hasSave = SavedGame != null;
            _createButton.gameObject.SetActive(!hasSave);
            _loadButton.gameObject.SetActive(hasSave);
            _saveButton.gameObject.SetActive(hasSave);
            _deleteButton.gameObject.SetActive(hasSave);
        }
    }
}
