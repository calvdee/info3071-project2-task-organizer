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

        public ViewModel()
        {
            this.Collection = new ObservableCollection<Task>();
        }
    }
}
