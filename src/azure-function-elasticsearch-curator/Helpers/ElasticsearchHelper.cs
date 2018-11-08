﻿namespace ElaticsearchCuratorAzureFunction.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using Nest;

    static class ElasticsearchHelper
    {
        public static IElasticClient CreateElasticClient(Uri endpointUri, int requestTimeout)
        {
            if (endpointUri == null)
            {
                throw new ArgumentNullException(nameof(endpointUri));
            }

            var connectionStettings = new ConnectionSettings(endpointUri)
                .DisableDirectStreaming()
                .EnableDebugMode()
                .RequestTimeout(TimeSpan.FromSeconds(requestTimeout));

            var client = new ElasticClient(connectionStettings);

            return client;
        }

        public static IEnumerable<CatIndicesRecord> GetOutOfDateIndices(IElasticClient client, string prefix, int retentionDays, ILogger log)
        {
            var indices = new List<CatIndicesRecord>();

            var catIndicesResponse = client.CatIndices(x => x.AllIndices());

            if (catIndicesResponse.IsValid)
            {
                foreach (var idx in catIndicesResponse.Records.Where(r => r.Index.StartsWith(prefix)))
                {
                    var idxDate = idx.Index.Substring(prefix.Length);
                    var idxDateTime = DateTime.Parse(idxDate, new CultureInfo("en-US"));

                    if (idxDateTime.AddDays(retentionDays) < DateTime.Today)
                    {
                        indices.Add(idx);
                    }
                }
            }
            else
            {
                log.LogWarning(catIndicesResponse.DebugInformation);
            }

            return indices;
        }
    }
}