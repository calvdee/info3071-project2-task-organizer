using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Reflection;

namespace TaskOrganizer.Classes
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
        public Task Instance { get { return this; } }
        public string TaskName { get; set; }
        public string Description { get; set; }
        public DateTime DateStarted { get; set; }
        public DateTime DueDate { get; set; }
        public TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public string Details { get; set; }
        public List<string> PropertyNodes 
        { 
            get
            {
                return new List<string>()
                {
                    "Status: " + Enum.GetName(typeof(TaskStatus), this.Status),
                    "Date Started: " + this.DateStarted.ToString(Task.DateFormat),
                    "Date Due: " + this.DueDate.ToString(Task.DateFormat)
                };
            }
        }

        public Task()
        {

        }

        public override string ToString()
        {
            return this.TaskName;
        }
    }

    public class TaskList
    {
        public string Priority { get; set; }
        public ObservableCollection<Task> Tasks { get; set; }

        public TaskList(TaskPriority priority)
        {
            this.Priority = Enum.GetName(typeof(TaskPriority), priority);
            this.Tasks = new ObservableCollection<Task>();
        }
    }

    public class TaskNode : TextBlock
    {
        public TaskNode() : base() { }
        public Task Task { get; set; }
    }
}
