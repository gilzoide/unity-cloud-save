#if UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_TVOS || UNITY_VISIONOS
using System;
using System.Runtime.InteropServices;

namespace Gilzoide.CloudSave.Providers.Internal
{
    internal struct CFErrorRef
    {
        [DllImport(GameCenterLibraryPaths.LibraryPath)] private static extern string Gilzoide_CloudSave_GameCenter_ErrorToString(IntPtr error);

        private IntPtr _nativeHandle;

        public CFErrorRef(IntPtr handle)
        {
            _nativeHandle = handle;
        }

        public override string ToString()
        {
            return Gilzoide_CloudSave_GameCenter_ErrorToString(_nativeHandle);
        }
    }
}
#endif
