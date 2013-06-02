using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TaskOrganizer.ViewModels;
using System.Globalization;

namespace TaskOrganizer.Tasks
{
    /// <summary>
    /// Class used to create a form that allows for editing.  All controls are Content controls.
    /// </summary>
    public class EditableTaskForm
    {
        private TextBox _txtName;
        private TextBox _txtDescription;
        private DatePicker _dateStarted;
        private DatePicker _dateDue;
        private ComboBox _cBoxStatus;
        private ComboBox _cBoxPriority;
        private TextBox txtDetails;

        public TextBox Name { get { return _txtName; } }
        public TextBox Description { get { return _txtDescription; } }
        public DatePicker DateStarted { get { return _dateStarted; } }
        public DatePicker DateDue { get { return _dateDue; } }
        public ComboBox Status { get { return _cBoxStatus; } }
        public ComboBox Priority { get { return _cBoxPriority; } }
        public TextBox Details { get { return txtDetails; } }

        protected EditableTaskForm(TaskViewModel task = null)
        {
            _txtName = BuildControl<TextBox>(0, 1, "TextInput");
            _txtDescription = BuildControl<TextBox>(1, 1, "TextInput");
            _dateStarted = BuildControl<DatePicker>(2, 1, "DateInput");
            _dateDue = BuildControl<DatePicker>(3, 1, "DateInput");
            _cBoxStatus = BuildControl<ComboBox>(4, 1, "ComboInput");
            _cBoxPriority = BuildControl<ComboBox>(5, 1, "ComboInput");
            txtDetails = BuildControl<TextBox>(6, 1);

            BuildComboBoxes();

            if (task != null)
            {
                _txtName.Text = task.TaskName;
                _txtDescription.Text = task.Description;
                _dateStarted.SelectedDate = DateTime.Parse(task.DateStarted);
                _dateDue.SelectedDate = DateTime.Parse(task.DueDate);
                _cBoxStatus.SelectedItem = task.Status;
                _cBoxPriority.SelectedItem = task.Priority;
                txtDetails.AppendText(task.Details);
            }
        }

        /// <summary>
        /// Factory method to create a new form with editable controls.
        /// </summary>
        /// <param name="task">The task from which the fo</param>
        /// <returns>A form for the task with all properties rendered as labels.</returns>
        public static EditableTaskForm CreateNew()
        {
            return new EditableTaskForm();
        }

        /// <summary>
        /// Factory method to create a new form with defaults.
        /// </summary>
        /// <param name="task">The task from which the fo</param>
        /// <returns>A form for the task with all properties rendered as labels.</returns>
        public static EditableTaskForm CreateNew(TaskViewModel task)
        {
            return new EditableTaskForm(task);
        }

        /// <summary>
        /// Creates the comobo boxes for TaskPriority and TaskStatus enumerations.
        /// </summary>
        private void BuildComboBoxes()
        {
            _cBoxStatus.ItemsSource = new string[]
            {
                Enum.GetName(typeof(TaskStatus), TaskStatus.CREATED),
                Enum.GetName(typeof(TaskStatus), TaskStatus.STARTED),
                Enum.GetName(typeof(TaskStatus), TaskStatus.DONE),
                Enum.GetName(typeof(TaskStatus), TaskStatus.OVERDUE),
            };

            _cBoxPriority.ItemsSource = new string[]
            {
                Enum.GetName(typeof(TaskPriority), TaskPriority.LOW),
                Enum.GetName(typeof(TaskPriority), TaskPriority.MEDIUM),
                Enum.GetName(typeof(TaskPriority), TaskPriority.HIGH)
            };
        }

        /// <summary>
        /// Builds a control of type T, setting it's grid position and style.
        /// </summary>
        /// <param name="style">The style resource.</param>
        /// <param name="row">The row of the containing grid.</param>
        /// <param name="col">The column of the containing grid.</param>
        /// <returns></returns>
        private T BuildControl<T>(int row, int col, string style = "") where T : Control
        {
            T control = (T)Activator.CreateInstance(typeof(T));
            control.Style = style == "" ? null : Application.Current.FindResource(style) as Style;
            control.SetValue(Grid.RowProperty, row);
            control.SetValue(Grid.ColumnProperty, col);

            return control;
        }
    }
}
