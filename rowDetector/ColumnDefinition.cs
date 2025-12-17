using System;
using System.Collections.Generic;
using System.Text;

namespace rowDetector
{
    public class ColumnDefinition
    {
        public string HeaderText { get; set; } = default!;
        public ColumnValueType ValueType { get; set; }
        public bool Required { get; set; } = true;
    }
}
