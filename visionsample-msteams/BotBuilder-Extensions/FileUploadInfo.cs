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
    /// Upload information for the file.
    /// </summary>
    public partial class FileUploadInfo
    {
        /// <summary>
        /// Initializes a new instance of the FileUploadInfo class.
        /// </summary>
        public FileUploadInfo() { }

        /// <summary>
        /// Initializes a new instance of the FileUploadInfo class.
        /// </summary>
        /// <param name="name">File name.</param>
        /// <param name="uploadUrl">URL to an upload session for the file
        /// contents.</param>
        /// <param name="contentUrl">URL to the file.</param>
        /// <param name="uniqueId">Identifier that uniquely identifies the
        /// file.</param>
        /// <param name="fileType">File type.</param>
        public FileUploadInfo(string name = default(string), string uploadUrl = default(string), string contentUrl = default(string), string uniqueId = default(string), string fileType = default(string))
        {
            Name = name;
            UploadUrl = uploadUrl;
            ContentUrl = contentUrl;
            UniqueId = uniqueId;
            FileType = fileType;
        }

        /// <summary>
        /// Gets or sets file name.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets URL to an upload session for the file contents.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "uploadUrl")]
        public string UploadUrl { get; set; }

        /// <summary>
        /// Gets or sets URL to the file.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "contentUrl")]
        public string ContentUrl { get; set; }

        /// <summary>
        /// Gets or sets identifier that uniquely identifies the file.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "uniqueId")]
        public string UniqueId { get; set; }

        /// <summary>
        /// Gets or sets file type.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "fileType")]
        public string FileType { get; set; }

    }
}