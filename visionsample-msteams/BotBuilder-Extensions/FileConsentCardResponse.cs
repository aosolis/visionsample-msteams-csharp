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

namespace Microsoft.Bot.Connector.Teams.Models
{
    using System.Linq;

    /// <summary>
    /// File consent card response invoke activity payload.
    /// </summary>
    public partial class FileConsentCardResponse
    {
        /// <summary>
        /// Value of the <see cref="IInvokeActivity.Name"/> property of the invoke activity.
        /// </summary>
        public const string InvokeName = "fileConsent/invoke";

        /// <summary>
        /// Value of the <see cref="Action"/> property when the user accepts the file consent request.
        /// </summary>
        public const string AcceptAction = "accept";

        /// <summary>
        /// Value of the <see cref="Action"/> property when the user declines the file consent request.
        /// </summary>
        public const string DeclineAction = "decline";
        
        /// <summary>
        /// Initializes a new instance of the FileConsentCardResponse class.
        /// </summary>
        public FileConsentCardResponse() { }

        /// <summary>
        /// Initializes a new instance of the FileConsentCardResponse class.
        /// </summary>
        /// <param name="action">User action on the file consent card.
        /// Possible values include: 'accept', 'decline'</param>
        /// <param name="context">Context sent with the file consent
        /// card.</param>
        /// <param name="uploadInfo">Context sent back to the Bot if user
        /// declined. This is free flow schema and is sent back in Value
        /// field of Activity.</param>
        public FileConsentCardResponse(string action = default(string), object context = default(object), FileUploadInfo uploadInfo = default(FileUploadInfo))
        {
            Action = action;
            Context = context;
            UploadInfo = uploadInfo;
        }

        /// <summary>
        /// Gets or sets user action on the file consent card. Possible values
        /// include: 'accept', 'decline'
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "action")]
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets context sent with the file consent card.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "context")]
        public object Context { get; set; }

        /// <summary>
        /// Gets or sets context sent back to the Bot if user declined. This
        /// is free flow schema and is sent back in Value field of Activity.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "uploadInfo")]
        public FileUploadInfo UploadInfo { get; set; }

    }
}