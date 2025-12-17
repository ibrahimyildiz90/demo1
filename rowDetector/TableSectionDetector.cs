using System;
using System.Collections.Generic;
using System.Linq;

namespace rowDetector
{
    public static class TableSectionDetector
    {
        public static SectionBounds DetectSectionBounds(
    List<List<PdfWordModel>> lines,
    string sectionText)
        {
            var headerY = FindSectionHeaderY(lines, sectionText);

            if (headerY == null)
                throw new Exception($"Section bulunamadı: {sectionText}");

            var top = headerY.Value - 2;

            // Bir sonraki SECTION veya HEADER gelene kadar
            var bottom = lines
                .Where(l => l.Average(w => w.Y) < top)
                .Select(l => l.Average(w => w.Y))
                .DefaultIfEmpty(0)
                .Min();

            return new SectionBounds
            {
                TopY = top,
                BottomY = bottom
            };
        }

        //public static (double TopY, double BottomY)? DetectSectionBounds(
        //    List<List<PdfWordModel>> lines,
        //    TableSectionDefinition section)
        //{
        //    for (int i = 0; i < lines.Count; i++)
        //    {
        //        var lineText = PdfLayoutHelper.LineText(lines[i]);

        //        if (!lineText.Contains(section.SectionTitle,
        //                StringComparison.OrdinalIgnoreCase))
        //            continue;

        //        double titleY = lines[i].Average(w => w.Y);

        //        // Section alt sınırı = bir sonraki büyük başlık veya sayfa sonu
        //        double bottomY = lines
        //            .Skip(i + 1)
        //            .Select(l => l.Average(w => w.Y))
        //            .FirstOrDefault(y => y < titleY - 100);

        //        if (bottomY == 0)
        //            bottomY = double.MinValue;

        //        return (titleY, bottomY);
        //    }

        //    return null;
        //}

        public static double? FindSectionHeaderY(
    List<List<PdfWordModel>> lines,
    string sectionText)
        {
            var normalizedSection = Normalize(sectionText);

            foreach (var line in lines)
            {
                // Satırı TEK STRING haline getir
                var lineText = string.Join(" ",
                    line.OrderBy(w => w.X).Select(w => w.Text));

                var normalizedLine = Normalize(lineText);

                if (normalizedLine.Contains(normalizedSection))
                {
                    // Section başlığının Y değeri
                    return line.Average(w => w.Y);
                }
            }

            return null;
        }

        private static string Normalize(string text)
        {
            return text
                .ToUpperInvariant()
                .Replace("İ", "I")
                .Replace("Ş", "S")
                .Replace("Ğ", "G")
                .Replace("Ü", "U")
                .Replace("Ö", "O")
                .Replace("Ç", "C")
                .Replace("  ", " ")
                .Trim();
        }

    }
}
