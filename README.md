# Install:
```
dotnet add package Serilog.Sinks.ClickHouse --version 1.0.2
```

# Default fields:
1. timestamp (DateTime)
2. level (String)
3. message (String)

# Usage:
```C#
var log = new LoggerConfiguration()
  .Enrich.WithProperty("source", "network")
  .Enrich.WithProperty("user", "admin")
  .WriteTo.ClickHouse(
      "Compress=False;CheckCompressedHash=False;Compressor=lz4;Host=localhost;Port=9000;Database=test_db;User=default;Password=",
      "test_db.logs",
      50,
      TimeSpan.FromSeconds(30),
      new ColumnOptions{
         RemoveStandardColumns = new List<string>{"level","message"},
         OrderBy = new List<string> { "timestamp", "source"},
         PartitionBy = new List<string> { "toDate(timestamp)"},
         TimeToLive = new TimeToLiveOptions {
          Table = new TimeToLiveAttribute { 
            DateTimeColumnName = "timestamp",
            IntervalValue = 12,
            IntervalName = "MONTH"
          },
          Fields = new Dictionary<string,TimeToLiveAttribute> {
            { "source", 
              new TimeToLiveAttribute { 
                DateTimeColumnName = "timestamp",
                IntervalValue = 3,
                IntervalName = "WEEK"
              }
            },
            { "callstack", 
              new TimeToLiveAttribute { 
                DateTimeColumnName = "timestamp",
                IntervalValue = 3,
                IntervalName = "WEEK"
              }
            }
          }
         }
      },
      new List<AdditionalColumn>
      {
          new AdditionalColumn { Name = "source", Type = "String" },
          new AdditionalColumn { Name = "callstack", Type = "String" },
          new AdditionalColumn { Name = "user", Type = "String" }
      },
      restrictedToMinimumLevel: LogEventLevel.Information
  ).CreateLogger();
 ``` 
 Using appsettings.json:
 ```json
 {
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug"
    },
    "WriteTo": [
      {
        "Name": "ClickHouse",
        "Args": {
          "connectionString": "Compress=False;CheckCompressedHash=False;Compressor=lz4;Host=localhost;Port=9000;Database=test_db;User=default;Password=",
          "tableName": "test_db.logs",
          "batchPostingLimit": 50,
          "period": "00:00:30",
          "columnOptions": { 
            "RemoveStandardColumns": ["level","message"],
            "OrderBy": ["timestamp","source"],
            "PartitionBy": ["toDate(timestamp)"],
            "TimeToLive": {
              "Table": {
                  "DateTimeColumnName": "timestamp",
                  "IntervalValue": 12,
                  "IntervalName": "MONTH"
              },
              "Fields": {
                  "source": {
                      "DateTimeColumnName": "timestamp",
                      "IntervalValue": 3,
                      "IntervalName": "WEEK"
                  },
                  "callstack": {
                      "DateTimeColumnName": "timestamp",
                      "IntervalValue": 3,
                      "IntervalName": "WEEK"
                  }
              }
            }
          },
          "additionalColumns": [{
              "Name": "source",
              "Type": "String"
            },
            {
              "Name": "callstack",
              "Type": "String"
            },
            {
              "Name": "user",
              "Type": "String"
            }]
        }
      }
    ]
  }
}
 ```
 ```C#
 var configuration = new ConfigurationBuilder()
  .AddJsonFile($"appsettings.json", true)
  .Build();

var logFromAppsettings = new LoggerConfiguration()
  .ReadFrom.Configuration(configuration)
  .Enrich.WithProperty("source", "network")
  .Enrich.WithProperty("user", "admin")
  .CreateLogger();
 ```
