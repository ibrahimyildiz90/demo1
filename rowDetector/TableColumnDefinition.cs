using System;
using System.Collections.Generic;
using System.Text;

namespace rowDetector
{
    /*
     * Her kolon:
     * Bir başlığa sahiptir
     * X ekseninde bir aralığı vardır 
     */

    public class TableColumnDefinition
    {
        public string HeaderText { get; set; } = "";
        public double XStart { get; set; }
        public double XEnd { get; set; }

        // 🔴 YENİ
        public double CenterX => (XStart + XEnd) / 2;
    }
}
