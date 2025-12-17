using System;
using System.Collections.Generic;
using System.Text;

namespace rowDetector
{
    public class RowCandidate
    {
        public List<PdfWordModel> Line { get; set; } = new();
        public Dictionary<string, string> ValuesByColumn { get; set; } = new();
        public double Confidence { get; set; }
    }

}
