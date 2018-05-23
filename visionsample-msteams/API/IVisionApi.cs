namespace VisionSample.API
{
    using System.Threading.Tasks;
    using VisonSample.API.Models;

    public interface IVisionApi
    {
        Task<DescribeImageResult> DescribeImageAsync(string image, string language = "en", int maxCandidates = 1);

        Task<DescribeImageResult> DescribeImageAsync(byte[] image, string language = "en", int maxCandidates = 1);

        Task<OcrResult> RunOcrAsync(string image, string language = "en");

        Task<OcrResult> RunOcrAsync(byte[] image, string language = "en");
    }
}
