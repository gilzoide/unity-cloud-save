# Cloud Save
Cloud Save interface with implementations for the Unity Editor, [Apple's Game Center](https://developer.apple.com/documentation/gamekit/saving-the-player-s-game-data-to-an-icloud-account) and [Google Play Games Services](https://developer.android.com/games/pgs/savedgames).

Future implementations might include [Unity Gaming Services](https://docs.unity.com/ugs/manual/cloud-save/manual), [Epic Online Services](https://dev.epicgames.com/docs/game-services/player-data-storage), [Steam Cloud](https://partner.steamgames.com/doc/features/cloud) and [Firebase Cloud Storage](https://firebase.google.com/docs/storage/unity/start).

> Note from *gilzoide*: since I work as a mobile game developer, iOS and Android support is enough for my needs.
> I will likely not work on the suggested future implementations in the mid term, unless this work is sponsored.


## Features
- Simple interface with async methods for fetching, loading, saving and deleting cloud save games
- Single interface for all implementations, for easy multiplatform development
- Supports the Unity Editor by saving files in the "Library/Gilzoide.CloudSave" folder
- Supports macOS / iOS / tvOS / visionOS platforms using Game Center + iCloud
- Supports Android platforms using Google Play Games Services


## TODO
- Store additional metadata in Game Center provider, retrieve last save date
- Add support for resolving conflicts between cloud saved games
- Add a way to automate Game Center / iCloud capabilities in XCode project?


## How to install
Either:
- Install using the [Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui-giturl.html) with the following URL:
  ```
  https://github.com/gilzoide/unity-cloud-save.git#1.0.0-preview1
  ```
- Clone this repository or download a snapshot of it directly inside your project's `Assets` or `Packages` folder.


## Usage example
```cs
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
        // 1.a The Editor provider stores "cloud saves" in the "Library/Gilzoide.CloudSave" folder.
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
        // 1.d The Dummy provider does absolutely nothing and exists for unsupported platforms.
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
        List<ICloudSaveGameMetadata> savedGames = await cloudSaveProvider.FetchSavedGamesAsync();
        foreach (ICloudSaveGameMetadata savedGame in savedGames)
        {
            Debug.Log($"Found saved game with name {savedGame.Name}");
        }

        await cloudSaveProvider.SaveTextAsync("SaveText", "Sample text data");
        await cloudSaveProvider.SaveBytesAsync("SaveBytes", new byte[] { 1, 2, 3, 4 });
        await cloudSaveProvider.SaveJsonAsync("SaveJson", new Vector3(1, 2, 3));

        string savedText = await cloudSaveProvider.LoadTextAsync("SaveText");
        byte[] savedBytes = await cloudSaveProvider.LoadBytesAsync("SaveBytes");
        Vector3 savedJson = await cloudSaveProvider.LoadJsonAsync<Vector3>("SaveJson");

        await cloudSaveProvider.DeleteGameAsync("SaveText");
    }
}
```


## Game Center
- Game Center is supported on macOS, iOS, tvOS and visionOS platforms.
- You must add the Game Center capability to the XCode project.
- You must add the iCloud capability to the XCode project.
  The "iCloud Documents" checkbox must be enabled and an iCloud container must be configured for the cloud save to work.
- The user must be signed in to Game Center and have iCloud Drive enabled in their account for cloud save to work.


## Google Play Games Services
- Play Games is supported on Android platforms
- You must install the [Google Play Games plugin for Unity](https://github.com/playgameservices/play-games-plugin-for-unity) in your project.
  If you use an old version of the plugin that does not come with Assembly Definition files, create the `Google.Play.Games` asmdef in the plugin's root folder, as well as the `Google.Play.Games.Editor` editor-only asmdef on the plugin's "Editor" folder.
- You must define the `HAVE_GOOGLE_PLAY_GAMES` [custom scripting symbol](https://docs.unity3d.com/Manual/custom-scripting-symbols.html).
  This package needs this to avoid compilation errors when Google Play Games plugin is not installed.
- The user must be signed in to Play Games for the cloud save to work
