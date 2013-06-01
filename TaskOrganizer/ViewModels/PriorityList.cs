using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TaskOrganizer.TaskClasses;

namespace TaskOrganizer.ViewModels
{
    public class TaskViewModel
    {
        public string Priority { get; set; }
        public List<Task> Tasks { get; set; }
    }
}
