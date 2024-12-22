#if UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_TVOS || UNITY_VISIONOS
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AOT;
using Gilzoide.CloudSave.Providers.Internal;

namespace Gilzoide.CloudSave.Providers
{
    public class GameCenterCloudSaveProvider : ICloudSaveProvider
    {
        [DllImport(GameCenterLibraryPaths.LibraryPath)] private static extern void Gilzoide_CloudSave_GameCenter_Fetch(IntPtr callback, IntPtr userdata);
        [DllImport(GameCenterLibraryPaths.LibraryPath)] private static extern void Gilzoide_CloudSave_GameCenter_Load([MarshalAs(UnmanagedType.LPStr)] string name, IntPtr callback, IntPtr userdata);
        [DllImport(GameCenterLibraryPaths.LibraryPath)] private static extern void Gilzoide_CloudSave_GameCenter_Save([MarshalAs(UnmanagedType.LPStr)] string name, IntPtr bytes, long bytesSize, IntPtr callback, IntPtr userdata);
        [DllImport(GameCenterLibraryPaths.LibraryPath)] private static extern void Gilzoide_CloudSave_GameCenter_Delete([MarshalAs(UnmanagedType.LPStr)] string name, IntPtr callback, IntPtr userdata);

        private delegate void FetchDelegate(IntPtr userdata, IntPtr savedGames, IntPtr error);
        private delegate void LoadDelegate(IntPtr userdata, IntPtr savedGame, IntPtr error);
        private delegate void SaveDelegate(IntPtr userdata, IntPtr savedGame, IntPtr error);
        private delegate void DeleteDelegate(IntPtr userdata, IntPtr error);

        public async Task<List<ISavedGame>> FetchSavedGamesAsync(CancellationToken cancellationToken = default)
        {
            var taskCompletionSource = new TaskCompletionSource<List<ISavedGame>>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled(cancellationToken)))
            {
                GCHandle gcHandle = GCHandle.Alloc(taskCompletionSource);
                Gilzoide_CloudSave_GameCenter_Fetch(OnFetchPtr, GCHandle.ToIntPtr(gcHandle));
                return await taskCompletionSource.Task;
            }
        }

        public async Task<ISavedGame> LoadGameAsync(string name, CancellationToken cancellationToken = default)
        {
            var taskCompletionSource = new TaskCompletionSource<ISavedGame>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled(cancellationToken)))
            {
                GCHandle gcHandle = GCHandle.Alloc(taskCompletionSource);
                Gilzoide_CloudSave_GameCenter_Load(name, OnLoadPtr, GCHandle.ToIntPtr(gcHandle));
                return await taskCompletionSource.Task;
            }
        }

        public async Task<ISavedGame> SaveGameAsync(string name, byte[] data, SaveGameMetadata metadata = null, CancellationToken cancellationToken = default)
        {
            var taskCompletionSource = new TaskCompletionSource<ISavedGame>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled(cancellationToken)))
            {
                GCHandle gcHandle = GCHandle.Alloc(taskCompletionSource);
                unsafe
                {
                    fixed (byte* bytes = data)
                    {
                        Gilzoide_CloudSave_GameCenter_Save(name, (IntPtr)bytes, data.LongLength, OnSavePtr, GCHandle.ToIntPtr(gcHandle));
                    }
                }
                return await taskCompletionSource.Task;
            }
        }

        public async Task<bool> DeleteGameAsync(string name, CancellationToken cancellationToken = default)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled(cancellationToken)))
            {
                GCHandle gcHandle = GCHandle.Alloc(taskCompletionSource);
                Gilzoide_CloudSave_GameCenter_Delete(name, OnDeletePtr, GCHandle.ToIntPtr(gcHandle));
                return await taskCompletionSource.Task;
            }
        }

        [MonoPInvokeCallback(typeof(FetchDelegate))]
        private static void OnFetch(IntPtr userdata, IntPtr savedGamesPtr, IntPtr errorPtr)
        {
            GCHandle gcHandle = GCHandle.FromIntPtr(userdata);
            var taskCompletionSource = (TaskCompletionSource<List<ISavedGame>>) gcHandle.Target;
            gcHandle.Free();

            if (errorPtr != IntPtr.Zero)
            {
                var error = new CFErrorRef(errorPtr);
                taskCompletionSource.TrySetException(new CloudSaveException(error.ToString()));
            }
            else if (savedGamesPtr != IntPtr.Zero)
            {
                var savedGames = new CFArrayRef(savedGamesPtr);
                var result = new List<ISavedGame>();
                for (int i = 0, count = savedGames.Length; i < count; i++)
                {
                    var game = new GameCenterSavedGame(new GKSavedGameRef(savedGames[i]));
                    result.Add(game);
                }
                taskCompletionSource.TrySetResult(result);
            }
            else
            {
                taskCompletionSource.TrySetResult(null);
            }
        }
        private static readonly IntPtr OnFetchPtr = Marshal.GetFunctionPointerForDelegate<FetchDelegate>(OnFetch);

        [MonoPInvokeCallback(typeof(LoadDelegate))]
        private static void OnLoad(IntPtr userdata, IntPtr savedGamePtr, IntPtr errorPtr)
        {
            GCHandle gcHandle = GCHandle.FromIntPtr(userdata);
            var taskCompletionSource = (TaskCompletionSource<ISavedGame>) gcHandle.Target;
            gcHandle.Free();

            if (errorPtr != IntPtr.Zero)
            {
                var error = new CFErrorRef(errorPtr);
                taskCompletionSource.TrySetException(new CloudSaveException(error.ToString()));
            }
            else if (savedGamePtr != IntPtr.Zero)
            {
                var game = new GameCenterSavedGame(new GKSavedGameRef(savedGamePtr));
                taskCompletionSource.TrySetResult(game);
            }
            else
            {
                taskCompletionSource.TrySetResult(null);
            }
        }
        private static readonly IntPtr OnLoadPtr = Marshal.GetFunctionPointerForDelegate<LoadDelegate>(OnLoad);

        [MonoPInvokeCallback(typeof(SaveDelegate))]
        private static void OnSave(IntPtr userdata, IntPtr savedGamePtr, IntPtr errorPtr)
        {
            GCHandle gcHandle = GCHandle.FromIntPtr(userdata);
            var taskCompletionSource = (TaskCompletionSource<ISavedGame>) gcHandle.Target;
            gcHandle.Free();

            if (errorPtr != IntPtr.Zero)
            {
                var error = new CFErrorRef(errorPtr);
                taskCompletionSource.TrySetException(new CloudSaveException(error.ToString()));
            }
            else if (savedGamePtr != IntPtr.Zero)
            {
                var game = new GameCenterSavedGame(new GKSavedGameRef(savedGamePtr));
                taskCompletionSource.TrySetResult(game);
            }
            else
            {
                taskCompletionSource.TrySetResult(null);
            }
        }
        private static readonly IntPtr OnSavePtr = Marshal.GetFunctionPointerForDelegate<SaveDelegate>(OnSave);

        [MonoPInvokeCallback(typeof(DeleteDelegate))]
        private static void OnDelete(IntPtr userdata, IntPtr errorPtr)
        {
            GCHandle gcHandle = GCHandle.FromIntPtr(userdata);
            var taskCompletionSource = (TaskCompletionSource<bool>) gcHandle.Target;
            gcHandle.Free();

            if (errorPtr != IntPtr.Zero)
            {
                var error = new CFErrorRef(errorPtr);
                taskCompletionSource.TrySetException(new CloudSaveException(error.ToString()));
            }
            else
            {
                taskCompletionSource.TrySetResult(true);
            }
        }
        private static readonly IntPtr OnDeletePtr = Marshal.GetFunctionPointerForDelegate<DeleteDelegate>(OnDelete);
    }
}
#endif
