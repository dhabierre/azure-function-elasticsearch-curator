namespace ElaticsearchCuratorAzureFunction.Settings
{
    using System.Collections.Generic;

    class CuratorSettings
    {
        public bool WithDryRun { get; set; }

        public int RequestTimeout { get; set; }

        public IEnumerable<CuratorEntry> Entries { get; set; }
    }
}
