# Cloud Save
Cloud Save interface with implementations for the Unity Editor, [Apple's Game Center](https://developer.apple.com/documentation/gamekit/saving-the-player-s-game-data-to-an-icloud-account) and [Google Play Games Services](https://developer.android.com/games/pgs/savedgames).

Future implementations might include [Unity Gaming Services](https://docs.unity.com/ugs/manual/cloud-save/manual), [Epic Online Services](https://dev.epicgames.com/docs/game-services/player-data-storage) and [Steam Cloud](https://partner.steamgames.com/doc/features/cloud).

> Note from *gilzoide*: since I work as a mobile game developer, iOS and Android support is enough for my needs.
> I will likely not work on the suggested future implementations in the mid term, unless this work is sponsored.


## Features
- Simple interface with async methods for fetching, loading, saving and deleting cloud save games
- Single interface for all implementations, for easy multiplatform development
- Supports the Unity Editor by saving files in the "Library/Gilzoide.CloudSave" folder
- Supports macOS / iOS / tvOS / visionOS platforms using Game Center + iCloud
- Supports Android platforms using Google Play Games Services


## TODO
- Store additional metadata in Game Center provider
- Add support for resolving conflicts between cloud saved games
- Add a way to automate Game Center / iCloud capabilities in XCode project?


## How to install
Either:
- Install using the [Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui-giturl.html) with the following URL:
  ```
  https://github.com/gilzoide/unity-cloud-save.git
  ```
- Clone this repository or download a snapshot of it directly inside your project's `Assets` or `Packages` folder.


## Game Center
- Game Center is supported on macOS, iOS, tvOS and visionOS platforms.
- You must add the Game Center capability to the XCode project.
- You must add the iCloud capability to the XCode project.
  The "iCloud Documents" checkbox must be enabled and an iCloud container must be configured for the cloud save to work.
- The user must be signed in to Game Center and have iCloud Drive enabled in their account for cloud save to work.

<details>
<summary><b>Example MonoBehaviour code</b></summary>

```cs
using Gilzoide.CloudSave.Providers;
using UnityEngine;

public class MyGameCenterCloudBehaviour : MonoBehaviour
{
    void Start()
    {
        // 1. Make sure user is logged in to Game Center
        if (Social.localUser.authenticated)
        {
            DoSomethingWithCloudSave();
        }
        else
        {
            Social.localUser.Authenticate((success, message) =>
            {
                if (success)
                {
                    DoSomethingWithCloudSave();
                }
                else
                {
                    Debug.LogError($"Social authenticate error: {message}");
                }
            });
        }
    }

    async void DoSomethingWithCloudSave()
    {
        // 2. Instantiate the Game Center ICloudSaveProvider implementation
        ICloudSaveProvider cloudSaveProvider = new GameCenterCloudSaveProvider();

        // 3. Do something (all calls are async)
        await cloudSaveProvider.SaveGameAsync("SaveText", "Sample text data");
        await cloudSaveProvider.SaveGameAsync("SaveBytes", new byte[] { 1, 2, 3, 4 });

        string savedText = await cloudSaveProvider.LoadTextAsync("SaveText");
        byte[] savedBytes = await cloudSaveProvider.LoadDataAsync("SaveBytes");

        await cloudSaveProvider.DeleteGameAsync("SaveText");
    }
}
```
</details>


## Google Play Games Services
- Play Games is supported on Android platforms
- You must install the [Google Play Games plugin for Unity](https://github.com/playgameservices/play-games-plugin-for-unity) in your project.
  If you use an old version of the plugin that does not come with Assembly Definition files, create the `Google.Play.Games` asmdef in the plugin's root folder, as well as the `Google.Play.Games.Editor` editor-only asmdef on the plugin's "Editor" folder.
- You must define the `HAVE_GOOGLE_PLAY_GAMES` [custom scripting symbol](https://docs.unity3d.com/Manual/custom-scripting-symbols.html).
  This package needs this to avoid compilation errors when Google Play Games plugin is not installed.
- The user must be signed in to Play Games for the cloud save to work

<details>
<summary><b>Example MonoBehaviour code</b></summary>

```cs
using Gilzoide.CloudSave.Providers;
using UnityEngine;

public class MyGameCenterCloudBehaviour : MonoBehaviour
{
    void Start()
    {
        // 1. Activate Play Games as the current Social provider in Unity
        // Alternativelly, use Play Games API directly instead of UnityEngine.Social
        GooglePlayGames.PlayGamesPlatform.Activate();
        // 2. Make sure user is logged in to Play Games
        if (Social.localUser.authenticated)
        {
            DoSomethingWithCloudSave();
        }
        else
        {
            Social.localUser.Authenticate((success, message) =>
            {
                if (success)
                {
                    DoSomethingWithCloudSave();
                }
                else
                {
                    Debug.LogError($"Social authenticate error: {message}");
                }
            });
        }
    }

    async void DoSomethingWithCloudSave()
    {
        // 3. Instantiate the Play Games ICloudSaveProvider implementation
        ICloudSaveProvider cloudSaveProvider = new PlayGamesCloudSaveProvider();

        // 4. Do something (all calls are async)
        await cloudSaveProvider.SaveGameAsync("SaveText", "Sample text data");
        await cloudSaveProvider.SaveGameAsync("SaveBytes", new byte[] { 1, 2, 3, 4 });

        string savedText = await cloudSaveProvider.LoadTextAsync("SaveText");
        byte[] savedBytes = await cloudSaveProvider.LoadDataAsync("SaveBytes");

        await cloudSaveProvider.DeleteGameAsync("SaveText");
    }
}
```
</details>


## Multiplatform development
Since all implementations use the same interface, one only needs to instantiate the correct implementation and all following code can be maintained.

Example MonoBehaviour code:
```cs
using Gilzoide.CloudSave.Providers;
using UnityEngine;

public class MyGameCenterCloudBehaviour : MonoBehaviour
{
    void Start()
    {
#if UNITY_ANDROID && HAVE_GOOGLE_PLAY_GAMES
        GooglePlayGames.PlayGamesPlatform.Activate();
#endif
        if (Social.localUser.authenticated)
        {
            DoSomethingWithCloudSave();
        }
        else
        {
            Social.localUser.Authenticate((success, message) =>
            {
                if (success)
                {
                    DoSomethingWithCloudSave();
                }
                else
                {
                    Debug.LogError($"Social authenticate error: {message}");
                }
            });
        }
    }

    async void DoSomethingWithCloudSave()
    {
        ICloudSaveProvider cloudSaveProvider;
        // HERE: instantiate the correct implementation and everything else can be the same
#if UNITY_EDITOR
        cloudSaveProvider = new EditorCloudSaveProvider();
#elif UNITY_ANDROID && HAVE_GOOGLE_PLAY_GAMES
        cloudSaveProvider = new PlayGamesCloudSaveProvider();
#elif UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_TVOS || UNITY_VISIONOS
        cloudSaveProvider = new GameCenterCloudSaveProvider();
#else
        cloudSaveProvider = new DummyCloudSaveProvider();
#endif

        await cloudSaveProvider.SaveGameAsync("SaveText", "Sample text data");
        await cloudSaveProvider.SaveGameAsync("SaveBytes", new byte[] { 1, 2, 3, 4 });

        string savedText = await cloudSaveProvider.LoadTextAsync("SaveText");
        byte[] savedBytes = await cloudSaveProvider.LoadDataAsync("SaveBytes");

        await cloudSaveProvider.DeleteGameAsync("SaveText");
    }
}
```
