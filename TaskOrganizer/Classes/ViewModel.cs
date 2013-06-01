using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskOrganizer.Classes
{
    public class AppViewModel
    {
        public ObservableCollection<Task> TaskCollection { get; private set; }
        public Dictionary<TaskPriority, TaskList> TaskMap { get; private set; }
        //public string[] TaskStatuses { get; private set; }
        //public string[] TaskPriorities { get; private set; }

        public AppViewModel()
        {
            this.TaskCollection = new ObservableCollection<Task>();
            this.TaskMap = new Dictionary<TaskPriority, TaskList>()
            {
                { TaskPriority.LOW, new TaskList(TaskPriority.LOW) },
                { TaskPriority.MEDIUM, new TaskList(TaskPriority.MEDIUM) },
                { TaskPriority.HIGH, new TaskList(TaskPriority.HIGH) },
            };
        }

        /// <summary>
        /// Adds the task to the `TaskMap` collection and persists the task to disk.
        /// </summary>
        /// <param name="newTask"></param>
        public void SaveTask(Task newTask)
        {
            TaskPriority priority = newTask.Priority;

            this.TaskMap[priority].Tasks.Add(newTask);

            //TODO: Save to disk
        }

        public void SaveTask(ref Task existingTask, Task newTask)
        {
            TaskPriority priority = existingTask.Priority;

            // Iterate over the existing tasks and find a matching object
            for (int idx = 0; idx < this.TaskMap[priority].Tasks.Count; ++idx)
            {
                if (this.TaskMap[priority].Tasks[idx].Equals(existingTask))
                {
                    this.TaskMap[priority].Tasks[idx] = newTask;
                    break;
                }
            }
        }

        public void DeleteTask(ref Task existingTask)
        {
            TaskPriority priority = existingTask.Priority;

            // Iterate over the existing tasks and find a matching object
            for (int idx = 0; idx < this.TaskMap[priority].Tasks.Count; ++idx)
            {
                if (this.TaskMap[priority].Tasks[idx].Equals(existingTask))
                {
                    this.TaskMap[priority].Tasks.RemoveAt(idx);
                    break;
                }
            }
        }
    }
}
