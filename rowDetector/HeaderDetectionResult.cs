using System;
using System.Collections.Generic;
using System.Text;

namespace rowDetector
{
    public class HeaderDetectionResult
    {
        /// <summary>
        /// Header satırından türetilmiş tablo kolonları.
        /// Her kolon yalnızca PDF üzerindeki X koordinat aralığını bilir.
        /// </summary>
        public List<TableColumn> Columns { get; set; } = new();

        /// <summary>
        /// Header satırının alt sınırı (Y koordinatı).
        /// Bu değerin ALTINDA kalan satırlar data row adayı kabul edilir.
        /// </summary>
        public double HeaderBottomY { get; set; }

        /// <summary>
        /// Header’ın bulunduğu satırın Y koordinatı (opsiyonel ama debug için faydalı).
        /// </summary>
        public double HeaderY { get; set; }

        public List<PdfWordModel> HeaderLine { get; set; } = new();

        /// <summary>
        /// Header tespitinin güven skoru (opsiyonel, ileride false-positive elemek için).
        /// Şimdilik 0–1 arası normalize bir değer olarak bırakıldı.
        /// </summary>
        public double Confidence { get; set; }
    }
}
