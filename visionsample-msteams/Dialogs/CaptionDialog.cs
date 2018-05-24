using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using VisionSample.Api;
using System.Linq;
using Newtonsoft.Json.Linq;
using Microsoft.Bot.Connector.Teams.Models;
using VisonSample.Api.Models;
using System.Net.Http;

namespace VisonSample.Dialogs
{
    [Serializable]
    public class CaptionDialog : IDialog<object>
    {
        private IVisionApi visionApi;

        private IMicrosoftAppCredentialsProvider appCredentialsProvider;

        private HttpClient httpClient;

        public CaptionDialog(IVisionApi visionApi, IMicrosoftAppCredentialsProvider appCredentialsProvider, HttpClient httpClient)
        {
            this.visionApi = visionApi;
            this.appCredentialsProvider = appCredentialsProvider;
            this.httpClient = httpClient;
        }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result as Activity;

            // Send typing activity
            var typingActivity = message.CreateReply();
            typingActivity.Type = ActivityTypes.Typing;
            await context.PostAsync(typingActivity);

            // Caption Bot can take an image file in 3 ways:

            // 1) File attachment -- a file picked from OneDrive or uploaded from the computer
            var fileAttachment = message.Attachments?.Where(a => a.ContentType == FileDownloadInfo.ContentType)?.FirstOrDefault();
            if (fileAttachment != null)
            {
                // Image was sent as a file attachment
                // downloadUrl is an unauthenticated URL to the file contents, valid for only a few minutes
                var fileDownloadInfo = ((JObject)fileAttachment.Content).ToObject<FileDownloadInfo>();
                await this.SendImageCaptionAsync(context, Task.Run(async () =>
                {
                    return await this.visionApi.DescribeImageAsync(fileDownloadInfo.DownloadUrl);
                }));
                return;
            }

            // 2) Inline image attachment -- an image pasted into the compose box, or selected from the photo library on mobile
            var inlineImageUrl = MessageHelper.GetFirstInlineImageAttachmentUrl(message.Attachments);
            if (inlineImageUrl != null)
            {
                // Image was sent as inline content
                // contentUrl is a url to the file content; the bot's bearer token is required
                await this.SendImageCaptionAsync(context, Task.Run(async () =>
                {
                    var imageContent = await MessageHelper.GetInlineAttachmentContentAsync(inlineImageUrl, this.appCredentialsProvider.GetCredentials(), this.httpClient);
                    return await this.visionApi.DescribeImageAsync(imageContent);
                }));
                return;
            }

            // 3) URL to an image sent in the text of the message
            var url = MessageHelper.FindUrl(message.Text);
            if (url != null)
            {
                await this.SendImageCaptionAsync(context, Task.Run(async () =>
                {
                    return await this.visionApi.DescribeImageAsync(url);
                }));
                return;
            }

            // Send instruction text
            await context.PostAsync("Hi! Send me a picture or a link to one, and I'll tell you what it is.");
        }

        private async Task SendImageCaptionAsync(IDialogContext context, Task<DescribeImageResult> operation)
        {
            try
            {
                var result = await operation;
                var caption = result.Description.Captions?.FirstOrDefault();
                if (caption != null)
                {
                    await context.PostAsync(string.Format("I think that's {0}.", caption.Text));
                }
                else
                {
                    await context.PostAsync("¯\\_(ツ)_/¯");
                }
            }
            catch (Exception ex)
            {
                await context.PostAsync(string.Format("There was a problem analyzing the image: {0}", ex.Message));
            }
        }
    }
}