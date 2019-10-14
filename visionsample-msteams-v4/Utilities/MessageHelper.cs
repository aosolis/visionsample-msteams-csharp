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

namespace VisonSample.Utilities
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Bot.Schema;

    /// <summary>
    /// Collection of utility functions for messages.
    /// </summary>
    public static class MessageHelper
    {
        /// <summary>
        /// Returns the inline image attachments in the list of attachments.
        /// </summary>
        /// <param name="attachments">The list of attachments</param>
        /// <returns>The attachments in the given list that are of type inline image</returns>
        public static IEnumerable<Attachment> GetInlineImageAttachments(IEnumerable<Attachment> attachments)
        {
            return attachments?.Where(a => a.ContentType.StartsWith("image/"));
        }

        /// <summary>
        /// Gets the content of the inline image attachment.
        /// </summary>
        /// <param name="contentUrl">The URL to the image.</param>
        /// <param name="appCredentials">The credentials to use when downloading the image.</param>
        /// <param name="httpClient">The client to use when downloading the image.</param>
        /// <returns>The image contents</returns>
        public static async Task<byte[]> GetInlineAttachmentContentAsync(string contentUrl, MicrosoftAppCredentials appCredentials, HttpClient httpClient = null)
        {
            httpClient = httpClient ?? new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, contentUrl);

            var token = await appCredentials.GetTokenAsync();
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsByteArrayAsync();
            return data;
        }

        /// <summary>
        /// Looks for an HTTP or HTTPS URL in the given text.
        /// </summary>
        /// <param name="text">The text to scan</param>
        /// <returns>The first HTTP/HTTPS url in the given text, or null if there are none.</returns>
        public static string FindHttpUrl(string text)
        {
            var urlRegex = new Regex(@"https?://\S*", RegexOptions.IgnoreCase);
            var match = urlRegex.Match(text);
            return match.Success ? match.Value : null;
        }
    }
}