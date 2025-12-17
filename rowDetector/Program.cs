namespace rowDetector
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var pdfPath = @"D:\notlar\kdv iadesi rapor\2 4 BÖLÜ 10 TEVKİFAT HAZİRAN.pdf";

            var words = PdfPigReader.ReadWords(pdfPath);

            // Sayfa bazlı değil, Y’e göre satırlara ayır
            var lines = PdfLayoutHelper.GroupWordsIntoLines(words);

            // --------------------------------------------------
            // 2️⃣ KOLON TANIMLARI (SENİN VERDİĞİN)
            // --------------------------------------------------

            var columnDefinitions = new List<ColumnDefinition>
            {
                new() { HeaderText = "İşlem Türü", ValueType = ColumnValueType.String },
                new() { HeaderText = "Matrah",     ValueType = ColumnValueType.Decimal },
                new() { HeaderText = "KDV Oranı",  ValueType = ColumnValueType.Percentage },
                new() { HeaderText = "Tevkifat",   ValueType = ColumnValueType.String },
                new() { HeaderText = "Vergi",      ValueType = ColumnValueType.Decimal }
            };

            // --------------------------------------------------
            // 3️⃣ HEADER + KOLONLARI TESPİT ET
            // --------------------------------------------------

            HeaderDetectionResult headerResult;

            try
            {
                headerResult = TableHeaderDetector.DetectHeader(
                    lines,
                    columnDefinitions
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Header bulunamadı:");
                Console.WriteLine(ex.Message);
                return;
            }

            // Debug – kolonları yazdır
            Console.WriteLine("---- COLUMNS ----");
            foreach (var col in headerResult.Columns)
            {
                Console.WriteLine(
                    $"{col.HeaderText} | XStart={col.XStart:F1} XEnd={col.XEnd:F1}");
            }

            // --------------------------------------------------
            // 4️⃣ EN İYİ DATA ROW’U BUL
            // --------------------------------------------------

         

            var sectionBounds = TableSectionDetector.DetectSectionBounds(lines, "KISMİ TEVKİFAT UYGULANAN İŞLEMLER");

            var bestRow = DataRowDetector.DetectBestDataRow(
             lines,
             headerResult,
             columnDefinitions,
             sectionBounds);


            if (bestRow == null)
            {
                Console.WriteLine("❌ Uygun data row bulunamadı.");
                return;
            }

            var islemTuruDef = columnDefinitions
                .First(d => d.HeaderText == "İşlem Türü");

            var islemTuru = StringColumnExtractor.Extract(
                bestRow,
                lines,
                headerResult,
                islemTuruDef);

            Console.WriteLine("İşlem Türü:");
            Console.WriteLine(islemTuru);

            // --------------------------------------------------
            // 5️⃣ SONUÇ
            // --------------------------------------------------

            Console.WriteLine();
            Console.WriteLine("---- SELECTED DATA ROW ----");

            foreach (var def in columnDefinitions)
            {
                bestRow.ValuesByColumn.TryGetValue(def.HeaderText, out var value);

                Console.WriteLine($"{def.HeaderText}: {value}");
            }

            // --------------------------------------------------
            // 6️⃣ DEBUG – NUMERIC WORDS (isteğe bağlı)
            // --------------------------------------------------

            Console.WriteLine();
            Console.WriteLine("---- NUMERIC WORDS ----");

            foreach (var w in words
                .Where(w => ValueTypeChecker.IsValid(w.Text, ColumnValueType.Decimal)
                         || ValueTypeChecker.IsValid(w.Text, ColumnValueType.Percentage)))
            {
                Console.WriteLine(
                    $"{w.Text} | X={w.X:F1} Y={w.Y:F1}");
            }

            Console.ReadLine();


        }
    }
}
