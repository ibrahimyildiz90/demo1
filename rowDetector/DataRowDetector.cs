using System;
using System.Collections.Generic;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace rowDetector
{
    public static class DataRowDetector
    {
        /*
         * GÖREVİ:
         * - Header altındaki TÜM satırları tarar
         * - Her satırı kolon tanımlarına göre puanlar
         * - Aynı PDF’te birden fazla tablo olsa bile
         *   EN UYUMLU (confidence’ı en yüksek) satırı seçer
         */
        public static RowCandidate? DetectBestDataRow(
            List<List<PdfWordModel>> lines,
            HeaderDetectionResult headerResult,
            List<ColumnDefinition> columnDefinitions)
        {
            var candidates = new List<RowCandidate>();

            foreach (var line in lines)
            {
                // Header üstünü alma
                if (line.Max(w => w.Y) >= headerResult.HeaderBottomY)
                    continue;

                if (!IsRowStarter(line, headerResult.Columns, columnDefinitions))
                    continue;

                var candidate = EvaluateLine(line, headerResult, columnDefinitions);

                if (candidate != null)
                    candidates.Add(candidate);
            }

            return candidates
                .OrderByDescending(c => c.Confidence)
                .FirstOrDefault();
        }

        // ------------------------------------------------------

        private static RowCandidate? EvaluateLine(
            List<PdfWordModel> line,
            HeaderDetectionResult headerResult,
            List<ColumnDefinition> columnDefinitions)
        {
            int validColumnCount = 0;
            double confidence = 0;

            var values = new Dictionary<string, string>();

            foreach (var column in headerResult.Columns)
            {
                // HeaderText bazlı eşleşme (KRİTİK)
                var def = columnDefinitions.FirstOrDefault(d =>
                    d.HeaderText.Equals(column.HeaderText,
                        StringComparison.OrdinalIgnoreCase));

                if (def == null)
                    continue;

                if (def.ValueType == ColumnValueType.String)
                    continue; // 🔴 KRİTİK Artık: EvaluateLine = sadece numeric kolonlar String kolonlar data row seçimini etkilemez

                var rawValue = GridValueExtractor.Extract(line, column);

                if (string.IsNullOrWhiteSpace(rawValue))
                    continue;

                if (!ValueTypeChecker.IsValid(rawValue, def.ValueType))
                    continue;

                validColumnCount++;
                values[column.HeaderText] = rawValue;

                confidence += GetConfidenceWeight(def.ValueType, rawValue);
            }

            // En az 2 numeric kolon dolu mu
            if (validColumnCount < 2)
                return null;

            return new RowCandidate
            {
                Line = line,
                ValuesByColumn = values,
                Confidence = confidence
            };
        }

        // ------------------------------------------------------

        private static double GetConfidenceWeight(
            ColumnValueType type,
            string value)
        {
            switch (type)
            {
                case ColumnValueType.Decimal:
                    return 3.0;

                case ColumnValueType.Percentage:
                    return value.Contains("%") || value.Length <= 3 ? 2.5 : 1.5;

                case ColumnValueType.String:
                    return value.Length > 3 ? 1.5 : 0.5;

                case ColumnValueType.Date:
                    return 2.0;

                default:
                    return 1.0;
            }
        }

        private static bool IsRowStarter(
            List<PdfWordModel> line,
            List<TableColumn> columns,
            List<ColumnDefinition> definitions)
        {
            int numericCount = 0;

            foreach (var col in columns)
            {
                var def = definitions.First(d => d.HeaderText == col.HeaderText);

                if (def.ValueType == ColumnValueType.Decimal ||
                    def.ValueType == ColumnValueType.Percentage)
                {
                    var value = GridValueExtractor.Extract(line, col);

                    if (!string.IsNullOrWhiteSpace(value))
                        numericCount++;
                }
            }

            return numericCount >= 2; // 🔴 KRİTİK KURAL
        }

    }
}
