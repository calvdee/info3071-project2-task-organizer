using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TaskOrganizer.Classes
{
    public class EditableTaskForm
    {
        private TextBox _txtName;
        private TextBox _txtDescription;
        private DatePicker _dateStarted;
        private DatePicker _dateDue;
        private ComboBox _cBoxStatus;
        private ComboBox _cBoxPriority;
        private RichTextBox _rtbDetails;
        private Button _btnSave;

        public TextBox Name { get { return _txtName; } }
        public TextBox Description { get { return _txtDescription; } }
        public DatePicker DateStarted { get { return _dateStarted; } }
        public DatePicker DateDue { get { return _dateDue; } }
        public ComboBox Status { get { return _cBoxStatus; } }
        public ComboBox Priority { get { return _cBoxPriority; } }
        public RichTextBox Details { get { return _rtbDetails; } }

        protected EditableTaskForm()
        {
            _txtName = BuildControl<TextBox>(0, 1, "TextInput");
            _txtDescription = BuildControl<TextBox>(1, 1, "TextInput");
            _dateStarted = BuildControl<DatePicker>(2, 1, "DateInput");
            _dateDue = BuildControl<DatePicker>(3, 1, "DateInput");
            _cBoxStatus = BuildControl<ComboBox>(4, 1, "ComboInput");
            _cBoxPriority = BuildControl<ComboBox>(5, 1, "ComboInput");
            _rtbDetails = BuildControl<RichTextBox>(6, 1);
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
