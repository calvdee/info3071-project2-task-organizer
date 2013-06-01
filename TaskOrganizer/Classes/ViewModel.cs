using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskOrganizer.Classes
{
    public class ViewModel
    {
        public ObservableCollection<Task> Collection;
        public Dictionary<TaskPriority, Classes.Task> TreeCollection;
        public string[] TaskStatuses { get; private set; }
        public string[] TaskPriorities { get; private set; }
        

        public ViewModel()
        {
            this.Collection = new ObservableCollection<Task>();
            BuildCombos();
        }

        private void BuildCombos()
        {
            this.TaskStatuses = new string[]
            {
                Enum.GetName(typeof(TaskPriority), TaskPriority.LOW),
                Enum.GetName(typeof(TaskPriority), TaskPriority.MEDIUM),
                Enum.GetName(typeof(TaskPriority), TaskPriority.HIGH)
            };

            this.TaskPriorities = new string[]
            {
                Enum.GetName(typeof(Classes.TaskStatus), Classes.TaskStatus.CREATED),
                Enum.GetName(typeof(Classes.TaskStatus), Classes.TaskStatus.STARTED),
                Enum.GetName(typeof(Classes.TaskStatus), Classes.TaskStatus.DONE),
                Enum.GetName(typeof(Classes.TaskStatus), Classes.TaskStatus.OVERDUE),
            };
        }
    }
}
