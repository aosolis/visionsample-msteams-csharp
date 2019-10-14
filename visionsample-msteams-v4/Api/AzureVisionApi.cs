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
    using VisionSample.Api.Models;

    /// <summary>
    /// Interface to the Azure Computer Vision API
    /// </summary>
    public interface IVisionApi
    {
        /// <summary>
        /// Returns a description for the given image.
        /// </summary>
        /// <param name="imageUrl">URL to the image</param>
        /// <param name="language">Language to use</param>
        /// <param name="maxCandidates">Maximum number of captions to return</param>
        /// <returns>A description for the image</returns>
        Task<DescribeImageResult> DescribeImageAsync(string imageUrl, string language = "en", int maxCandidates = 1);

        /// <summary>
        /// Returns a description for the given image.
        /// </summary>
        /// <param name="imageBuffer">Image contents</param>
        /// <param name="language">Language to use</param>
        /// <param name="maxCandidates">Maximum number of captions to return</param>
        /// <returns>A description for the image</returns>
        Task<DescribeImageResult> DescribeImageAsync(byte[] imageBuffer, string language = "en", int maxCandidates = 1);

        /// <summary>
        /// Runs optical character recognition on the given image.
        /// </summary>
        /// <param name="imageUrl">URL to the image</param>
        /// <returns>The OCR result</returns>
        Task<OcrResult> RunOcrAsync(string imageUrl);

        /// <summary>
        /// Runs optical character recognition on the given image.
        /// </summary>
        /// <param name="imageBuffer">Image contents</param>
        /// <returns>The OCR result</returns>
        Task<OcrResult> RunOcrAsync(byte[] imageBuffer);
    }

    /// <summary>
    /// Implementation of the <see cref="IVisionApi"/> interface.
    /// </summary>
    public class AzureVisionApi : IVisionApi
    {
        // Service endpoint paths
        public const string DescribePath = "vision/v2.0/describe";
        public const string OcrPath = "vision/v2.0/ocr";

        // Common headers
        public const string SubscriptionKeyHeaderName = "Ocp-Apim-Subscription-Key";

        // JSON formatter
        private static readonly JsonMediaTypeFormatter JsonFormatter = new JsonMediaTypeFormatter();

        // API endpoint (hostname)
        private string endpoint;
        // API access key (this is specific to the DC of the endpoint)
        private string accessKey;
        // HTTP client to use for operations
        private HttpClient httpClient;

        /// <summary>
        /// Intializes a new instance of <see cref="AzureVisionApi"/>.
        /// </summary>
        /// <param name="endpoint">The API endpoint to use</param>
        /// <param name="accessKey">The access key to use</param>
        /// <param name="httpClient">The HTTP client to use</param>
        public AzureVisionApi(string endpoint, string accessKey, HttpClient httpClient = null)
        {
            this.endpoint = endpoint;
            this.accessKey = accessKey;
            this.httpClient = httpClient ?? new HttpClient();
        }

        /// <summary>
        /// Returns a description for the given image.
        /// </summary>
        /// <param name="imageUrl">URL to the image</param>
        /// <param name="language">Language to use</param>
        /// <param name="maxCandidates">Maximum number of captions to return</param>
        /// <returns>A description for the image</returns>
        public async Task<DescribeImageResult> DescribeImageAsync(string imageUrl, string language = "en", int maxCandidates = 1)
        {
            var url = $"https://{endpoint}/{DescribePath}?language={language}&maxCandidates={maxCandidates}";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add(SubscriptionKeyHeaderName, this.accessKey);

            var body = new DescribeImageRequest { Url = imageUrl };
            request.Content = new ObjectContent<DescribeImageRequest>(body, JsonFormatter);

            var response = await this.httpClient.SendAsync(request);
            var result = await this.ProcessApiResponse<DescribeImageResult>(response);
            return result;
        }

        /// <summary>
        /// Returns a description for the given image.
        /// </summary>
        /// <param name="imageBuffer">Image contents</param>
        /// <param name="language">Language to use</param>
        /// <param name="maxCandidates">Maximum number of captions to return</param>
        /// <returns>A description for the image</returns>
        public async Task<DescribeImageResult> DescribeImageAsync(byte[] imageBuffer, string language = "en", int maxCandidates = 1)
        {
            var url = $"https://{endpoint}/{DescribePath}?language={language}&maxCandidates={maxCandidates}";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add(SubscriptionKeyHeaderName, this.accessKey);

            request.Content = new StreamContent(new MemoryStream(imageBuffer));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var response = await this.httpClient.SendAsync(request);
            var result = await this.ProcessApiResponse<DescribeImageResult>(response);
            return result;
        }

        /// <summary>
        /// Runs optical character recognition on the given image.
        /// </summary>
        /// <param name="imageUrl">URL to the image</param>
        /// <returns>The OCR result</returns>
        public async Task<OcrResult> RunOcrAsync(string imageUrl)
        {
            var url = $"https://{endpoint}/{OcrPath}?detectOrientation=true";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add(SubscriptionKeyHeaderName, this.accessKey);

            var body = new OcrRequest { Url = imageUrl };
            request.Content = new ObjectContent<OcrRequest>(body, JsonFormatter);

            var response = await this.httpClient.SendAsync(request);
            var result = await this.ProcessApiResponse<OcrResult>(response);
            return result;
        }

        /// <summary>
        /// Runs optical character recognition on the given image.
        /// </summary>
        /// <param name="imageBuffer">Image contents</param>
        /// <returns>The OCR result</returns>
        public async Task<OcrResult> RunOcrAsync(byte[] imageBuffer)
        {
            var url = $"https://{endpoint}/{OcrPath}?detectOrientation=true";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add(SubscriptionKeyHeaderName, this.accessKey);

            request.Content = new StreamContent(new MemoryStream(imageBuffer));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var response = await this.httpClient.SendAsync(request);
            var result = await this.ProcessApiResponse<OcrResult>(response);
            return result;
        }

        /// <summary>
        /// Process the response from the computer vision API. 
        /// If successful, the body is deserialized to the given type. Else, an exception of type <see cref="AzureVisionApiException"/> is thrown.
        /// </summary>
        /// <typeparam name="T">The expected type of the result</typeparam>
        /// <param name="response">The HTTP response from the API</param>
        /// <returns>The result of the API call</returns>
        private async Task<T> ProcessApiResponse<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
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
