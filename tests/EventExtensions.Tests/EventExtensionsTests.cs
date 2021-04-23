using EventExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MateMachine.Core.Tests.Utils
{
    public class EventExtensionsTests
    {
        private event Func<Task> AsyncEvent;
        private event Func<int> IntEvent;
        private event Func<Task<int>> AsyncIntEvent;

        [Fact]
        public async void Test_AwaitForAllTasksCorrectly()
        {
            bool a = false, b = false;
            AsyncEvent += async () => a = await WaitAndReturn(TimeSpan.FromMilliseconds(500), true).ConfigureAwait(false);
            AsyncEvent += async () => b = await WaitAndReturn(TimeSpan.FromSeconds(1), true).ConfigureAwait(false);
            await AsyncEvent.InvokeAsync().ConfigureAwait(false);
            Assert.True(a);
            Assert.True(b);
        }

        [Theory]
        [InlineData(8, 85, 12, 45)]
        public void Test_Collect(params int[] numbers)
        {
            foreach (var num in numbers)
                IntEvent += () => num;
            var collection = IntEvent.Collect<int>();
            Assert.Equal(numbers, collection);
        }

        [Theory]
        [InlineData(1, 2, 3, 5, 6, 7)]
        public async void Test_CollectAsync(params int[] numbers)
        {
            foreach (var num in numbers)
                AsyncIntEvent += () => Task.FromResult(num);
            var collection = await AsyncIntEvent.CollectAsync<int>();
            Assert.Equal(numbers, collection);
        }

        private static async Task<T> WaitAndReturn<T>(TimeSpan wait, T value)
        {
            await Task.Delay(wait).ConfigureAwait(false);
            return value;
        }
    }
}
