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

namespace VisonSample.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Connector.Authentication;

    /// <summary>
    /// Interface that returns the app credentials for a given bot.
    /// </summary>
    public interface IMicrosoftAppCredentialsProvider
    {
        /// <summary>
        /// Returns the app credentials for the given bot.
        /// </summary>
        /// <param name="botId">The bot ID (in the 28:xxx format)</param>
        /// <returns>The app credentials for the bot</returns>
        MicrosoftAppCredentials GetCredentials(string botId);
    }

    /// <summary>
    /// Implementation of the <see cref="IMicrosoftAppCredentialsProvider"/> interface.
    /// </summary>
    public class MicrosoftAppCredentialsProvider : IMicrosoftAppCredentialsProvider
    {
        private Dictionary<string, MicrosoftAppCredentials> appCredentials = new Dictionary<string, MicrosoftAppCredentials>();
        private string captionBotId;
        private string ocrBotId;

        /// <summary>
        /// Intializes a new instance of the <see cref="MicrosoftAppCredentialsProvider"/> class.
        /// </summary>
        public MicrosoftAppCredentialsProvider()
        {
            this.captionBotId = GetSetting("CaptionBotId");
            this.ocrBotId = GetSetting("OcrBotId");
        }

        /// <summary>
        /// Returns the app credentials for the given bot.
        /// </summary>
        /// <param name="botId">The bot ID (in the 28:xxx format)</param>
        /// <returns>The app credentials for the bot</returns>
        public MicrosoftAppCredentials GetCredentials(string botId)
        {
            if (!this.appCredentials.TryGetValue(botId, out MicrosoftAppCredentials credentials))
            {
                credentials = this.CreateCredentials(botId);
                this.appCredentials[botId] = credentials;
            }
            return credentials;
        }

        /// <summary>
        /// Creates app credentials object for the given bot.
        /// </summary>
        /// <param name="botId">The bot ID (in the 28:xxx format)</param>
        /// <returns>The app credentials for the bot</returns>
        public MicrosoftAppCredentials CreateCredentials(string botId)
        {
            if (botId == this.captionBotId)
            {
                return new MicrosoftAppCredentials(GetSetting("CaptionMicrosoftAppId"), GetSetting("CaptionMicrosoftAppPassword"));
            }
            else if (botId == this.ocrBotId)
            {
                return new MicrosoftAppCredentials(GetSetting("OcrMicrosoftAppId"), GetSetting("OcrMicrosoftAppPassword"));
            }

            throw new ArgumentException(nameof(botId));
        }
        
        /// <summary>
        /// Gets the value of the given setting, from either app settings or an environment variable.
        /// </summary>
        /// <param name="name">The name of the setting</param>
        /// <returns>The effective value of the setting</returns>
        private static string GetSetting(string name)
        {
            var value = ConfigurationManager.AppSettings[name];
            if (string.IsNullOrWhiteSpace(value))
            {
                value = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
            }
            return value;
        }
    }
}