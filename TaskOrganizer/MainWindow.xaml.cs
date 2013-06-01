using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaskOrganizer.Classes;

namespace TaskOrganizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModel _vmTasks;
        private ReadOnlyTaskForm _readOnlyForm;
        private EditableTaskForm _editableTaskForm;
        private TaskFactory _taskFactory = new TaskFactory();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _vmTasks = new ViewModel();

            // Each task list represents a different priority
            List<TaskList> priorities = new List<TaskList>() 
            { 
                _vmTasks.TaskMap[TaskPriority.LOW], 
                _vmTasks.TaskMap[TaskPriority.MEDIUM], 
                _vmTasks.TaskMap[TaskPriority.HIGH] 
            };

            
            DataContext = new
            {
                Priorities = priorities
            };
        }

        void ndeTask_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Classes.Task task = (sender as TextBlock).DataContext as TaskOrganizer.Classes.Task;
        }

        /// <summary>
        /// Asynchronously creates a new EditableTaskForm.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Clear the read only form
            _taskFactory.StartNew(() => ClearReadOnlyForm());

            // Create the new editable task form and set some defaults.
            _editableTaskForm = EditableTaskForm.CreateNew();
            _editableTaskForm.Status.ItemsSource = _vmTasks.TaskStatuses;
            _editableTaskForm.Status.SelectedIndex = 0;
            _editableTaskForm.Priority.ItemsSource = _vmTasks.TaskPriorities;
            _editableTaskForm.Priority.SelectedIndex = 0;
            _editableTaskForm.Details.SetValue(Grid.ColumnSpanProperty, 2);
            
            // Render the form asynchronously and setup tab indexes.
            _taskFactory.StartNew(() =>
            {
                AddToDetailForm(_editableTaskForm.DateDue);
                AddToDetailForm(_editableTaskForm.DateStarted);
                AddToDetailForm(_editableTaskForm.Description);
                AddToDetailForm(_editableTaskForm.Details);
                AddToDetailForm(_editableTaskForm.Name);
                AddToDetailForm(_editableTaskForm.Priority);
                AddToDetailForm(_editableTaskForm.Status);

                Dispatcher.BeginInvoke(new Action(() => btnSave.Visibility = System.Windows.Visibility.Visible));
            })
            .ContinueWith((result) => 
            {
                Dispatcher.BeginInvoke(new Action(() => SetTabIndexes(_editableTaskForm)));
                Dispatcher.BeginInvoke(new Action(() => _editableTaskForm.Name.Focus()));
            });
        }

        /// <summary>
        /// Adds the control to the task detail form.  The row and column properties should be
        /// set on the control at this point.
        /// </summary>
        /// <param name="control">The control to add to the grid.</param>
        private void AddToDetailForm(Control control)
        {
            Dispatcher.BeginInvoke(new Action(() => gridTaskDetail.Children.Add(control)));
        }

        /// <summary>
        /// Removes all of the labels from the task detail grid.
        /// </summary>
        private void ClearReadOnlyForm()
        {
            if(_readOnlyForm != null)
            {
                foreach(Label label in _readOnlyForm.LabelCollection)
                {
                    UIElement child = null;
                    Dispatcher.BeginInvoke(new Action(() => 
                        child = gridTaskDetail.FindName(label.Name) as UIElement));
                    gridTaskDetail.Children.Remove(child);
                }
            }
        }

        /// <summary>
        /// Sets the tab indexes for form input fields.
        /// </summary>
        /// <param name="form">The form.</param>
        private void SetTabIndexes(EditableTaskForm form)
        {
            form.Name.TabIndex = 0;
            form.Description.TabIndex = 1;
            form.DateStarted.TabIndex = 2;
            form.DateDue.TabIndex = 3;
            form.Status.TabIndex = 4;
            form.Priority.TabIndex = 5;
            form.Details.TabIndex = 6;
        }

        /// <summary>
        /// Handles the save by creating a new task object and calling the view model's `SaveTask()`
        /// method which will add the task to the tree view and persist the updated task list to disk.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Classes.TaskStatus status = (Classes.TaskStatus)Enum.Parse(
                    typeof(Classes.TaskStatus), 
                    _editableTaskForm.Status.SelectedItem.ToString());

            Classes.TaskPriority priority = (Classes.TaskPriority)Enum.Parse(
                    typeof(Classes.TaskPriority), 
                    _editableTaskForm.Priority.SelectedItem.ToString());

            Classes.Task newTask = new Classes.Task()
            {
                TaskName = _editableTaskForm.Name.Text,
                DateStarted = _editableTaskForm.DateStarted.SelectedDate ?? DateTime.Today,
                DueDate = _editableTaskForm.DateDue.SelectedDate ?? DateTime.Today,
                Description = _editableTaskForm.Description.Text,
                Details = new TextRange(
                    _editableTaskForm.Details.Document.ContentStart, 
                    _editableTaskForm.Details.Document.ContentEnd).Text,
                Priority = priority,
                Status = status
            };

            _vmTasks.SaveTask(newTask);
        }
    }
}
