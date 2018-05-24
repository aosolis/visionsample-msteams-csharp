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
    /// File download info attachment.
    /// </summary>
    public partial class FileDownloadInfo
    {
        /// <summary>
        /// Content type to be used in the type property.
        /// </summary>
        public const string ContentType = "application/vnd.microsoft.teams.file.download.info";
        
        /// <summary>
        /// Initializes a new instance of the FileDownloadInfo class.
        /// </summary>
        public FileDownloadInfo() { }

        /// <summary>
        /// Initializes a new instance of the FileDownloadInfo class.
        /// </summary>
        /// <param name="downloadUrl">File download url.</param>
        /// <param name="uniqueId">Unique Id for the file.</param>
        /// <param name="fileType">Type of file.</param>
        /// <param name="etag">ETag for the file.</param>
        public FileDownloadInfo(string downloadUrl = default(string), string uniqueId = default(string), string fileType = default(string), object etag = default(object))
        {
            DownloadUrl = downloadUrl;
            UniqueId = uniqueId;
            FileType = fileType;
        }

        /// <summary>
        /// Gets or sets file download url.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "downloadUrl")]
        public string DownloadUrl { get; set; }

        /// <summary>
        /// Gets or sets unique Id for the file.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "uniqueId")]
        public string UniqueId { get; set; }

        /// <summary>
        /// Gets or sets type of file.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "fileType")]
        public string FileType { get; set; }
    }
}