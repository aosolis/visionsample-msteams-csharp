using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using VisionSample.Api;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Autofac;

namespace VisonSample
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private ILifetimeScope scope;
        private IVisionApi visionApi;

        public MessagesController(ILifetimeScope scope, IVisionApi visionApi)
        {
            this.scope = scope;
            this.visionApi = visionApi;
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            using (var scope = DialogModule.BeginLifetimeScope(this.scope, activity))
            {
                if (activity.GetActivityType() == ActivityTypes.Message)
                {
                    await this.HandleMessageAsync(activity, scope.Resolve<IConnectorClient>());
                }
                else if (activity.GetActivityType() == ActivityTypes.Invoke)
                {
                    await this.HandleInvokeAsync(activity, scope.Resolve<IConnectorClient>());
                }
            }
            
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private async Task HandleMessageAsync(Activity message, IConnectorClient connectorClient)
        {
            // Send typing activity
            var typingActivity = message.CreateReply();
            typingActivity.Type = ActivityTypes.Typing;
            await connectorClient.Conversations.ReplyToActivityAsync(typingActivity);

            // Send instruction text
            var instructionText = message.CreateReply();
            instructionText.Text = "Hi! Send me a picture or a link to one, and I'll tell you what it is.";
            await connectorClient.Conversations.ReplyToActivityAsync(instructionText);
        }

        private async Task HandleInvokeAsync(Activity invoke, IConnectorClient connectorClient)
        {
            // Send typing activity
            var typingActivity = invoke.CreateReply();
            typingActivity.Type = ActivityTypes.Typing;
            await connectorClient.Conversations.SendToConversationAsync(typingActivity);
        }
    }
}