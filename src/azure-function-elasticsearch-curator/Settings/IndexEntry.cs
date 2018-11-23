namespace ElaticsearchCuratorAzureFunction.Settings
{
    class IndexEntry
    {
        public string Prefix { get; set; }

        public int RetentionDays { get; set; }

        public bool DeleteCloseIndicesOnly { get; set; }
    }
}