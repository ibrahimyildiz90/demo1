namespace rowDetector
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var pdfPath = @"D:\notlar\kdv iadesi rapor\2 4 BÖLÜ 10 TEVKİFAT HAZİRAN.pdf";

            var words = PdfPigReader.ReadWords(pdfPath);

            var lines = PdfLayoutHelper.GroupWordsIntoLines(words);

            var columnDefinitions = new List<ColumnDefinition>
                    {
                        new() { HeaderText = "Matrah",    ValueType = ColumnValueType.Decimal },
                        new() { HeaderText = "KDV Oranı", ValueType = ColumnValueType.Percentage },
                        new() { HeaderText = "Vergi",     ValueType = ColumnValueType.Decimal }
                    };

            var headerResult = TableHeaderDetector.DetectHeader(lines, columnDefinitions);

            var rowCandidates = DataRowDetector.DetectDataRows(
                                 lines,
                                 headerResult,
                                 columnDefinitions);

            var bestRow = rowCandidates
                .OrderByDescending(r => r.Confidence)
                .FirstOrDefault();

            if (bestRow != null)
            {
                Console.WriteLine("---- SELECTED DATA ROW ----");

                for (int i = 0; i < columnDefinitions.Count; i++)
                {
                    var value = bestRow.ValuesByColumn.ContainsKey(i)
                        ? bestRow.ValuesByColumn[i]
                        : "";

                    Console.WriteLine($"{columnDefinitions[i].HeaderText}: {value}");
                }
            }

            /////
            Console.WriteLine("---- COLUMNS ----");
            foreach (var col in headerResult.Columns)
            {
                Console.WriteLine($"{col.HeaderText} | XStart={col.XStart:F1} XEnd={col.XEnd:F1}");
            }
            Console.WriteLine();

            Console.WriteLine("---- NUMERIC WORDS ----");
            foreach (var line in lines)
            {
                foreach (var w in line.Where(w => w.Text.Any(char.IsDigit)))
                {
                    Console.WriteLine($"{w.Text} | X={w.X:F1} Y={w.Y:F1}");
                }
            }

            Console.ReadLine();


        }
    }
}
