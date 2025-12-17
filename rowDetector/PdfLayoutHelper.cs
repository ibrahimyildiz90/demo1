using System;
using System.Collections.Generic;
using System.Text;

namespace rowDetector
{
    // PDF’te satır kavramı yoktur. Yakın Y değerlerini aynı satır kabul ederiz.
    /* 
     * Y eksenine göre satırları grupladık
     * Aynı satırdaki kelimeleri X’e göre sıraladık
     * LineText() sadece debug ve rule matching için 
     */
    public static class PdfLayoutHelper
    {
        public static List<List<PdfWordModel>> GroupWordsIntoLines(
            List<PdfWordModel> words,
            double yTolerance = 2.5)
        {
            return words
                .GroupBy(w => Math.Round(w.Y / yTolerance))
                .OrderByDescending(g => g.Key)
                .Select(g => g.OrderBy(w => w.X).ToList())
                .ToList();
        }

        public static string LineText(List<PdfWordModel> line)
        {
            return string.Concat(
                line
                    .OrderBy(w => w.X)
                    .Select(w => w.Text)
            );
        }

        /* 
         * Bir satırın içinde headerText’in
         * tüm parçalarının ardışık olarak
         * bulunup bulunmadığını kontrol eder
         *
         *Header’ı kelimelere böler ("KDV", "Oranı")
Satırdaki kelimeler üzerinde sliding window ile dolaşır
Ardışık eşleşme arar
        */
        public static bool LineContainsHeaderSequence(
            List<PdfWordModel> line,
            string headerText)
        {
            var headerParts = headerText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i <= line.Count - headerParts.Length; i++)
            {
                bool match = true;

                for (int j = 0; j < headerParts.Length; j++)
                {
                    if (!line[i + j].Text
                        .Equals(headerParts[j], StringComparison.OrdinalIgnoreCase))
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                    return true;
            }

            return false;
        }

    }
}
