namespace ElaticsearchCuratorAzureFunction.Settings
{
    using System.Collections.Generic;

    class CuratorSettings
    {
        public int RequestTimeout { get; set; }

        public IEnumerable<CuratorEntry> Entries { get; set; }
    }
}
