using System;
using System.Collections.Generic;
using System.Text;

namespace rowDetector
{
    /* Bir kolonun fiziksel PDF karşılığıdır..Bu sınıf değer tipi bilmez
    Sadece “PDF üzerinde bu kolon nerede” sorusuna cevap verir */
    public class TableColumn
    {
        public string HeaderText { get; set; } = default!;
        public double XStart { get; set; }
        public double XEnd { get; set; }
    }

}
