using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace VisonSample.Dialogs
{
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