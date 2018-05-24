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

namespace VisonSample.Dialogs
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    public static class MessageHelper
    {
        public static string GetFirstInlineImageAttachmentUrl(IEnumerable<Attachment> attachments)
        {
            var imageAttachment = attachments?.Where(a => a.ContentType.StartsWith("image/"))?.FirstOrDefault();
            return imageAttachment?.ContentUrl;
        }

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

        public static string FindUrl(string text)
        {
            var urlRegex = new Regex(@"https?://\S*", RegexOptions.IgnoreCase);
            var match = urlRegex.Match(text);
            return match.Success ? match.Value : null;
        }
    }
}