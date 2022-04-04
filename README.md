# EventExtensions
This library adds a few must-have extension methods to events.

- Collect the returned values of event handlers synchronously or asynchronously.
- Invoke an event asynchronously.
- Handles NULL events; since it checks for null values inside the extension methods and thus throws no null-pointer exceptions.

### Targets
- .NET Standard 2.0 or higher
- .NET Framework 4.5 or higher

## Installation
Install via <a href="https://www.nuget.org/packages/EventExtensions/">NuGet</a>.

    Install-Package EventExtensions -Version 2.0.1

## Getting Started

```csharp
using EventExtensions;
using System;

public class Program
{
    public event Func<int, Task<int>> MyEvent;

    public async Task Run() {
        MyEvent += SimpleHandler;
        var results = await MyEvent.CollectAsync(8);
        foreach (var result in results)
            Console.WriteLine(result);
    }

    private async Task<int> SimpleHandler(int num)
    {
        await Task.Delay(TimeSpan.FromSeconds(1));
        return num * 2;
    }

    public static Task Main(string[] args) {
        return new Program().Run();
    }
}
```
