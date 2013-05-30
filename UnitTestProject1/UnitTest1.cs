using System;
using TaskOrganizer.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TaskIndexTest()
        {
            // Create the tasks
            Task t1 = new Task()
            {
                TaskName = "Task1",
                DueDate = DateTime.Today.Add(TimeSpan.FromDays(1))
            };

            Task t2 = new Task()
            {
                TaskName = "Task2",
                DueDate = DateTime.Today.Add(TimeSpan.FromDays(2))
            };

            Task t3 = new Task()
            {
                TaskName = "Task3",
                DueDate = DateTime.Today.Add(TimeSpan.FromDays(3))
            };

            // Create the task list
            List<Task> taskList = new List<Task>() { t1, t2, t3 };
            TaskIndex index = new TaskIndex(taskList);

            // All of the tasks should have status of created and low priority
            taskList.ForEach(task => Assert.AreEqual(t1.Status, TaskStatus.CREATED));
            taskList.ForEach(task => Assert.AreEqual(t1.Priority, TaskPriority.LOW));

            // Make sure that the next is at a later date than the current task
            DateTime lastDate = DateTime.Today;
            foreach (KeyValuePair<DateTime, List<Task>> kvp in index.DateIndex)
            {
                Assert.IsTrue(kvp.Key > lastDate);
                lastDate = kvp.Key;
            }
        }
    }
}
