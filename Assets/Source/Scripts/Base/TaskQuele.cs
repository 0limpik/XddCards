using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]
namespace Xdd.Scripts.Base
{
    internal class TaskQueue
    {
        public event Action<TaskItem> OnTaskAdd;
        public event Action OnQueueEnd;

        private Queue<TaskItem> tasksQueue = new Queue<TaskItem>();

        private Task currentTask = Task.CompletedTask;

        public ITaskItem AddTask(
            Func<TaskItem, Task> action,
            Func<TaskItem, Task> after = null,
            Func<TaskItem, Task> before = null,
            string name = null)
        {
            var task = new TaskItem(action, after, before, name);
            tasksQueue.Enqueue(task);
            OnTaskAdd?.Invoke(task);
            if (!loop)
                TaskLoop().Forget();

            return task;
        }

        public async Task WaitQueleEnd()
        {
            do
            {
                await Task.Yield();
            } while (!currentTask.IsCompleted);
        }

        private bool loop;

        private async Task TaskLoop()
        {
            loop = true;
            while (tasksQueue.Count > 0)
            {
                var task = tasksQueue.Dequeue();
                currentTask = task.Execute();
                await currentTask;
            }
            loop = false;
            OnQueueEnd?.Invoke();
        }
    }

    internal class TaskItem : ITaskItem
    {
        public string Name { get; private set; }

        public event Action<TaskItem> OnStart;
        public event Action<TaskItem> OnEnd;

        public TaskItemStatus Status { get; private set; } = TaskItemStatus.Wait;

        public Func<TaskItem, Task> after;
        public Func<TaskItem, Task> action;
        public Func<TaskItem, Task> before;

        public TaskItem(
            Func<TaskItem, Task> action,
            Func<TaskItem, Task> after,
            Func<TaskItem, Task> before,
            string name = null)
        {
            this.action = action ?? throw new ArgumentNullException();
            this.after = after;
            this.before = before;
            this.Name = name;
        }

        public async Task Execute()
        {
            Status = TaskItemStatus.Run;
            OnStart?.Invoke(this);
            if (before != null)
                await before(this);

            await action(this);

            if (after != null)
                await after(this);
            Status = TaskItemStatus.Complete;
            OnEnd?.Invoke(this);
        }
    }

    internal interface ITaskItem
    {
        TaskItemStatus Status { get; }

        event Action<TaskItem> OnStart;
        event Action<TaskItem> OnEnd;
    }

    internal enum TaskItemStatus
    {
        Wait,
        Run,
        Complete
    }
}

