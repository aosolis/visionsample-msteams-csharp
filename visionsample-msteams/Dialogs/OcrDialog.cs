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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Autofac;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Connector.Teams.Models;
    using Newtonsoft.Json.Linq;
    using VisionSample.Api;
    using VisonSample.Api.Models;
    using VisonSample.Utilities;

    /// <summary>
    /// Root dialog for OCR bot
    /// </summary>
    [Serializable]
    public class OcrDialog: IDialog<object>
    {
        private const string OcrResultKey = "OcrResult";

        private IVisionApi visionApi;
        private IMicrosoftAppCredentialsProvider appCredentialsProvider;
        private HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="OcrDialog"/> class.
        /// </summary>
        /// <param name="visionApi">(Injected) Vision API</param>
        /// <param name="appCredentialsProvider">(Injected) App credentials provider</param>
        /// <param name="httpClient">(Injected) HTTP client</param>
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

        /// <summary>
        /// Handle an incoming message or invoke activity.
        /// </summary>
        /// <param name="context">Dialog context</param>
        /// <param name="result"></param>
        /// <returns>Tracking task</returns>
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result as Activity;

            // Handle invoke activities
            if (message.GetActivityType() == ActivityTypes.Invoke)
            {
                // We're only interested in invokes from file consent card responses
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
            var inlineImageAttachment = MessageHelper.GetInlineImageAttachments(message.Attachments).FirstOrDefault();
            if (inlineImageAttachment != null)
            {
                // Image was sent as inline content
                // contentUrl is a url to the file content; the bot's bearer token is required
                await this.SendOcrResultAsync(context, Task.Run(async () =>
                {
                    var appCredentials = this.appCredentialsProvider.GetCredentials(message.Recipient.Id);
                    var imageContent = await MessageHelper.GetInlineAttachmentContentAsync(inlineImageAttachment.ContentUrl, appCredentials, this.httpClient);
                    return await this.visionApi.RunOcrAsync(imageContent);
                }));
                return;
            }

            // 3) URL to an image sent in the text of the message
            var url = MessageHelper.FindHttpUrl(message.Text);
            if (url != null)
            {
                await this.SendOcrResultAsync(context, Task.Run(async () =>
                {
                    return await this.visionApi.RunOcrAsync(url);
                }));
                return;
            }

            // No image found -- send instruction text
            await context.PostAsync("Hi! Send me a picture or a link to one, and I'll tell you what it is.");
        }

        /// <summary>
        /// Send the OCR result to the user.
        /// </summary>
        /// <param name="context">Dialog context</param>
        /// <param name="operation">OCR task that yields an OCR result</param>
        /// <param name="filename">Suggested filename for the result</param>
        /// <returns>Tracking task</returns>
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
                        SizeInBytes = Encoding.UTF8.GetBytes(text).Length,
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

                    // Send the response to the user, asking for permission to send the OCR result as a file
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

        /// <summary>
        /// Handle the user response to the file consent card.
        /// </summary>
        /// <param name="context">Dialog context</param>
        /// <param name="message">The invoke activity triggered by the user action</param>
        /// <returns>Tracking task</returns>
        private async Task HandleFileConsentResponseAsync(IDialogContext context, Activity message)
        {
            // Create a connector client for deleting the file consent card
            IConnectorClientFactory connectorClientFactory = new ConnectorClientFactory(Address.FromActivity(message), this.appCredentialsProvider.GetCredentials(message.Recipient.Id));
            var connectorClient = connectorClientFactory.MakeConnectorClient();

            // Convert the value in the invoke to the expected type
            var fileConsentCardResponse = ((JObject)message.Value).ToObject<FileConsentCardResponse>();
            switch (fileConsentCardResponse.Action)
            {
                // User declined the file
                case FileConsentCardResponse.DeclineAction:
                    // Delete the file consent card
                    if (message.ReplyToId != null)
                    {
                        await connectorClient.Conversations.DeleteActivityAsync(message.Conversation.Id, message.ReplyToId);
                    }

                    await context.PostAsync("Ok! If you change your mind, just send me the picture again.");
                    break;

                // User accepted the file
                case FileConsentCardResponse.AcceptAction:
                    // Send typing activity while the file is uploading
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

                        // Delete the file consent card
                        if (message.ReplyToId != null)
                        {
                            await connectorClient.Conversations.DeleteActivityAsync(message.Conversation.Id, message.ReplyToId);
                        }

                        // Send the user a link to the uploaded file
                        // The fields in the file info card are populated from the upload info we got in the incoming invoke
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

        /// <summary>
        /// Sets the contents of the file to the provided text.
        /// </summary>
        /// <param name="uploadUrl">The URL to the OneDrive upload session</param>
        /// <param name="text">The text that will be the contents of the file</param>
        /// <returns>Tracking task</returns>
        private async Task UploadFileAsync(string uploadUrl, string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            var content = new StreamContent(new MemoryStream(bytes));
            content.Headers.ContentRange = new ContentRangeHeaderValue(0, bytes.LongLength - 1, bytes.LongLength);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var response = await this.httpClient.PutAsync(uploadUrl, content);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Gets the text recognized by the OCR operation.
        /// </summary>
        /// <param name="result">The OCR result</param>
        /// <returns>The text that will be returned to the user as a file</returns>
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

    /// <summary>
    /// Data to track for an OCR result
    /// </summary>
    class OcrResultData
    {
        /// <summary>
        /// Unique identifier for the result
        /// </summary>
        public string ResultId { get; set; }

        /// <summary>
        /// Text identified in the image
        /// </summary>
        public string Text { get; set; }
    }

    /// <summary>
    /// Context data sent with the file consent card
    /// </summary>
    class FileConsentContext
    {
        /// <summary>
        /// The unique id of the result referenced by the file consent card
        /// </summary>
        public string ResultId { get; set; }
    }


}