namespace Gilzoide.CloudSave.Providers.Internal
{
    internal static class GameCenterLibraryPaths
    {
#if UNITY_EDITOR || UNITY_STANDALONE_OSX
        public const string LibraryPath = "gilzoide.cloudsave.apple";
#else
        public const string LibraryPath = "__Internal";
#endif

        public const string CoreFoundationLibraryPath = "__Internal";
    }
}
