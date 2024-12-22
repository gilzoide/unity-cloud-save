namespace Gilzoide.CloudSave.Providers.Internal
{
    public static class GameCenterLibraryPaths
    {
#if UNITY_EDITOR || UNITY_STANDALONE_OSX
        public const string LibraryPath = "gilzoide.cloudsave.gamecenter";
#else
        public const string LibraryPath = "__Internal";
#endif

        public const string CoreFoundationLibraryPath = "__Internal";
    }
}
