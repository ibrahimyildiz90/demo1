using System;
using System.Collections.Generic;
using System.Text;

namespace rowDetector
{
    public class TableHeaderResult
    {
        public List<TableColumnDefinition> Columns { get; set; } = new();
        public double HeaderY { get; set; }
    }
}
