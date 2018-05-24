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