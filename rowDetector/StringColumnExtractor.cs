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
       RowCandidate dataRow,
       List<List<PdfWordModel>> allLines,
       HeaderDetectionResult headerResult,
       ColumnDefinition stringColumn,
       SectionBounds sectionBounds)
        {
            // 1️⃣ İlgili kolon
            var column = headerResult.Columns
                .First(c => c.HeaderText == stringColumn.HeaderText);

            // 2️⃣ Data row Y
            double dataRowY = dataRow.Line.Average(w => w.Y);

            // 3️⃣ HEADER Y (KRİTİK)
            double headerY = headerResult.HeaderLine.Average(w => w.Y);

            var collectedWords = new List<PdfWordModel>();

            // 4️⃣ Section içindeki satırları yukarıdan aşağı gez
            foreach (var line in allLines
                .Where(l => sectionBounds.Contains(l))
                .OrderByDescending(l => l.Average(w => w.Y)))
            {
                double lineY = line.Average(w => w.Y);

                // ⛔ HEADER ÜSTÜ (FORM AÇIKLAMALARI)
                if (lineY > headerY)
                    continue;

                // ⛔ ALTTA YENİ DATA ROW BAŞLADIYSA DUR
                if (lineY < dataRowY &&
                    IsDataLikeLine(line, headerResult))
                    break;

                // 🎯 HEADER ile DATA ROW ARASI
                if (lineY <= headerY && lineY >= dataRowY - 2)
                {
                    var wordsInColumn = line
                        .Where(w =>
                            w.X >= column.XStart &&
                            w.X <= column.XEnd &&
                            !ValueTypeChecker.IsValid(w.Text, ColumnValueType.Decimal) &&
                            !ValueTypeChecker.IsValid(w.Text, ColumnValueType.Percentage))
                        .ToList();

                    collectedWords.AddRange(wordsInColumn);
                }
            }

            if (!collectedWords.Any())
                return null;

            collectedWords = collectedWords
                .OrderByDescending(w => w.Y)
                .ThenBy(w => w.X)
                .ToList();

            return string.Join(" ", collectedWords.Select(w => w.Text));
        }



        private static bool IsDataLikeLine(
    List<PdfWordModel> line,
    HeaderDetectionResult headerResult)
        {
            int numericCount = 0;

            foreach (var col in headerResult.Columns)
            {
                var value = GridValueExtractor.Extract(line, col);

                if (!string.IsNullOrWhiteSpace(value))
                    numericCount++;
            }

            // En az 2 kolon doluysa bu bir DATA ROW’dur
            return numericCount >= 2;
        }
    }
}
