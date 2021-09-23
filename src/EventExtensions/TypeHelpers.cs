using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventExtensions
{
    internal static class TypeHelpers
    {
        public static bool IsTaskType(this Type type)
            => type.IsSubclassOf(typeof(Task));

        public static Type GetTaskResultType(this Type taskType)
        {
            if (!IsTaskType(taskType))
                throw new InvalidOperationException("The specified type is not a Task.");
            if (taskType == typeof(Task))
                return null;
#if NETSTANDARD2_1_OR_GREATER
            if (taskType == typeof(ValueTask))
                return null;
#endif
            return taskType.GetGenericArguments().Single();
        }
    }
}
