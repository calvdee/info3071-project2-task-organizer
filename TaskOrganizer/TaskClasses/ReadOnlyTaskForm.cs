using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TaskOrganizer.TaskClasses
{
    public class ReadOnlyTaskForm
    {
        private Label _lblName;
        private Label _lblDescription;
        private Label _lblDateStarted;
        private Label _lblStatus;
        private Label _lblPriority;
        private Label _lblDetails;
        private Label _lblDateDue;
        private List<Label> _labelCollection;

        public List<Label> LabelCollection { get { return _labelCollection; } }
        public Label NameLabel { get { return _lblName; } }
        public Label DescriptionLabel { get { return _lblDescription; } }
        public Label DateStartedLabel { get { return _lblDateStarted; } }
        public Label DateDueLabel { get { return _lblDateDue; } }
        public Label StatusLabel { get { return _lblStatus; } }
        public Label PriorityLabel { get { return _lblPriority; } }
        public Label DetailsLabel { get { return _lblDetails; } }

        protected ReadOnlyTaskForm(Task task)
        {
            _labelCollection = new List<Label>();
            _lblName = BuildLabel(task.TaskName, 0, 1);
            _lblDescription = BuildLabel(task.Description, 1, 1);
            _lblDateStarted = BuildLabel(task.DateStarted.ToString(Task.DateFormat), 2, 1);
            _lblDateDue = BuildLabel(task.DueDate.ToString(Task.DateFormat), 3, 1);
            _lblStatus = BuildLabel(Enum.GetName(typeof(TaskStatus), task.Status), 4, 1);
            _lblPriority = BuildLabel(Enum.GetName(typeof(TaskPriority), task.Priority), 5, 1);
            _lblDetails = BuildLabel(task.Details, 6, 1);
        }

        /// <summary>
        /// Factory method to create a new form from the task.  Will render the task with labels
        /// so that the properties are read-only.
        /// </summary>
        /// <param name="task">The task from which the fo</param>
        /// <returns>A form for the task with all properties rendered as labels.</returns>
        public static ReadOnlyTaskForm CreateForRead(Task task)
        {
            return new ReadOnlyTaskForm(task);
        }

        /// <summary>
        /// Builds a Label control from the params.
        /// </summary>
        /// <param name="content">The content for the label.</param>
        /// <param name="row">The row of the containing grid.</param>
        /// <param name="col">The column of the containing grid.</param>
        /// <returns></returns>
        private Label BuildLabel(string content, int row, int col)
        {
            Label label = new Label();
            label.Content = content;
            label.Style = Application.Current.FindResource("FormContent") as Style;
            label.SetValue(Grid.RowProperty, row);
            label.SetValue(Grid.ColumnProperty, col);
            _labelCollection.Add(label);
            
            return label;
        }
    }


}
