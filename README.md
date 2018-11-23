# azure-function-elasticsearch-curator

Elasticsearch Curator for Azure Function

## Environment

- Written in NET Core 2.1
- Using [NEST](https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/introduction.html) (Elasticsearch high-level client)

## Configuration

### Scheduling

The function is scheduled (each 5 seconds for testing - adapt to your needs).

```cs
public static class Function
{
    [FunctionName("ElaticsearchCuratorAzureFunction")]
    public static void Run([TimerTrigger("%CronExpression%")]TimerInfo timerInfo, ExecutionContext context, ILogger log)
    ...
}
```

### local.settings.json

```cs
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "CronExpression": "*/5 * * * * *"
  },
  "Curator": {
    "WithDryRun": true,  // true => will not perform index DELETE
    "RequestTimeout": 4, // in seconds
    "Entries": [
      {
        "Name": "Docker",
        "Endpoint": "http://127.0.0.1:9200",
        "IndexEntries": [
          {
            "Prefix": "sample-",
            "RetentionDays": 14,
            "DeleteCloseIndicesOnly": true // true => won't delete 'open' indices
          }
        ]
      }
    ]
  }
}
```

Set `WithDryRun` value to `false` in order to delete indices on the Elasticsearch (be careful !)

## Test the function

You need

- Visual Studio 2017 with Azure Tools
- Elasticsearch server

You can pop an Elasticsearch server using my [Docker Compose Elasticsearch stack](https://github.com/dhabierre/docker-integration-tests). Think to add the date to the index name in the `create-index.ps1` script before running it (ex, `sample-2018-10-01`).

## Limitations

- The pattern is hardcoded (%Y.%m.%d)