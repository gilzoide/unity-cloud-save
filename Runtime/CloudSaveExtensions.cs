using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gilzoide.CloudSave
{
    public static class CloudSaveExtensions
    {
        public static Task<ISavedGame> SaveGameAsync(this ICloudSaveProvider cloudSaveProvider, string name, string text, Encoding encoding = null, SaveGameMetadata metadata = null, CancellationToken cancellationToken = default)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            byte[] bytes = encoding.GetBytes(text);
            return cloudSaveProvider.SaveGameAsync(name, bytes, metadata, cancellationToken);
        }

        public static async Task<string> LoadTextAsync(this ISavedGame savedGame, Encoding encoding = null, CancellationToken cancellationToken = default)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            byte[] bytes = await savedGame.LoadDataAsync(cancellationToken);
            return encoding.GetString(bytes);
        }
    }
}
