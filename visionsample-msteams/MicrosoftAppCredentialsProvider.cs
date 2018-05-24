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
    using System;
    using System.Configuration;
    using Microsoft.Bot.Connector;

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