using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams.Models;
using Newtonsoft.Json.Linq;
using VisionSample.Api;
using VisonSample.Api.Models;

namespace VisonSample.Dialogs
{
    class OcrResultData
    {
        public string ResultId { get; set; }

        public string Text { get; set; }
    }

    class FileConsentContext
    {
        public string ResultId { get; set; }
    }

    [Serializable]
    public class OcrDialog: IDialog<object>
    {
        private const string OcrResultKey = "OcrResult";

        private IVisionApi visionApi;
        private IMicrosoftAppCredentialsProvider appCredentialsProvider;
        private HttpClient httpClient;

        public OcrDialog(IVisionApi visionApi, IMicrosoftAppCredentialsProvider appCredentialsProvider, HttpClient httpClient)
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
                if (message.Name == FileConsentCardResponse.InvokeName)
                {
                    await this.HandleFileConsentResponseAsync(context, message);
                }
                else
                {
                    Trace.WriteLine($"Received unknown invoke activity name: {message.Name}");
                }
                return;
            }

            // Send typing activity
            var typingActivity = context.MakeMessage();
            typingActivity.Type = ActivityTypes.Typing;
            await context.PostAsync(typingActivity);

            // OCR Bot can take an image file in 3 ways:

            // 1) File attachment -- a file picked from OneDrive or uploaded from the computer
            var fileAttachment = message.Attachments?.Where(a => a.ContentType == FileDownloadInfo.ContentType)?.FirstOrDefault();
            if (fileAttachment != null)
            {
                // Image was sent as a file attachment
                // downloadUrl is an unauthenticated URL to the file contents, valid for only a few minutes
                var fileDownloadInfo = ((JObject)fileAttachment.Content).ToObject<FileDownloadInfo>();
                var resultFilename = fileAttachment.Name + ".txt";
                await this.SendOcrResultAsync(context, Task.Run(async () =>
                {
                    return await this.visionApi.RunOcrAsync(fileDownloadInfo.DownloadUrl);
                }), resultFilename);
                return;
            }

            // 2) Inline image attachment -- an image pasted into the compose box, or selected from the photo library on mobile
            var inlineImageUrl = MessageHelper.GetFirstInlineImageAttachmentUrl(message.Attachments);
            if (inlineImageUrl != null)
            {
                // Image was sent as inline content
                // contentUrl is a url to the file content; the bot's bearer token is required
                await this.SendOcrResultAsync(context, Task.Run(async () =>
                {
                    var imageContent = await MessageHelper.GetInlineAttachmentContentAsync(inlineImageUrl, this.appCredentialsProvider.GetCredentials(), this.httpClient);
                    return await this.visionApi.RunOcrAsync(imageContent);
                }));
                return;
            }

            // 3) URL to an image sent in the text of the message
            var url = MessageHelper.FindUrl(message.Text);
            if (url != null)
            {
                await this.SendOcrResultAsync(context, Task.Run(async () =>
                {
                    return await this.visionApi.RunOcrAsync(url);
                }));
                return;
            }

            // Send instruction text
            await context.PostAsync("Hi! Send me a picture or a link to one, and I'll tell you what it is.");
        }

        private async Task HandleFileConsentResponseAsync(IDialogContext context, Activity message)
        {
            var fileConsentCardResponse = ((JObject)message.Value).ToObject<FileConsentCardResponse>();
            switch (fileConsentCardResponse.Action)
            {
                case FileConsentCardResponse.DeclineAction:
                    // TODO: Delete file consent card
                    await context.PostAsync("Ok! If you change your mind, just send me the picture again.");
                    break;

                case FileConsentCardResponse.AcceptAction:
                    // Send typing activity
                    var typingActivity = context.MakeMessage();
                    typingActivity.Type = ActivityTypes.Typing;
                    await context.PostAsync(typingActivity);

                    // Check that response is for the current OCR result
                    var responseContext = ((JObject)fileConsentCardResponse.Context).ToObject<FileConsentContext>();
                    var lastOcrResult = context.ConversationData.GetValueOrDefault<OcrResultData>(OcrResultKey);
                    if (lastOcrResult?.ResultId != responseContext.ResultId)
                    {
                        await context.PostAsync("That result has expired. Send me the picture again, and I'll rescan it.");
                        return;
                    }

                    try
                    {
                        // Upload the file contents to the upload session we got from the invoke value
                        // See https://docs.microsoft.com/en-us/onedrive/developer/rest-api/api/driveitem_createuploadsession#upload-bytes-to-the-upload-session
                        await this.UploadFileAsync(fileConsentCardResponse.UploadInfo.UploadUrl, lastOcrResult.Text);

                        // Send message with link to the file.
                        // The fields in the file info card are populated from the upload info we got in the incoming invoke.
                        var fileInfoCard = FileInfoCard.FromFileUploadInfo(fileConsentCardResponse.UploadInfo);
                        var reply = context.MakeMessage();
                        reply.Attachments = new List<Attachment> { fileInfoCard.ToAttachment() };
                        await context.PostAsync(reply);
                    }
                    catch (Exception ex)
                    {
                        await context.PostAsync(string.Format("There was an error uploading the file: {0}", ex.Message));
                    }
                    break;
            }
        }

        private async Task UploadFileAsync(string uploadUrl, string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            var content = new StreamContent(new MemoryStream(bytes));
            content.Headers.ContentRange = new ContentRangeHeaderValue(bytes.LongLength);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            await this.httpClient.PutAsync(uploadUrl, content);
        }

        private async Task SendOcrResultAsync(IDialogContext context, Task<OcrResult> operation, string filename = null)
        {
            try
            {
                var result = await operation;
                var text = this.GetRecognizedText(result);
                if (text.Length > 0)
                {
                    // Save the OCR result in conversation data
                    var resultData = new OcrResultData
                    {
                        ResultId = Guid.NewGuid().ToString(),
                        Text = text,
                    };
                    context.ConversationData.SetValue(OcrResultKey, resultData);

                    // Create file consent card
                    var consentContext = new FileConsentContext
                    {
                        ResultId = resultData.ResultId,
                    };
                    var consentCard = new FileConsentCard
                    {
                        Name = filename ?? "result.txt",
                        Description = "Text recognized from the image",
                        SizeInBytes = text.Length,
                        AcceptContext = consentContext,
                        DeclineContext = consentContext,
                    };

                    // Get a human-readable name for the language
                    string languageName = result.Language;
                    try
                    {
                        var cultureInfo = CultureInfo.GetCultureInfo(result.Language);
                        languageName = cultureInfo.EnglishName;
                    }
                    catch (CultureNotFoundException)
                    {
                        // Ignore unknown language codes
                    }

                    var reply = context.MakeMessage();
                    reply.Text = string.Format("I found {0} text in that image.", languageName);
                    reply.Attachments = new List<Attachment> { consentCard.ToAttachment() };
                    await context.PostAsync(reply);
                }
                else
                {
                    await context.PostAsync("I didn't find any text in that picture.");
                }
            }
            catch (Exception ex)
            {
                await context.PostAsync(string.Format("There was a problem analyzing the image: {0}", ex.Message));
            }
        }

        private string GetRecognizedText(OcrResult result)
        {
            var regions = result.Regions?.Select(region =>
            {
                var lines = region.Lines?.Select(line =>
                {
                    var words = line.Words?.Select(word => word.Text);
                    return (words != null) ? string.Join(" ", words) : string.Empty;
                });
                return (lines != null) ? string.Join("\r\n", lines) : string.Empty;
            });
            return (regions != null) ? string.Join("\r\n\r\n", regions) : string.Empty;
        }
    }
}