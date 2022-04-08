using System.Threading.Tasks;
using UnityEngine;

namespace Xdd.Scripts.Base
{
    internal static class TaskEx
    {
        public static async Task Delay(float delay)
        {
            var time = Time.timeAsDouble + delay;

            while (time > Time.timeAsDouble)
            {
                await Task.Yield();
            }
        }

        public static void Forget(this Task task)
        {
            task.ContinueWith(
                t => { Debug.LogException(t.Exception); },
                TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
