using System;

namespace Gilzoide.CloudSave
{
    public class CloudSaveException : Exception
    {
        public CloudSaveException(string message) : base(message) {}
        public CloudSaveException(string message, Exception innerException) : base(message, innerException) {}
    }
}
