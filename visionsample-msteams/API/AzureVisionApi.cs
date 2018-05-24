// Copyright (c) Microsoft Corporation
// All rights reserved.
//
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace VisionSample.Api
{
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
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add(SubscriptionKeyHeaderName, this.accessKey);

            var body = new DescribeImageRequest { Url = imageUrl };
            request.Content = new ObjectContent<DescribeImageRequest>(body, JsonFormatter);

            var response = await this.httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<DescribeImageResult>(await response.Content.ReadAsStringAsync());
            }
            else
            {
                var error = JsonConvert.DeserializeObject<Error>(await response.Content.ReadAsStringAsync());
                throw new AzureVisionApiException(error.Message)
                {
                    ErrorCode = error.Code,
                    RequestId = error.RequestId,
                };
            }
        }

        public async Task<DescribeImageResult> DescribeImageAsync(byte[] imageBuffer, string language = "en", int maxCandidates = 1)
        {
            var url = $"https://{endpoint}/{DescribePath}?language={language}&maxCandidates={maxCandidates}";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add(SubscriptionKeyHeaderName, this.accessKey);
            request.Content = new StreamContent(new MemoryStream(imageBuffer));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var response = await this.httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<DescribeImageResult>(await response.Content.ReadAsStringAsync());
            }
            else
            {
                var error = JsonConvert.DeserializeObject<Error>(await response.Content.ReadAsStringAsync());
                throw new AzureVisionApiException(error.Message)
                {
                    ErrorCode = error.Code,
                    RequestId = error.RequestId,
                };
            }
        }

        public async Task<OcrResult> RunOcrAsync(string imageUrl)
        {
            var url = $"https://{endpoint}/{OcrPath}?detectOrientation=true";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add(SubscriptionKeyHeaderName, this.accessKey);

            var body = new OcrRequest { Url = imageUrl };
            request.Content = new ObjectContent<OcrRequest>(body, JsonFormatter);

            var response = await this.httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<OcrResult>(await response.Content.ReadAsStringAsync());
            }
            else
            {
                var error = JsonConvert.DeserializeObject<Error>(await response.Content.ReadAsStringAsync());
                throw new AzureVisionApiException(error.Message)
                {
                    ErrorCode = error.Code,
                    RequestId = error.RequestId,
                };
            }
        }

        public async Task<OcrResult> RunOcrAsync(byte[] imageBuffer)
        {
            var url = $"https://{endpoint}/{OcrPath}?detectOrientation=true";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add(SubscriptionKeyHeaderName, this.accessKey);
            request.Content = new StreamContent(new MemoryStream(imageBuffer));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var response = await this.httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<OcrResult>(await response.Content.ReadAsStringAsync());
            }
            else
            {
                var error = JsonConvert.DeserializeObject<Error>(await response.Content.ReadAsStringAsync());
                throw new AzureVisionApiException(error.Message)
                {
                    ErrorCode = error.Code,
                    RequestId = error.RequestId,
                };
            }
        }
    }
}
