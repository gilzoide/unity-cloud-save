using System;

namespace Gilzoide.CloudSave
{
    public class CloudSaveNotEnabledException : CloudSaveException
    {
        public CloudSaveNotEnabledException(string message) : base(message) {}
        public CloudSaveNotEnabledException(string message, Exception innerException) : base(message, innerException) {}
    }
}
