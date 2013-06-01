﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskOrganizer.Classes
{
    public class ViewModel
    {
        public ObservableCollection<Task> TaskCollection { get; private set; }
        public Dictionary<TaskPriority, TaskList> TaskMap { get; private set; }
        public string[] TaskStatuses { get; private set; }
        public string[] TaskPriorities { get; private set; }

        public ViewModel()
        {
            this.TaskCollection = new ObservableCollection<Task>();
            this.TaskMap = new Dictionary<TaskPriority, TaskList>()
            {
                { TaskPriority.LOW, new TaskList(TaskPriority.LOW) },
                { TaskPriority.MEDIUM, new TaskList(TaskPriority.MEDIUM) },
                { TaskPriority.HIGH, new TaskList(TaskPriority.HIGH) },
            };

            BuildCombos();
        }

        /// <summary>
        /// Adds the task to the `TaskMap` collection and persists the task to disk.
        /// </summary>
        /// <param name="newTask"></param>
        public void SaveTask(Task newTask)
        {
            TaskPriority priority = newTask.Priority;

            this.TaskMap[priority].Tasks.Add(newTask);

            //TODO: Save to disk
        }

        /// <summary>
        /// Builds the combo items from the TaskPriorty and TaskStatus enumerations.
        /// </summary>
        private void BuildCombos()
        {
            this.TaskStatuses = new string[]
            {
                Enum.GetName(typeof(Classes.TaskStatus), Classes.TaskStatus.CREATED),
                Enum.GetName(typeof(Classes.TaskStatus), Classes.TaskStatus.STARTED),
                Enum.GetName(typeof(Classes.TaskStatus), Classes.TaskStatus.DONE),
                Enum.GetName(typeof(Classes.TaskStatus), Classes.TaskStatus.OVERDUE),
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
