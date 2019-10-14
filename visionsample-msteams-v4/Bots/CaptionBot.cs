// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json.Linq;
using VisionSample.Api;
using VisionSample.Api.Models;
using VisonSample.Utilities;

namespace VisionSample.Bots
{
    public class CaptionBot : TeamsActivityHandler
    {
        private readonly IVisionApi visionApi;
        private readonly IMicrosoftAppCredentialsProvider appCredentialsProvider;
        private readonly IHttpClientFactory _clientFactory;

        public CaptionBot(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var message = turnContext.Activity;

            // Ignore invoke activities
            if (message.Type == ActivityTypes.Invoke)
            {
                return;
            }

            // Send typing activity
            var typingActivity = new Activity(ActivityTypes.Typing);
            await turnContext.SendActivityAsync(typingActivity);

            // Caption Bot can take an image file in 3 ways:

            // 1) File attachment -- a file picked from OneDrive or uploaded from the computer
            var fileAttachment = message.Attachments?.Where(a => a.ContentType == FileDownloadInfo.ContentType)?.FirstOrDefault();
            if (fileAttachment != null)
            {
                // Image was sent as a file attachment
                // downloadUrl is an unauthenticated URL to the file contents, valid for only a few minutes
                var fileDownloadInfo = ((JObject)fileAttachment.Content).ToObject<FileDownloadInfo>();
                await this.SendImageCaptionAsync(turnContext, Task.Run(async () =>
                {
                    return await this.visionApi.DescribeImageAsync(fileDownloadInfo.DownloadUrl);
                }));
                return;
            }

            // 2) Inline image attachment -- an image pasted into the compose box, or selected from the photo library on mobile
            var inlineImageAttachment = MessageHelper.GetInlineImageAttachments(message.Attachments)?.FirstOrDefault();
            if (inlineImageAttachment != null)
            {
                // Image was sent as inline content
                // contentUrl is a url to the file content; the bot's bearer token is required
                await this.SendImageCaptionAsync(turnContext, Task.Run(async () =>
                {
                    var appCredentials = this.appCredentialsProvider.GetCredentials(message.Recipient.Id);
                    var imageContent = await MessageHelper.GetInlineAttachmentContentAsync(inlineImageAttachment.ContentUrl, appCredentials, this.httpClient);
                    return await this.visionApi.DescribeImageAsync(imageContent);
                }));
                return;
            }

            // 3) URL to an image sent in the text of the message
            var url = MessageHelper.FindHttpUrl(message.Text);
            if (url != null)
            {
                await this.SendImageCaptionAsync(turnContext, Task.Run(async () =>
                {
                    return await this.visionApi.DescribeImageAsync(url);
                }));
                return;
            }

            // Send instruction text
            if (message.Conversation.ConversationType == "personal")
            {
                await turnContext.SendActivityAsync("Hi! Send me a picture or a link to one, and I'll tell you what it is.");
            }
            else
            {
                await turnContext.SendActivityAsync("Hi! Send me a picture or a link to one, and I'll tell you what it is. In channels and group chats, please paste the picture directly into the compose box: Teams won't let me receive file attachments yet!");
            }
        }

        private async Task SendImageCaptionAsync(ITurnContext context, Task<DescribeImageResult> operation)
        {
            try
            {
                var result = await operation;
                var caption = result.Description.Captions?.FirstOrDefault();
                if (caption != null)
                {
                    await context.SendActivityAsync(string.Format("I think that's {0}.", caption.Text));
                }
                else
                {
                    await context.SendActivityAsync("¯\\_(ツ)_/¯");
                }
            }
            catch (Exception ex)
            {
                await context.SendActivityAsync(string.Format("There was a problem analyzing the image: {0}", ex.Message));
            }
        }
    }
}
