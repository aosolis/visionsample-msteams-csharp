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

namespace VisonSample.Api.Models
{
    using System.Collections.Generic;

    public class DescribeImageRequest
    {
        public string Url { get; set; }
    }

    public class DescribeImageResult
    {
        public ImageDescription Description { get; set; }

        public string RequestId { get; set; }

        public ImageMetadata Metadata { get; set; }
    }

    public class ImageMetadata
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public string Format { get; set; }
    }

    public class ImageDescription
    {
        public IList<string> Tags { get; set; }

        public IList<ImageCaption> Captions{ get; set; }
    }

    public class ImageCaption
    {
        public string Text { get; set; }

        public double Confidence { get; set; }
    }
}