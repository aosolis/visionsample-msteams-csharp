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

namespace VisonSample
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Autofac;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    /// <summary>
    /// API controller for bot messaging endpoint
    /// </summary>
    public class MessagesController : ApiController
    {
        private ILifetimeScope scope;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagesController"/> class.
        /// </summary>
        /// <param name="scope">(Injected) Component lifetime scope</param>
        public MessagesController(ILifetimeScope scope)
        {
            this.scope = scope;
        }

        /// <summary>
        /// Messaging endpoint for OCR bot
        /// </summary>
        [HttpPost]
        [Route("api/messages")]
        [Route("ocr/messages")]
        [BotAuthentication(MicrosoftAppIdSettingName = "OcrMicrosoftAppId", MicrosoftAppPasswordSettingName = "OcrMicrosoftAppPassword")]
        public async Task<HttpResponseMessage> PostToOcrBot([FromBody]Activity activity)
        {
            if ((activity.GetActivityType() == ActivityTypes.Message) ||
                (activity.GetActivityType() == ActivityTypes.Invoke))
            {
                await Conversation.SendAsync(activity, () => this.scope.Resolve<Dialogs.OcrDialog>());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        /// <summary>
        /// Messaging endpoint for Caption bot
        /// </summary>
        [HttpPost]
        [Route("caption/messages")]
        [BotAuthentication(MicrosoftAppIdSettingName = "CaptionMicrosoftAppId", MicrosoftAppPasswordSettingName = "CaptionMicrosoftAppPassword")]
        public async Task<HttpResponseMessage> PostToCaptionBot([FromBody]Activity activity)
        {
            if ((activity.GetActivityType() == ActivityTypes.Message) ||
                (activity.GetActivityType() == ActivityTypes.Invoke))
            {
                await Conversation.SendAsync(activity, () => this.scope.Resolve<Dialogs.CaptionDialog>());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            string messageType = message.GetActivityType();
            if (messageType == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (messageType == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (messageType == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (messageType == ActivityTypes.Typing)
            {
                // Handle knowing that the user is typing
            }
            else if (messageType == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}