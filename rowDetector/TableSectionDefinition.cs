namespace rowDetector
{
    public class TableSectionDefinition
    {
        public string SectionTitle { get; set; } = default!;

        // Section başlığından sonra kaç satır aşağısı tablo başlar
        public int MaxDistanceToHeader { get; set; } = 10;
    }
}
