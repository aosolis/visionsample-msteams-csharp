namespace VisionSample.Api
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
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

        private static readonly JsonMediaTypeFormatter JsonFormatter = new JsonMediaTypeFormatter();

        private string endpoint;
        private string accessKey;
        private HttpClient httpClient;

        public AzureVisionApi(string endpoint, string accessKey, HttpClient httpClient = null)
        {
            this.endpoint = endpoint;
            this.accessKey = accessKey;
            this.httpClient = httpClient ?? new HttpClient();
        }

        public async Task<DescribeImageResult> DescribeImageAsync(string imageUrl, string language = "en", int maxCandidates = 1)
        {
            var url = $"https://{endpoint}/{DescribePath}?language={language}&maxCandidates={maxCandidates}";

            var request = new HttpRequestMessage(HttpMethod.Post, $"https://{this.endpoint}/{DescribePath}?language={language}&maxCandidates={maxCandidates}");
            request.Headers.Add(SubscriptionKeyHeaderName, this.accessKey);

            var body = new DescribeImageRequest { Url = imageUrl };
            request.Content = new ObjectContent<DescribeImageRequest>(body, JsonFormatter);

            var response = await this.httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<DescribeImageResult>(await response.Content.ReadAsStringAsync());
        }

        public async Task<DescribeImageResult> DescribeImageAsync(byte[] image, string language = "en", int maxCandidates = 1)
        {
            var url = $"https://{endpoint}/{DescribePath}?language={language}&maxCandidates={maxCandidates}";

            var request = new HttpRequestMessage(HttpMethod.Post, $"https://{this.endpoint}/{DescribePath}?language={language}&maxCandidates={maxCandidates}");
            request.Headers.Add(SubscriptionKeyHeaderName, this.accessKey);
            request.Content = new StreamContent(new MemoryStream(image));
            //request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var response = await this.httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<DescribeImageResult>(await response.Content.ReadAsStringAsync());
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
