using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace rowDetector
{
    public class DataRowDetector
    {
        /* 
         Bir satır data row ise:
        Header’ın altında
        En az N adet numeric alan içerir
        Kolon X aralıklarıyla örtüşür
        Açıklama satırı değildir 
        */
        public static List<RowCandidate> DetectDataRows(
        List<List<PdfWordModel>> lines,
        HeaderDetectionResult headerResult,
        List<ColumnDefinition> columnDefinitions)
        {
            var candidates = new List<RowCandidate>();

            var possibleLines = lines
                .Where(l => l.First().Y < headerResult.HeaderBottomY)
                .OrderByDescending(l => l.First().Y);

            foreach (var line in possibleLines)
            {
                var candidate = new RowCandidate { Line = line };

                int validValueCount = 0;
                double confidence = 0;

                for (int i = 0; i < headerResult.Columns.Count; i++)
                {
                    var column = headerResult.Columns[i];
                    var def = columnDefinitions[i];

                    var raw = GridValueExtractor.Extract(line, column);

                    if (ValueTypeChecker.IsValid(raw, def.ValueType))
                    {
                        validValueCount++;
                        candidate.ValuesByColumn[i] = raw;

                        // confidence katkısı
                        confidence += GetConfidenceWeight(def.ValueType, raw);
                    }
                }

                // en az 2 kolon dolu olmalı
                if (validValueCount >= 2)
                {
                    candidate.Confidence = confidence;
                    candidates.Add(candidate);
                }
            }

            return candidates;
        }

        private static double GetConfidenceWeight(
            ColumnValueType type,
            string value)
        {
            return type switch
            {
                ColumnValueType.Percentage when value.Length <= 3 => 2.0, // 20 gibi
                ColumnValueType.Decimal when value.Contains(",") => 1.5,
                ColumnValueType.Integer => 1.0,
                _ => 0.5
            };
        }
    }

}
