#if UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_TVOS || UNITY_VISIONOS
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AOT;

namespace Gilzoide.CloudSave.Providers.Internal
{
    internal struct GKSavedGameRef : IDisposable
    {
        [DllImport(GameCenterLibraryPaths.LibraryPath)] private static extern string Gilzoide_CloudSave_GameCenter_SavedGameName(IntPtr savedGame);
        [DllImport(GameCenterLibraryPaths.LibraryPath)] private static extern void Gilzoide_CloudSave_GameCenter_SavedGameLoad(IntPtr savedGame, IntPtr callback, IntPtr userdata);
        [DllImport(GameCenterLibraryPaths.CoreFoundationLibraryPath)] private static extern IntPtr CFRetain(IntPtr cf);
        [DllImport(GameCenterLibraryPaths.CoreFoundationLibraryPath)] private static extern void CFRelease(IntPtr cf);

        private delegate void SavedGameLoadDelegate(IntPtr userdata, IntPtr data, IntPtr error);

        private IntPtr _nativeHandle;

        public string Name => Gilzoide_CloudSave_GameCenter_SavedGameName(_nativeHandle);

        public GKSavedGameRef(IntPtr handle)
        {
            if (handle != IntPtr.Zero)
            {
                handle = CFRetain(handle);
            }
            _nativeHandle = handle;
        }

        public void Dispose()
        {
            if (_nativeHandle != IntPtr.Zero)
            {
                CFRelease(_nativeHandle);
                _nativeHandle = IntPtr.Zero;
            }
        }

        public async Task<byte[]> LoadAsync(CancellationToken cancellationToken = default)
        {
            var taskCompletionSource = new TaskCompletionSource<byte[]>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled(cancellationToken)))
            {
                GCHandle gcHandle = GCHandle.Alloc(taskCompletionSource);
                Gilzoide_CloudSave_GameCenter_SavedGameLoad(_nativeHandle, OnSavedGameLoadPtr, GCHandle.ToIntPtr(gcHandle));
                return await taskCompletionSource.Task;
            }
        }

        #region Native Callbacks

        [MonoPInvokeCallback(typeof(SavedGameLoadDelegate))]
        private static void OnSavedGameLoad(IntPtr userdata, IntPtr dataPtr, IntPtr errorPtr)
        {
            GCHandle gcHandle = GCHandle.FromIntPtr(userdata);
            var taskCompletionSource = (TaskCompletionSource<byte[]>) gcHandle.Target;
            gcHandle.Free();

            if (errorPtr != IntPtr.Zero)
            {
                var error = new CFErrorRef(errorPtr);
                taskCompletionSource.TrySetException(new CloudSaveException(error.ToString()));
            }
            else
            {
                var data = new CFDataRef(dataPtr);
                byte[] bytes = data.ToBytes();
                taskCompletionSource.TrySetResult(bytes);
            }
        }
        private static readonly IntPtr OnSavedGameLoadPtr = Marshal.GetFunctionPointerForDelegate<SavedGameLoadDelegate>(OnSavedGameLoad);

        #endregion
    }
}
#endif
