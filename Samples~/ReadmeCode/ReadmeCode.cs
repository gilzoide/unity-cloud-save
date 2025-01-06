using System.Collections.Generic;
using Gilzoide.CloudSave;
using Gilzoide.CloudSave.Providers;
using UnityEngine;

public class MyCloudSaveBehaviour : MonoBehaviour
{
    void Start()
    {
        // 1. Instantiate the wanted cloud save provider implementation.
        ICloudSaveProvider cloudSaveProvider;
#if UNITY_EDITOR
        // 1.a The Editor provider stores files in the "Library/Gilzoide.CloudSave" folder.
        // This is not the folder one would usually store game save data, so it functions
        // as a "cloud save" folder even after you clear regular save files or PlayerPrefs.
        cloudSaveProvider = new EditorCloudSaveProvider();
#elif UNITY_ANDROID && HAVE_GOOGLE_PLAY_GAMES
        // 1.b Android + Google Play Games Services provider.
        // Requires installing the Google Play Games plugin and setting it up first.
        // You need to define the `HAVE_GOOGLE_PLAY_GAMES` scripting symbol on Android.
        // Users must be logged in to Play Games for cloud save to work.
        cloudSaveProvider = new PlayGamesCloudSaveProvider();
#elif UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_TVOS || UNITY_VISIONOS
        // 1.c Apple Game Center provider (macOS, iOS, tvOS, visionOS).
        // Requires defining the Game Center capability in XCode project.
        // Requires defining the iCloud capability with the "iCloud Documents"
        // checkbox enabled and an iCloud container configured in XCode.
        // Users must be logged in to Game Center and iCloud for cloud save to work.
        cloudSaveProvider = new GameCenterCloudSaveProvider();
#else
        // 1.d The Dummy provider does nothing and exists for use in unsupported platforms.
        // Calling methods on it do not throw, but they also do not save anything.
        cloudSaveProvider = new DummyCloudSaveProvider();
#endif

#if UNITY_ANDROID && HAVE_GOOGLE_PLAY_GAMES
        // 2. Activate Play Games as the current Social provider in Unity.
        // Alternativelly, use Play Games API directly instead of UnityEngine.Social
        GooglePlayGames.PlayGamesPlatform.Activate();
#endif

        // 3. Make sure user is logged in to Play Games / Game Center before using cloud save.
        // This is not required by the Editor implementation.
        if (Social.localUser.authenticated)
        {
            DoSomethingWithCloudSave(cloudSaveProvider);
        }
        else
        {
            Social.localUser.Authenticate((success, message) =>
            {
                if (success)
                {
                    DoSomethingWithCloudSave(cloudSaveProvider);
                }
                else
                {
                    Debug.LogError($"Social authenticate error: {message}");
                }
            });
        }
    }

    async void DoSomethingWithCloudSave(ICloudSaveProvider cloudSaveProvider)
    {
        // 4. Check if cloud save is enabled before using cloud save functionality.
        // Some providers require logging in to a social account before using cloud save.
        // Providers will likely throw exceptions on operations when cloud save is not enabled.
        if (!cloudSaveProvider.IsCloudSaveEnabled)
        {
            return;
        }

        // 5. Now do something with the cloud save provider
        List<ICloudSaveGameMetadata> savedGames = await cloudSaveProvider.FetchAllAsync();
        foreach (ICloudSaveGameMetadata savedGame in savedGames)
        {
            Debug.Log($"Found saved game with name {savedGame.Name}");
            byte[] data = await cloudSaveProvider.LoadBytesAsync(savedGame);
        }

        await cloudSaveProvider.SaveTextAsync("SaveText", "Sample text data");
        await cloudSaveProvider.SaveBytesAsync("SaveBytes", new byte[] { 1, 2, 3, 4 });
        await cloudSaveProvider.SaveJsonAsync("SaveJson", new Vector3(1, 2, 3));

        string savedText = await cloudSaveProvider.LoadTextAsync("SaveText");
        byte[] savedBytes = await cloudSaveProvider.LoadBytesAsync("SaveBytes");
        Vector3 savedJson = await cloudSaveProvider.LoadJsonAsync<Vector3>("SaveJson");

        await cloudSaveProvider.DeleteAsync("SaveText");
    }
}
