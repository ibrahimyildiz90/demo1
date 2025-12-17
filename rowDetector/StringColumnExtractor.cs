using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rowDetector
{
    public static class StringColumnExtractor
    {
        /*
         * GÖREVİ:
         * - Seçilmiş data row’un Y referansını alır
         * - String kolonun X aralığında
         * - Yukarı + aşağı satırlardan metni toplar
         * - Çok satırlı "İşlem Türü" gibi alanları üretir
         */
        public static string? Extract(
            RowCandidate selectedRow,
            List<List<PdfWordModel>> allLines,
            HeaderDetectionResult headerResult,
            ColumnDefinition columnDefinition,
            int maxLineDistance = 5)
        {
            // Sadece STRING kolonlar
            if (columnDefinition.ValueType != ColumnValueType.String)
                return null;

            var column = headerResult.Columns.FirstOrDefault(c =>
                c.HeaderText.Equals(columnDefinition.HeaderText,
                    StringComparison.OrdinalIgnoreCase));

            if (column == null)
                return null;

            // Data row’un merkez Y noktası
            double baseY = selectedRow.Line.Average(w => w.Y);

            var collectedWords = new List<PdfWordModel>();

            foreach (var line in allLines)
            {
                double lineY = line.Average(w => w.Y);

                // Data row etrafında belirli mesafede mi? Row sayısı 12
                if (Math.Abs(lineY - baseY) > maxLineDistance * 12)
                    continue;

                // Kolon X aralığında mı?
                var wordsInColumn = line.Where(w =>
                    w.X >= column.XStart &&
                    w.X <= column.XEnd)
                    .ToList();

                if (!wordsInColumn.Any())
                    continue;

                // Header satırını alma
                if (line.Max(w => w.Y) >= headerResult.HeaderBottomY)
                    continue;

                collectedWords.AddRange(wordsInColumn);
            }

            if (!collectedWords.Any())
                return null;

            // Okuma sırasına sok
            var ordered = collectedWords
                .OrderByDescending(w => w.Y)
                .ThenBy(w => w.X)
                .ToList();

            return PdfLayoutHelper.LineText(ordered);
        }
    }
}
