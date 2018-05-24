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
    /// <summary>
    /// File consent card attachment.
    /// </summary>
    public partial class FileConsentCard
    {
        /// <summary>
        /// Content type to be used in the type property.
        /// </summary>
        public const string ContentType = "application/vnd.microsoft.teams.card.file.consent";

        /// <summary>
        /// Initializes a new instance of the FileConsentCard class.
        /// </summary>
        public FileConsentCard() { }

        /// <summary>
        /// Initializes a new instance of the FileConsentCard class.
        /// </summary>
        /// <param name="description">File description.</param>
        /// <param name="sizeInBytes">Size of the file to be uploaded in
        /// Bytes.</param>
        /// <param name="acceptContext">Context sent back to the Bot if user
        /// consented to upload. This is free flow schema and is sent back in
        /// Value field of Activity.</param>
        /// <param name="declineContext">Context sent back to the Bot if user
        /// declined. This is free flow schema and is sent back in Value
        /// field of Activity.</param>
        public FileConsentCard(string description = default(string), long? sizeInBytes = default(long?), object acceptContext = default(object), object declineContext = default(object))
        {
            Description = description;
            SizeInBytes = sizeInBytes;
            AcceptContext = acceptContext;
            DeclineContext = declineContext;
        }

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets file description.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets size of the file to be uploaded in Bytes.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "sizeInBytes")]
        public long? SizeInBytes { get; set; }

        /// <summary>
        /// Gets or sets context sent back to the Bot if user consented to
        /// upload. This is free flow schema and is sent back in Value field
        /// of Activity.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "acceptContext")]
        public object AcceptContext { get; set; }

        /// <summary>
        /// Gets or sets context sent back to the Bot if user declined. This
        /// is free flow schema and is sent back in Value field of Activity.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "declineContext")]
        public object DeclineContext { get; set; }

        /// <summary>
        /// Creates a new attachment from <see cref="FileConsentCard"/>.
        /// </summary>
        /// <returns> The generated attachment.</returns>
        public Attachment ToAttachment()
        {
            return new Attachment
            {
                Content = this,
                ContentType = FileConsentCard.ContentType,
                Name = this.Name,
            };
        }
    }
}