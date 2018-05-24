namespace VisonSample.Api.Models
{
    using System.Collections.Generic;

    public class OcrRequest
    {
        public string Url { get; set; }
    }

    public class OcrResult
    {
        public string Language { get; set; }

        public double TextAngle { get; set; }

        public string Orientation { get; set; }

        public IList<OcrTextRegion> Regions { get; set; }
    }

    public class OcrTextRegion
    {
        public string BoundingBox { get; set; }

        public IList<OcrTextLine> Lines { get; set; }
    }

    public class OcrTextLine
    {
        public string BoundingBox { get; set; }

        public IList<OcrWord> Words { get; set; }
    }

    public class OcrWord
    {
        public string BoundingBox { get; set; }

        public string Text { get; set; }
    }
}