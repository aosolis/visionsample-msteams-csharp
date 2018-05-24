using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Configuration;
using System;

namespace VisonSample
{
    public interface IMicrosoftAppCredentialsProvider
    {
        MicrosoftAppCredentials GetCredentials(string botId);
    }

    public class MicrosoftAppCredentialsProvider : IMicrosoftAppCredentialsProvider
    {
        private string captionBotId;
        private string ocrBotId;

        public MicrosoftAppCredentialsProvider()
        {
            this.captionBotId = ConfigurationManager.AppSettings["CaptionBotId"] ?? Environment.GetEnvironmentVariable("CaptionBotId", EnvironmentVariableTarget.Process);
            this.ocrBotId = ConfigurationManager.AppSettings["OcrBotId"] ?? Environment.GetEnvironmentVariable("OcrBotId", EnvironmentVariableTarget.Process);
        }

        public MicrosoftAppCredentials GetCredentials(string botId)
        {
            if (botId == this.captionBotId)
            {
                var appId = ConfigurationManager.AppSettings["CaptionMicrosoftAppId"] ?? Environment.GetEnvironmentVariable("CaptionMicrosoftAppId");
                var appPassword = ConfigurationManager.AppSettings["CaptionMicrosoftAppPassword"] ?? Environment.GetEnvironmentVariable("CaptionMicrosoftAppPassword");
                return new MicrosoftAppCredentials(appId, appPassword);
            }
            else if (botId == this.ocrBotId)
            {
                var appId = ConfigurationManager.AppSettings["OcrMicrosoftAppId"] ?? Environment.GetEnvironmentVariable("OcrMicrosoftAppId");
                var appPassword = ConfigurationManager.AppSettings["OcrMicrosoftAppPassword"] ?? Environment.GetEnvironmentVariable("OcrMicrosoftAppPassword");
                return new MicrosoftAppCredentials(appId, appPassword);
            }

            throw new ArgumentException(nameof(botId));
        }
    }
}