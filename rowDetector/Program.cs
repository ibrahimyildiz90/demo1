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
            // 4️⃣ EN İYİ DATA ROW’U BUL
            // --------------------------------------------------

            string sectionText = "KISMİ TEVKİFAT UYGULANAN İŞLEMLER";

            var sectionBounds = SectionDetector.DetectSectionBounds(
                lines,
                sectionText
            );

            // bu satır header’ı sadece o section içinde arar
            var headerResult = TableHeaderDetector.DetectHeader(
                lines,
                columnDefinitions,
                sectionBounds);

            // şimdi sadece o section’a ait satırlar taranır
            var bestRow = DataRowDetector.DetectBestDataRow(
                lines,
                headerResult,
                columnDefinitions,
                sectionBounds);


            var islemTuruDef = columnDefinitions
                .First(d => d.HeaderText == "İşlem Türü");

            var jsonlines = System.Text.Json.JsonSerializer.Serialize(lines, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            var jsonbestRow = System.Text.Json.JsonSerializer.Serialize(bestRow, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            var islemTuru = StringColumnExtractor.Extract(
                bestRow,
                lines,
                headerResult,
                islemTuruDef,
                sectionBounds);

            Console.WriteLine("İşlem Türü:");
            Console.WriteLine(islemTuru);


            // Debug – kolonları yazdır
            Console.WriteLine("---- COLUMNS ----");
            foreach (var col in headerResult.Columns)
            {
                Console.WriteLine(
                    $"{col.HeaderText} | XStart={col.XStart:F1} XEnd={col.XEnd:F1}");
            }


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
