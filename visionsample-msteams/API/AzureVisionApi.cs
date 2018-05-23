namespace VisionSample.API
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using VisonSample.API.Models;

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
            this.httpClient.BaseAddress = new Uri($"https://${endpoint}/");
        }

        public async Task<DescribeImageResult> DescribeImageAsync(string imageUrl, string language = "en", int maxCandidates = 1)
        {
            var qsp = string.Format("language={0}&maxCandidates={1}", language, maxCandidates);
            var url = $"${DescribePath}?${qsp}";

            var result = await this.httpClient.PostAsJsonAsync(url, new DescribeImageRequest { Url = imageUrl });
            if (result.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<DescribeImageResult>(await result.Content.ReadAsStringAsync());
            }
            else
            {
                throw new Exception();
            }
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
