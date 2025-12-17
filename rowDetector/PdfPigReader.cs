using System;
using System.Collections.Generic;
using System.Text;
using UglyToad.PdfPig;

namespace rowDetector
{
    /* 
     * PDFPig kullanarak PDF içindeki tüm kelimeleri
     * Text + X + Y + Width bilgisiyle okumak.
    */
    public static class PdfPigReader
    {
        public static List<PdfWordModel> ReadWords(string pdfPath)
        {
            var result = new List<PdfWordModel>();

            using var document = PdfDocument.Open(pdfPath);

            foreach (var page in document.GetPages())
            {
                foreach (var word in page.GetWords())
                {
                    result.Add(new PdfWordModel
                    {
                        Text = word.Text,
                        X = word.BoundingBox.Left,
                        Y = word.BoundingBox.Bottom,
                        Width = word.BoundingBox.Width
                    });
                }
            }

            return result;
        }
    }
}
