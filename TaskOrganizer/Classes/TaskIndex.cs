using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskOrganizer.Classes
{
    public class TaskIndex
    {
        private List<Task> _list;

        public List<Task> TaskList { get { return _list; } }
        public Dictionary<DateTime, List<Task>> DateIndex { get; private set; }
        
        public TaskIndex() { }
        public TaskIndex(List<Task> tasks)
        {
            this._list = tasks;
            this.DateIndex = new Dictionary<DateTime, List<Task>>();
        }


        public void AddTask(Task task)
        {
            _list.Add(task);
            rebuildDateIndex();
        }

        /// <summary>
        /// Rebuilds the date index by ordering by the due date.
        /// </summary>
        private void rebuildDateIndex()
        {
            this.DateIndex = 
                _list.OrderBy(t => t.DueDate)
                .GroupBy(t => t.DueDate)
                .ToDictionary(tGroup => tGroup.Key, tGroup => tGroup.ToList());
        }
    }
}
