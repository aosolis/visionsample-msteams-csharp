namespace VisionSample.Api
{
    using System;

    public class AzureVisionApiException : Exception
    {
        public AzureVisionApiException()
        {
        }

        public AzureVisionApiException(string message) : base(message)
        {
        }

        public AzureVisionApiException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public string ErrorCode { get; set; }

        public string RequestId { get; set; }
    }
}
