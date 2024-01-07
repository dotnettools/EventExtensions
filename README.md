# EventExtensions
This library adds a few extension methods to events. While it's not best practice to have async events, this library attempts to provide the following features:

- Collect the returned values of event handlers synchronously or asynchronously.
- Invoke an event asynchronously.
- Handles NULL events; since it checks for null values inside the extension methods and thus throws no null-pointer exceptions.

## What is exactly added?
- `InvokeAsync` to simply invoke async events
- `Collect` to collect the returned values by invoking synchronous events
- `CollectAsync` to invoke async events and collect the returned values
- `AsyncEventHandler` and `AsyncEventHandler<T>` as the asynchronous versions of `EventHandler` and `EventHandler<T>`

## Installation
Install via <a href="https://www.nuget.org/packages/EventExtensions/">NuGet</a>.

    Install-Package EventExtensions -Version 2.2.0

## Getting Started

```csharp
using EventExtensions;
using System;

public class Program
{
    public event Func<int, Task<int>> MyEvent;

    public async Task Run() {
        MyEvent += SimpleHandler;
        var results = await MyEvent.CollectAsync(8); // There are other flavors to this method such as 'InvokeAsync'!
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
