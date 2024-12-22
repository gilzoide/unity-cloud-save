#if UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_TVOS || UNITY_VISIONOS
using System;
using System.Runtime.InteropServices;
using CFIndex = System.Int64;

namespace Gilzoide.CloudSave.Providers.Internal
{
    internal struct CFArrayRef
    {
        [DllImport(GameCenterLibraryPaths.CoreFoundationLibraryPath)] private static extern CFIndex CFArrayGetCount(IntPtr array);
        [DllImport(GameCenterLibraryPaths.CoreFoundationLibraryPath)] private static extern IntPtr CFArrayGetValueAtIndex(IntPtr array, CFIndex index);

        private readonly IntPtr _nativeHandle;

        public long LongLength => CFArrayGetCount(_nativeHandle);
        public int Length => (int) CFArrayGetCount(_nativeHandle);
        public IntPtr this[int index] => CFArrayGetValueAtIndex(_nativeHandle, index);

        public CFArrayRef(IntPtr ptr)
        {
            _nativeHandle = ptr;
        }
    }
}
#endif
