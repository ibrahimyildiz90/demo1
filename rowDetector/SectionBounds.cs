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
}
