namespace rowDetector
{
    /// <summary>
    /// PDF içindeki bir tablo section’ının dikey sınırlarını temsil eder
    /// Örn:
    /// TEVKİFAT UYGULANMAYAN İŞLEMLER
    /// KISMİ TEVKİFAT UYGULANAN İŞLEMLER
    /// </summary>
    public class SectionBounds
    {
        /// <summary>
        /// Section başlığının hemen ALTINDAN başlayan Y sınırı
        /// (daha küçük Y = sayfanın daha aşağısı)
        /// </summary>
        public double TopY { get; set; }

        /// <summary>
        /// Section’ın bittiği alt sınır
        /// (bir sonraki section ya da sayfa sonu)
        /// </summary>
        public double BottomY { get; set; }

        /// <summary>
        /// Bir satır bu section içinde mi?
        /// </summary>
        public bool Contains(List<PdfWordModel> line)
        {
            var y = line.Average(w => w.Y);

            return y < TopY && y > BottomY;
        }
    }

    public static class SectionDetector
    {
        public static SectionBounds DetectSectionBounds(
            List<List<PdfWordModel>> lines,
            string sectionText)
        {
            // normalize arama
            string Normalize(string s) => string.Concat(
                s.ToUpperInvariant()
                 .Where(c => !char.IsWhiteSpace(c)));

            var target = Normalize(sectionText);

            double? foundY = null;

            foreach (var line in lines)
            {
                var text = PdfLayoutHelper.LineText(line);
                if (Normalize(text).Contains(target))
                {
                    foundY = line.Average(w => w.Y);
                    break;
                }
            }

            if (foundY == null)
                throw new Exception("Section header bulunamadı: " + sectionText);

            // Aşağıdaki satırları section içinde tut
            double topY = foundY.Value;

            // section alt sınırı = bir sonraki büyük düşüş
            double bottomY = lines
                .Where(l => l.Average(w => w.Y) < topY)
                .Select(l => l.Average(w => w.Y))
                .DefaultIfEmpty(0)
                .Min();

            return new SectionBounds
            {
                TopY = topY,
                BottomY = bottomY
            };
        }
    }
}
