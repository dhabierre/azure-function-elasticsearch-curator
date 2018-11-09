namespace ElaticsearchCuratorAzureFunction
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ElaticsearchCuratorAzureFunction.Helpers;
    using ElaticsearchCuratorAzureFunction.Settings;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Nest;

    public static class Function
    {
        [FunctionName("ElaticsearchCuratorAzureFunction")]
        public static void Run([TimerTrigger("%CronExpression%")]TimerInfo timerInfo, ExecutionContext context, ILogger log)
        {
            var configuration = GetConfiguration(context);
            var section = configuration.GetSection("Curator");
            var settings = section.Get<CuratorSettings>();

            Parallel.ForEach(
                settings.Entries,
                new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * 2 },
                entry =>
            {
                log.LogInformation($"Name = {entry.Name}, Endpoint = {entry.Endpoint}");

                var client = ElasticsearchHelper.CreateElasticClient(new Uri(entry.Endpoint), settings.RequestTimeout);

                foreach (var index in entry.Indices)
                {
                    IEnumerable<CatIndicesRecord> indices = null;

                    try
                    {
                        indices = ElasticsearchHelper.GetOutOfDateIndices(client, index.Prefix, index.RetentionDays, log);
                    }
                    catch (Exception e)
                    {
                        log.LogError(e, $"!!> Error while fetching out-of-date indices (requestTimeout?): {e.Message}");

                        continue;
                    }

                    foreach (var idx in indices)
                    {
                        if (!settings.WithDryRun)
                        {
                            log.LogInformation($"--> Deleting index '{idx.Index}' (DocsCount = {idx.DocsCount}, PrimaryStoreSize = {idx.PrimaryStoreSize}, StoreSize = {idx.StoreSize})");

                            try
                            {
                                client.DeleteIndex(idx.Index);
                            }
                            catch (Exception e)
                            {
                                log.LogError(e, $"!!> Error while deleting index '{idx.Index}': {e.Message}");
                            }
                        }
                        else
                        {
                            log.LogInformation($"--> [DRY-RUN] Deleting index '{idx.Index}' (DocsCount = {idx.DocsCount}, PrimaryStoreSize = {idx.PrimaryStoreSize}, StoreSize = {idx.StoreSize})");
                        }
                    }
                }
            });
        }

        private static IConfiguration GetConfiguration(ExecutionContext context)
        {
            return new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        private static CuratorSettings GetSettings(IConfiguration configuration)
        {
            var section = configuration.GetSection("Curator");
            var settings = section.Get<CuratorSettings>();

            return settings;
        }
    }
}