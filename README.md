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
    public static void Run([TimerTrigger("*/5 * * * * *")]TimerInfo timerInfo, ExecutionContext context, ILogger log)
    ...
}
```

### local.settings.json

```cs
"Curator": {
    "RequestTimeout": 10,
    "Entries": [
      {
        "Name": "Docker",
        "Endpoint": "http://127.0.0.1:9200",
        "Indices": [
          {
            "Prefix": "sample-",
            "RetentionDays": 14
          }
        ]
      }
    ]
}
```

## Test the function

You need

- Visual Studio 2017 with Azure Tools
- Elasticsearch server

You can pop an Elasticsearch server using my [Docker Compose Elasticsearch stack](https://github.com/dhabierre/docker-integration-tests). Think to add the date to the index name in the `create-index.ps1` script before running it (ex, `sample-2018-10-01`).

## Limitations

- The pattern is hardcoded (%Y.%m.%d)