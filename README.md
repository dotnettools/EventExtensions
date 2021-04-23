# EventExtensions
This .NET Standard package adds a few must-have extension methods to events.

- Collect the returned values of event handlers synchronously or asynchronously.
- Invoke an event asynchronously.

## Installation
Install via <a href="https://www.nuget.org/packages/EventExtensions/">NuGet</a>.

    Install-Package EventExtensions -Version 1.0.0

## Getting Started

    using EventExtensions;
    using System;
    
    public class Program
    {
        public event Func<int, Task<int>> MyEvent;
        
        public async void Run() {
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
    
        public static void Main(string[] args) {
            new Program().Run();
        }
    }
