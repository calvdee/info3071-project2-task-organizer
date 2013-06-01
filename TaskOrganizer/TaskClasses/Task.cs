using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskOrganizer.TaskClasses
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

    public class Task
    {
        public const string DateFormat = "dddd MM, yyyy";
        public string TaskName { get; set; }
        public string Description { get; set; }
        public DateTime DateStarted { get; set; }
        public DateTime DueDate { get; set; }
        public TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public string Details { get; set; }

        public override string ToString()
        {
            return this.TaskName;
        }
    }
}
