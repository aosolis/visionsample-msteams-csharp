namespace VisionSample.Api
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using VisonSample.Api.Models;

    public class AzureVisionApi : IVisionApi
    {
        // Service endpoint paths
        public const string DescribePath = "vision/v2.0/describe";
        public const string OcrPath = "vision/v2.0/ocr";

        // Common headers
        public const string SubscriptionKeyHeaderName = "Ocp-Apim-Subscription-Key";

        private HttpClient httpClient;

        public AzureVisionApi(string endpoint, string accessKey, HttpMessageHandler httpHandler = null)
        {
            this.httpClient = new HttpClient(httpHandler ?? new HttpClientHandler());
            this.httpClient.DefaultRequestHeaders.Add(SubscriptionKeyHeaderName, accessKey);
            this.httpClient.BaseAddress = new Uri($"https://{endpoint}/");
        }

        public async Task<DescribeImageResult> DescribeImageAsync(string imageUrl, string language = "en", int maxCandidates = 1)
        {
            var url = $"{DescribePath}?language={language}&maxCandidates={maxCandidates}";

            var result = await this.httpClient.PostAsJsonAsync(url, new DescribeImageRequest { Url = imageUrl });
            result.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<DescribeImageResult>(await result.Content.ReadAsStringAsync());
        }

        public Task<DescribeImageResult> DescribeImageAsync(byte[] image, string language = "en", int maxCandidates = 1)
        {
            throw new NotImplementedException();
        }

        public Task<OcrResult> RunOcrAsync(string image, string language = "en")
        {
            throw new NotImplementedException();
        }

        public Task<OcrResult> RunOcrAsync(byte[] image, string language = "en")
        {
            throw new NotImplementedException();
        }
    }
}
