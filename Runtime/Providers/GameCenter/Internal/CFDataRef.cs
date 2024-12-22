#if UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_TVOS || UNITY_VISIONOS
using System;
using System.Runtime.InteropServices;
using CFIndex = System.Int64;

namespace Gilzoide.CloudSave.Providers.Internal
{
    internal struct CFDataRef
    {
        [DllImport(GameCenterLibraryPaths.CoreFoundationLibraryPath)] private static extern IntPtr CFDataGetBytePtr(IntPtr data);
        [DllImport(GameCenterLibraryPaths.CoreFoundationLibraryPath)] private static extern CFIndex CFDataGetLength(IntPtr data);

        private IntPtr _nativeHandle;

        public long LongLength => CFDataGetLength(_nativeHandle);
        public int Length => (int) CFDataGetLength(_nativeHandle);
        public IntPtr BytePtr => CFDataGetBytePtr(_nativeHandle);

        public CFDataRef(IntPtr handle)
        {
            _nativeHandle = handle;
        }

        public byte[] ToBytes()
        {
            var bytes = new byte[LongLength];
            Marshal.Copy(BytePtr, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
#endif
