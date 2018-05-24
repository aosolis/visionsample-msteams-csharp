namespace VisonSample
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Autofac;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    public class MessagesController : ApiController
    {
        private ILifetimeScope scope;

        public MessagesController(ILifetimeScope scope)
        {
            this.scope = scope;
        }

        /// <summary>
        /// POST: api/messages
        /// (OCR Bot) Receive a message from a user and reply to it
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
        /// POST: api/messages
        /// (Caption Bot) Receive a message from a user and reply to it
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