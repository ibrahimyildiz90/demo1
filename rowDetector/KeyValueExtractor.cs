using System;
using System.Collections.Generic;
using System.Text;

namespace rowDetector
{
    // SATIR BAZLI ANAHTAR-DEĞER ÇIKARICI
    public static class KeyValueExtractor
    {
        public static string? ExtractAfterKey(
            List<List<PdfWordModel>> lines,
            string key)
        {
            for (int i = 0; i < lines.Count - 1; i++)
            {
                var text = PdfLayoutHelper.LineText(lines[i]);

                if (!text.Contains(key))
                    continue;

                return PdfLayoutHelper.LineText(lines[i + 1]);
            }

            return null;
        }
    }
}
