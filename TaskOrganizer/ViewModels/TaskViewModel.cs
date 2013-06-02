using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskOrganizer.Tasks;

namespace TaskOrganizer.ViewModels
{
    public class TaskViewModel : Task
    {
        public TaskViewModel Instance { get { return this; } }
        public List<string> PropertyNodes
        {
            get
            {
                return new List<string>()
                {
                    "Status: " + this.Status,
                    "Date Started: " + this.DateStarted,
                    "Date Due: " + this.DueDate
                };
            }
        }

        protected TaskViewModel() { }

        /// <summary>
        /// Factory method to create `TaskViewModel` objects.
        /// </summary>
        /// <param name="task">The task from which the new object will be created.</param>
        /// <returns></returns>
        public static TaskViewModel FromTask(Task task)
        {
            return new TaskViewModel()
            {
                DateStarted = task.DateStarted,
                Description = task.Description,
                Details = task.Details,
                DueDate = task.DueDate,
                Priority = task.Priority,
                Status = task.Status,
                TaskName = task.TaskName
            };
        }

        public override string ToString()
        {
            return this.TaskName;
        }
    }
}
