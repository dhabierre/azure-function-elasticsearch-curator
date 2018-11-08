namespace ElaticsearchCuratorAzureFunction.Settings
{
    using System.Collections.Generic;

    class CuratorEntry
    {
        public string Name { get; set; }

        public string Endpoint { get; set; }

        public IEnumerable<IndexEntry> Indices { get; set; }
    }
}