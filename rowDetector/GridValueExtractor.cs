using System;
using System.Collections.Generic;
using System.Text;

namespace rowDetector
{
    /* Kolon Degeri */
    public static class GridValueExtractor
    {
        public static string? Extract(
        List<PdfWordModel> dataLine,
        TableColumn column)
        {
            // 1️⃣ Kolon bandındaki numeric token'ları al
            var numericCandidates = dataLine
                .Where(w =>
                    w.X >= column.XStart &&
                    w.X <= column.XEnd &&
                    w.Text.Any(char.IsDigit))
                .ToList();

            if (!numericCandidates.Any())
                return null;

            // 2️⃣ Kolonun "ideal X noktası"
            double targetX = (column.XStart + column.XEnd) / 2;

            // 3️⃣ En yakın numeric token'ı seç
            var best = numericCandidates
                .OrderBy(w => Math.Abs(w.X - targetX))
                .First();

            return best.Text;
        }
    }
}
