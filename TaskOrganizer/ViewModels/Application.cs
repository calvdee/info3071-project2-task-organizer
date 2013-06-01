using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TaskOrganizer.TaskClasses;

namespace TaskOrganizer.ViewModels
{
    public class ApplicationViewModel
    {
        public ObservableCollection<Task> TaskCollection { get; private set; }
        public Dictionary<TaskPriority, List<Task>> TreeCollection;
        public string[] TaskStatuses { get; private set; }
        public string[] TaskPriorities { get; private set; }

        public ApplicationViewModel()
        {
            this.TaskCollection = new ObservableCollection<Task>();
            this.TreeCollection = new Dictionary<TaskPriority, List<Task>>();
            BuildCombos();
        }

        public void SaveTask(Task newTask)
        {
            TaskPriority priority = newTask.Priority;

            if (TreeCollection.ContainsKey(priority))
                // Append to this priority.
                this.TreeCollection[priority].Add(newTask);
            else
                // No entry for this priority, create and append.
                this.TreeCollection[priority] = new List<Task>() { newTask };

            this.TaskCollection.Add(newTask);
        }

        /// <summary>
        /// Builds the combo items from the TaskPriorty and TaskStatus enumerations.
        /// </summary>
        private void BuildCombos()
        {
            this.TaskStatuses = new string[]
            {
                Enum.GetName(typeof(TaskStatus), TaskStatus.CREATED),
                Enum.GetName(typeof(TaskStatus), TaskStatus.STARTED),
                Enum.GetName(typeof(TaskStatus), TaskStatus.DONE),
                Enum.GetName(typeof(TaskStatus), TaskStatus.OVERDUE),
            };

            this.TaskPriorities = new string[]
            {
                                Enum.GetName(typeof(TaskPriority), TaskPriority.LOW),
                Enum.GetName(typeof(TaskPriority), TaskPriority.MEDIUM),
                Enum.GetName(typeof(TaskPriority), TaskPriority.HIGH)
            };
        }
    }
}
