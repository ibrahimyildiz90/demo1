using System;
using System.Collections.Generic;
using System.Text;

namespace rowDetector
{
    public class TableHeaderDetector
    {
        /*GÖREVİ:
         * Matrah | KDV Oranı | Vergi” satırını bulur
         * Kolonların X aralıklarını çıkarır*/
        public static HeaderDetectionResult DetectHeader(
       List<List<PdfWordModel>> lines,
       List<ColumnDefinition> columnDefinitions)
        {
            // 1️⃣ Header satırını bul
            var headerLine = FindHeaderLine(lines, columnDefinitions);

            if (headerLine == null)
                throw new Exception("Header satırı bulunamadı.");

            // 2️⃣ Header alt sınırı
            double headerBottomY = headerLine.Min(w => w.Y);

            // 3️⃣ Kolonların X aralıklarını tespit et
            var columns = DetectColumns(headerLine, columnDefinitions);

            return new HeaderDetectionResult
            {
                Columns = columns,
                HeaderBottomY = headerBottomY
            };
        }


        /* 
         * Header bulunduysa:
         * ilk kelimenin X’i → kolon başlangıcı
         * son kelimenin X + Width’i → kolon bitişi         
         */
        private static List<PdfWordModel> FindHeaderWords(
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
                {
                    return line.Skip(i).Take(headerParts.Length).ToList();
                }
            }

            return new List<PdfWordModel>();
        }

        /* 
         * Her bir kolon için header kelimelerini bul
         * X aralıklarını belirle
         * LineText ile birleştirilmiş metin
Contains bazlı eşleşme
         */
        private static List<PdfWordModel>? FindHeaderLine(
            List<List<PdfWordModel>> lines,
            List<ColumnDefinition> columnDefinitions)
        {
            foreach (var line in lines)
            {
                var lineText = PdfLayoutHelper.LineText(line);

                int matchCount = columnDefinitions.Count(cd =>
                    lineText.Contains(cd.HeaderText, StringComparison.OrdinalIgnoreCase));

                // En az 2 header eşleşiyorsa bu satır header’dır
                if (matchCount >= 2)
                    return line;
            }

            return null;
        }

        /*
         * Kolonların X Aralıkları Nasıl Çıkıyor
         * Header kelimesinin X merkezi
         * Yanındaki header’lara göre sınırlar
         */
        private static List<TableColumn> DetectColumns(
    List<PdfWordModel> headerLine,
    List<ColumnDefinition> columnDefinitions)
        {
            var headerCenters = new List<(string HeaderText, double CenterX)>();

            foreach (var def in columnDefinitions)
            {
                var headerWords = FindHeaderWords(headerLine, def.HeaderText);

                if (!headerWords.Any())
                    continue;

                double startX = headerWords.Min(w => w.X);
                double endX = headerWords.Max(w => w.X + w.Width);
                double centerX = (startX + endX) / 2;

                headerCenters.Add((def.HeaderText, centerX));
            }

            // X’e göre sırala
            headerCenters = headerCenters.OrderBy(h => h.CenterX).ToList();

            var columns = new List<TableColumn>();

            for (int i = 0; i < headerCenters.Count; i++)
            {
                double xStart = i == 0
                    ? headerCenters[i].CenterX - 80
                    : (headerCenters[i - 1].CenterX + headerCenters[i].CenterX) / 2;

                double xEnd = i == headerCenters.Count - 1
                    ? headerCenters[i].CenterX + 200
                    : (headerCenters[i].CenterX + headerCenters[i + 1].CenterX) / 2;

                columns.Add(new TableColumn
                {
                    HeaderText = headerCenters[i].HeaderText,
                    XStart = xStart,
                    XEnd = xEnd
                });
            }

            return columns;
        }



    }
}
