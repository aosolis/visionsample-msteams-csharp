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
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Connector.Teams.Models;
    using Newtonsoft.Json.Linq;
    using VisionSample.Api;
    using VisonSample.Api.Models;

    /// <summary>
    /// Root dialog for Caption bot.
    /// </summary>
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

            // Ignore invoke activities
            if (message.GetActivityType() == ActivityTypes.Invoke)
            {
                return;
            }

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
                    var imageContent = await MessageHelper.GetInlineAttachmentContentAsync(inlineImageUrl, this.appCredentialsProvider.GetCredentials(message.Recipient.Id), this.httpClient);
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