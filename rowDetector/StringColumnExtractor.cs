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

            // 3️⃣ Header Y (üst sınır)
            double headerY = headerResult.HeaderLine.Average(w => w.Y);

            var collectedWords = new List<PdfWordModel>();
            bool startedCollecting = false;

            // 4️⃣ Section içindeki satırları DATA ROW'DAN YUKARI doğru tara
            foreach (var line in allLines
                .Where(l => sectionBounds.Contains(l))
                .OrderByDescending(l => l.Average(w => w.Y)))
            {
                double lineY = line.Average(w => w.Y);

                // Header üstüne çıkma (form açıklamaları)
                if (lineY > headerY)
                    continue;

                // Data row’un ALTINI alma
                if (lineY >= dataRowY)
                    continue;

                // Yeni bir data row başladıysa ve biz zaten topladıysak → DUR
                if (startedCollecting && IsDataLikeLine(line, headerResult))
                    break;

                // 🎯 Aynı kolon X aralığındaki STRING kelimeler
                var wordsInColumn = line
                    .Where(w =>
                        w.X >= column.XStart &&
                        w.X <= column.XEnd &&
                        !ValueTypeChecker.IsValid(w.Text, ColumnValueType.Decimal) &&
                        !ValueTypeChecker.IsValid(w.Text, ColumnValueType.Percentage))
                    .ToList();

                if (!wordsInColumn.Any())
                {
                    // Eğer toplamaya başladıysak ve bu satır boşsa → blok bitti
                    if (startedCollecting)
                        break;

                    continue;
                }

                // İlk anlamlı string satırı bulduk
                startedCollecting = true;
                collectedWords.AddRange(wordsInColumn);
            }

            if (!collectedWords.Any())
                return null;

            // 5️⃣ Okunabilir sıraya koy
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
