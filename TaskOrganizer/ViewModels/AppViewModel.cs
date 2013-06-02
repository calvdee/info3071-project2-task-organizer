using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ServiceStack.Text;
using TaskOrganizer.Tasks;
using System.IO;
using System.Windows;

namespace TaskOrganizer.ViewModels
{
    /// <summary>
    /// Class used to encapsulate application logic.
    /// </summary>
    public class AppViewModel
    {
        const string DATA_FILE = "tasks.json";
        public Dictionary<TaskPriority, TaskList> TaskMap { get; private set; }

        public AppViewModel()
        {
            this.TaskMap = new Dictionary<TaskPriority, TaskList>()
            {
                { TaskPriority.LOW, new TaskList(TaskPriority.LOW) },
                { TaskPriority.MEDIUM, new TaskList(TaskPriority.MEDIUM) },
                { TaskPriority.HIGH, new TaskList(TaskPriority.HIGH) },
            };

            LoadFromDisk();
        }

        /// <summary>
        /// Adds the task to the `TaskMap` collection and persists the task to disk.
        /// </summary>
        /// <param name="newTask">The new task to save.</param>
        public void SaveTask(TaskViewModel newTask)
        {
            TaskPriority priority = GetPriorityEnum(newTask.Priority);

            this.TaskMap[priority].Tasks.Add(newTask);
        }

        /// <summary>
        /// Modifies the collection of tasks updating the existing task to the new task.
        /// </summary>
        /// <param name="existingTask">A reference to the existing task.</param>
        /// <param name="newTask">The new task used to overwrite the existing task.</param>
        public void SaveTask(ref TaskViewModel existingTask, TaskViewModel newTask)
        {
            TaskPriority priority = GetPriorityEnum(existingTask.Priority);

            // Iterate over the existing tasks and find a matching object
            Task task = null;
            for (int idx = 0; idx < this.TaskMap[priority].Tasks.Count; ++idx)
            {
                task = this.TaskMap[priority].Tasks[idx];

                if (task.Equals(existingTask))
                {
                    // Need to switch the priorities in the map if it's changed.
                    if (GetPriorityEnum(newTask.Priority) != GetPriorityEnum(existingTask.Priority))
                    {
                        this.TaskMap[priority].Tasks.RemoveAt(idx);
                        priority = GetPriorityEnum(newTask.Priority);
                        this.TaskMap[priority].Tasks.Add(newTask);
                    }
                    else
                        this.TaskMap[priority].Tasks[idx] = newTask;

                    break;
                }
            }
        }

        /// <summary>
        /// Removes the task from the colleciton of tasks.
        /// </summary>
        /// <param name="existingTask">A reference to the existing task.</param>
        public void DeleteTask(ref TaskViewModel existingTask)
        {
            TaskPriority priority = GetPriorityEnum(existingTask.Priority);

            // Iterate over the existing tasks and find a matching object
            Task task = null;
            for (int idx = 0; idx < this.TaskMap[priority].Tasks.Count; ++idx)
            {
                task = this.TaskMap[priority].Tasks[idx];

                if (task.Equals(existingTask))
                {
                    this.TaskMap[priority].Tasks.RemoveAt(idx);
                    break;
                }
            }
        }

        /// <summary>
        /// Writes the tasks to a JSON file.
        /// </summary>
        public void DumpToDisk()
        {
            JsonArrayObjects jsonArray = new JsonArrayObjects();


            try
            {
                foreach (KeyValuePair<TaskPriority, TaskList> kvp in this.TaskMap)
                {
                    // Get each task and create a JSON object
                    foreach (TaskViewModel taskVM in kvp.Value.Tasks)
                    {
                        Task task = new Task()
                        {
                            DateStarted = taskVM.DateStarted,
                            Description = taskVM.Description,
                            Details = taskVM.Details,
                            DueDate = taskVM.DueDate,
                            Priority = taskVM.Priority,
                            Status = taskVM.Status,
                            TaskName = taskVM.TaskName
                        };

                        jsonArray.Add(JsonObject.Parse(task.ToJson()));
                    }
                }

                string fPath = Path.Combine(Directory.GetCurrentDirectory(), DATA_FILE);
                File.WriteAllText(fPath, jsonArray.SerializeToString());

            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error: Failed to save tasks: " + ex.Message,
                    "Save Tasks Failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public void LoadFromDisk()
        {
            try
            {
                string fPath = Path.Combine(Directory.GetCurrentDirectory(), DATA_FILE);

                // NO file, don't do anything.
                if (!File.Exists(fPath))
                    return;

                string json = File.ReadAllText(fPath);

                JsonArrayObjects oArray = JsonObject.ParseArray(json);

                // Iterate over the JSON objects and add to the TaskMap for each one.
                Task task = null;
                foreach (JsonObject o in oArray)
                {
                    task = JsonSerializer.DeserializeFromString<Task>(o.SerializeToString());
                    
                    TaskPriority priority = GetPriorityEnum(task.Priority);
                    
                    // Use the factory method to create a view model.
                    this.TaskMap[priority].Tasks.Add(TaskViewModel.FromTask(task));
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Returns a `TaskPriority` enumeration from a string.
        /// </summary>
        /// <param name="priority">The string version of the TaskPriority.</param>
        /// <returns>A TaskPriority enumerated value.</returns>
        public static TaskPriority GetPriorityEnum(string priority)
        {
            return (TaskPriority)Enum.Parse(typeof(TaskPriority), priority);
        }
    }

    /// <summary>
    /// Class used for TreeView data binding.
    /// </summary>
    public class TaskList
    {
        public string Priority { get; set; }
        public ObservableCollection<TaskViewModel> Tasks { get; set; }

        public TaskList(TaskPriority priority)
        {
            this.Priority = Enum.GetName(typeof(TaskPriority), priority);
            this.Tasks = new ObservableCollection<TaskViewModel>();
        }
    }
}
