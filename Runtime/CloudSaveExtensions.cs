using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gilzoide.CloudSave
{
    public static class CloudSaveExtensions
    {
        public static Task<ISavedGame> SaveGameAsync(this ICloudSaveProvider cloudSaveProvider, string name, string text, Encoding encoding = null, SaveGameMetadata metadata = null, CancellationToken cancellationToken = default)
        {
            byte[] bytes = (encoding ?? Encoding.UTF8).GetBytes(text);
            return cloudSaveProvider.SaveGameAsync(name, bytes, metadata, cancellationToken);
        }

        public static async Task<string> LoadTextAsync(this ISavedGame savedGame, Encoding encoding = null, CancellationToken cancellationToken = default)
        {
            byte[] bytes = await savedGame.LoadDataAsync(cancellationToken);
            return (encoding ?? Encoding.UTF8).GetString(bytes);
        }

        public static async Task<byte[]> LoadDataAsync(this ICloudSaveProvider cloudSaveProvider, string name, CancellationToken cancellationToken = default)
        {
            ISavedGame savedGame = await cloudSaveProvider.LoadGameAsync(name, cancellationToken);
            return await savedGame?.LoadDataAsync(cancellationToken);
        }

        public static async Task<string> LoadTextAsync(this ICloudSaveProvider cloudSaveProvider, string name, Encoding encoding = null, CancellationToken cancellationToken = default)
        {
            ISavedGame savedGame = await cloudSaveProvider.LoadGameAsync(name, cancellationToken);
            return await savedGame?.LoadTextAsync(encoding, cancellationToken);
        }
    }
}
