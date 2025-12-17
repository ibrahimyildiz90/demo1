using System;
using System.Collections.Generic;
using System.Text;

namespace rowDetector
{
    // PDFPig’ten gelen kelimeleri normalize ederiz.
    public class PdfWordModel
    {
        public string Text { get; set; } = string.Empty;
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
    }
}
