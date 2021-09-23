using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventExtensions
{
    public static class EventsExtensions
    {
        /// <summary>
        /// Collects the results of the invocation of all handlers.
        /// </summary>
        public static IEnumerable<object> CollectEnumerable(this Delegate @delegate, params object[] @params)
        {
            if (@delegate == null)
                yield break;

            var invocationList = @delegate.GetInvocationList();

            for (int i = 0; i < invocationList.Length; i++)
            {
                var result = invocationList[i].DynamicInvoke(@params);
                yield return result;
            }
        }

        /// <summary>
        /// Collects the results of the invocation of all handlers.
        /// </summary>
        public static object[] Collect(this Delegate @delegate, params object[] @params)
        {
            if (@delegate == null)
#if NETSTANDARD2_0_OR_GREATER
                return Array.Empty<object>();
#else
                return new object[0];
#endif

            var invocationList = @delegate.GetInvocationList();
            var results = new object[invocationList.Length];

            for (int i = 0; i < invocationList.Length; i++)
            {
                var result = invocationList[i].DynamicInvoke(@params);
                results[i] = result;
            }

            return results;
        }

        /// <summary>
        /// Collects the results of type <typeparamref name="T"/> as the result of the invocation of all handlers.
        /// </summary>
        public static T[] Collect<T>(this Delegate @delegate, params object[] @params)
            => CollectEnumerable(@delegate, @params).OfType<T>().ToArray();

        /// <summary>
        /// Collects the results of the invocation of all handlers asynchronously.
        /// </summary>
        public static async Task<object[]> CollectAsync(this Delegate @delegate, params object[] @params)
        {
            var results = new List<object>();

            foreach (var invocationResult in Collect(@delegate, @params))
            {
                object result;
                if (invocationResult is Task task)
                {
                    await task.ConfigureAwait(false);
                    result = ExtractResult(task, out var hasResult);
                    if (hasResult)
                        results.Add(result);
                }
                else
                    results.Add(invocationResult);
            }

            return results.ToArray();
        }

        /// <summary>
        /// Collects the results of type <typeparamref name="T"/> as the result of asynchronous invocation of all 
        /// handlers.
        /// </summary>
        public static async Task<T[]> CollectAsync<T>(this Delegate @delegate, params object[] @params)
        {
            var results = await CollectAsync(@delegate, @params).ConfigureAwait(false);
            return results.OfType<T>().ToArray();
        }

        /// <summary>
        /// Invokes all of the invocation handlers and returns a <see cref="Task"/> that completes when
        /// all of the handlers are successfully called.
        /// </summary>
        public static Task InvokeAsync(this Delegate @delegate, params object[] @params)
        {
            var tasks = Collect<Task>(@delegate, @params);
            return Task.WhenAll(tasks);
        }

        private static object ExtractResult(Task task, out bool hasResult)
        {
            var type = task.GetType();
            var resultType = TypeHelpers.GetTaskResultType(type);
            if (resultType == null)
            {
                hasResult = false;
                return null;
            }

            var resultProperty = type.GetProperty(nameof(Task<bool>.Result));
            if (resultProperty == null)
                throw new InvalidOperationException("Could not find the result property of the Task.");
            hasResult = true;
            return resultProperty.GetValue(task);
        }
    }
}
