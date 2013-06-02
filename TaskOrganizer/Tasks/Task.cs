using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Reflection;

namespace TaskOrganizer.Tasks
{
    public enum TaskStatus
    {
        CREATED,
        STARTED,
        DONE,
        OVERDUE
    }

    public enum TaskPriority
    {
        LOW,
        MEDIUM,
        HIGH
    }

    /// <summary>
    /// Class used to represent a task.
    /// </summary>
    public class Task
    {
        private TaskStatus _status;
        private TaskPriority _priority;

        public const string DateFormat = "yyyy/MM/dd";
        public string TaskName { get; set; }
        public string Description { get; set; }
        public string DateStarted { get; set; }
        public string DueDate { get; set; }
        public string Status 
        {
            get { return Enum.GetName(typeof(TaskStatus), _status); }
            set { _status = (TaskStatus)Enum.Parse(typeof(TaskStatus), value); }
        }
        public string Priority
        {
            get { return Enum.GetName(typeof(TaskPriority), _priority); }
            set { _priority = (TaskPriority)Enum.Parse(typeof(TaskPriority), value); }
        }
        public string Details { get; set; }
    }


}
