#if UNITY_EDITOR
namespace Gilzoide.CloudSave.Providers
{
    public class EditorCloudSaveProvider : FileCloudSaveProvider
    {
        public EditorCloudSaveProvider() : base("Library/Gilzoide.CloudSave")
        {
        }
    }
}
#endif
