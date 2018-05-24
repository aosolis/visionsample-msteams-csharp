namespace VisionSample.Api
{
    using System.Threading.Tasks;
    using VisonSample.Api.Models;

    public interface IVisionApi
    {
        Task<DescribeImageResult> DescribeImageAsync(string image, string language = "en", int maxCandidates = 1);

        Task<DescribeImageResult> DescribeImageAsync(byte[] image, string language = "en", int maxCandidates = 1);

        Task<OcrResult> RunOcrAsync(string image);

        Task<OcrResult> RunOcrAsync(byte[] image);
    }
}
