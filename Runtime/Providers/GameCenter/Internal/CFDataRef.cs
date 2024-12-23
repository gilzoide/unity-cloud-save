#if UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_TVOS || UNITY_VISIONOS
using System;
using System.Runtime.InteropServices;
using CFIndex = System.Int64;

namespace Gilzoide.CloudSave.Providers.Internal
{
    internal readonly struct CFDataRef
    {
        [DllImport(GameCenterLibraryPaths.CoreFoundationLibraryPath)] private static extern IntPtr CFDataGetBytePtr(IntPtr data);
        [DllImport(GameCenterLibraryPaths.CoreFoundationLibraryPath)] private static extern CFIndex CFDataGetLength(IntPtr data);

        private readonly IntPtr _nativeHandle;

        public readonly long LongLength => CFDataGetLength(_nativeHandle);
        public readonly int Length => (int) CFDataGetLength(_nativeHandle);
        public readonly IntPtr BytePtr => CFDataGetBytePtr(_nativeHandle);

        public CFDataRef(IntPtr handle)
        {
            _nativeHandle = handle;
        }

        public readonly byte[] ToBytes()
        {
            var bytes = new byte[LongLength];
            Marshal.Copy(BytePtr, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
#endif
