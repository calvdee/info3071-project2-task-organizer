using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaskOrganizer.Tasks;
using TaskOrganizer.Tasks.Forms;
using TaskOrganizer.ViewModels;

namespace TaskOrganizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AppViewModel _vmTasks;
        private EditableTaskForm _editableTaskForm;
        private System.Threading.Tasks.TaskFactory _taskFactory = new System.Threading.Tasks.TaskFactory();
        private List<Label> _forLabels = new List<Label>();
        private TaskViewModel _selectedTask = null;

        public MainWindow()
        {
            InitializeComponent();
        }


        #region Form Helpers
        
        /// <summary>
        /// Adds the control to the task detail form.  The row and column properties should be
        /// set on the control at this point.
        /// </summary>
        /// <param name="control">The control to add to the grid.</param>
        private void AddToDetailGrid(Control control)
        {
            Dispatcher.BeginInvoke(new Action(() => gridTaskDetail.Children.Add(control)));
        }

        /// <summary>
        /// Clears the task detail grid of all it's controls.
        /// </summary>
        private void ClearForm()
        {
            Dispatcher.BeginInvoke(new Action(() => gridTaskDetail.Children.Clear()));
            Dispatcher.BeginInvoke(new Action(() => BuildForLabels()));
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
        /// Builds and renders the for-labels for the task detail fields.
        /// </summary>
        private void BuildForLabels()
        {
            for (int idx = 0; idx < _forLabels.Count; ++idx)
            {
                // Set the row to the index
                _forLabels[idx].SetValue(Grid.RowProperty, idx);

                // Set the column to 0
                _forLabels[idx].SetValue(Grid.ColumnProperty, 0);

                // Set the global style
                _forLabels[idx].Style = Application.Current.FindResource("ForLabel") as Style;

                AddToDetailGrid(_forLabels[idx]);
            }
        }

        /// <summary>
        /// Builds a form with input controls and sets default content if there is task.
        /// </summary>
        /// <param name="task">The task used to create default values for the controls.</param>
        private void BuildEditableForm(TaskViewModel task = null)
        {
            // Create the new editable task form and set some defaults.
            if (task == null)
            {
                _editableTaskForm = EditableTaskForm.CreateNew();
                _editableTaskForm.Status.SelectedIndex = 0;
                _editableTaskForm.Priority.SelectedIndex = 0;
                _editableTaskForm.DateStarted.SelectedDate = DateTime.Today;
                _editableTaskForm.DateDue.SelectedDate = DateTime.Today;
            }
            else
                _editableTaskForm = EditableTaskForm.CreateNew(task);
            
            _editableTaskForm.Details.SetValue(Grid.ColumnSpanProperty, 2);
            _editableTaskForm.DateStarted.SelectedDateFormat = DatePickerFormat.Long;
            _editableTaskForm.DateDue.SelectedDateFormat = DatePickerFormat.Long;
            _editableTaskForm.Details.TextWrapping = TextWrapping.Wrap;

            // Render the form asynchronously and setup tab indexes.
            _taskFactory.StartNew(() =>
            {
                AddToDetailGrid(_editableTaskForm.DateDue);
                AddToDetailGrid(_editableTaskForm.DateStarted);
                AddToDetailGrid(_editableTaskForm.Description);
                AddToDetailGrid(_editableTaskForm.Details);
                AddToDetailGrid(_editableTaskForm.Name);
                AddToDetailGrid(_editableTaskForm.Priority);
                AddToDetailGrid(_editableTaskForm.Status);

                Dispatcher.BeginInvoke(new Action(() => btnSave.Visibility = System.Windows.Visibility.Visible));
                Dispatcher.BeginInvoke(new Action(() => btnCancel.Visibility = System.Windows.Visibility.Visible));
            })
            .ContinueWith((result) =>
            {
                // Done rendering, set tab indexes and focus.
                Dispatcher.BeginInvoke(new Action(() => SetTabIndexes(_editableTaskForm)));
                Dispatcher.BeginInvoke(new Action(() => _editableTaskForm.Name.Focus()));
            });
        }

        /// <summary>
        /// Builds a form for the task, rendering all of the fields as labels.
        /// </summary>
        /// <param name="?"></param>
        private void BuildReadOnlyForm(TaskViewModel task)
        {
            ReadOnlyTaskForm form = ReadOnlyTaskForm.Create(task);

            _taskFactory.StartNew(() => form.LabelCollection.ForEach(l => AddToDetailGrid(l)));

            _selectedTask = task;
        }

        #endregion

        #region UI Events

        /// <summary>
        /// Asynchronously creates a new EditableTaskForm.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            _selectedTask = null;

            // Clear the read only form
            _taskFactory.StartNew(() => ClearForm());
            BuildEditableForm();
        }

        /// <summary>
        /// Handles the save by creating a new task object and calling the view model's `SaveTask()`
        /// method which will add the task to the tree view and persist the updated task list to disk.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Create Task instance
            TaskViewModel newTask = TaskViewModel.FromTask(new Task()
            {
                TaskName = _editableTaskForm.Name.Text,
                DateStarted = DateTime.Parse(_editableTaskForm.DateStarted.SelectedDate.ToString()).ToString(Task.DateFormat),
                DueDate = DateTime.Parse(_editableTaskForm.DateDue.SelectedDate.ToString()).ToString(Task.DateFormat),
                Description = _editableTaskForm.Description.Text,
                Details = _editableTaskForm.Details.Text,
                Priority = _editableTaskForm.Priority.SelectedItem.ToString(),
                Status = _editableTaskForm.Status.SelectedItem.ToString()
            });

            // Save the task
            if (_selectedTask == null)
            {
                _vmTasks.SaveTask(newTask);

                // Reset the editable form async
                _taskFactory.StartNew(() => ClearForm())
                    .ContinueWith((result) =>
                        Dispatcher.BeginInvoke(new Action(() => BuildEditableForm())));
            }
            else
            {
                _vmTasks.SaveTask(ref _selectedTask, newTask);

                // Render the read only form async
                _taskFactory.StartNew(() => ClearForm())
                    .ContinueWith((result) =>
                        Dispatcher.BeginInvoke(new Action(() => BuildReadOnlyForm(newTask))));
            }



            btnSave.Visibility = System.Windows.Visibility.Hidden;
            btnCancel.Visibility = System.Windows.Visibility.Hidden;
            btnEdit.Visibility = System.Windows.Visibility.Hidden;

            // Reset the selected task
            _selectedTask = null;
        }

        /// <summary>
        /// Causes all of the form elements to be rendered as input controls.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            _taskFactory.StartNew(() => ClearForm());

            BuildEditableForm(_selectedTask);
        }

        /// <summary>
        /// Brings the selected task into context for viewing and editing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ndeTask_MouseUp(object sender, MouseButtonEventArgs e)
        {
            TaskViewModel task = (sender as TextBlock).DataContext as TaskViewModel;

            _taskFactory.StartNew(() => ClearForm());

            BuildReadOnlyForm(task);

            btnEdit.Visibility = System.Windows.Visibility.Visible;
            btnRemove.IsEnabled = true;
        }

        /// <summary>
        /// Prompts the user to avoid accidental form cancellation. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to cancel?",
                "Cancel New Task",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {
                _taskFactory.StartNew(() => ClearForm());
                BuildEditableForm();
            }

            btnEdit.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Handles a remove action by prompting the user to prevent accidental removal of a task.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            // Sanity check and remove
            if (_selectedTask != null)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Are you sure you want to remove task \"" + _selectedTask.TaskName + "\"?",
                    "Confirm Remove Task",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning,
                    MessageBoxResult.No);

                if (result == MessageBoxResult.Yes)
                {
                    // Delete and clear the form.
                    _vmTasks.DeleteTask(ref _selectedTask);
                    _taskFactory.StartNew(new Action(() => ClearForm()));
                }

                _selectedTask = null;
                btnRemove.IsEnabled = false;
            }
        }

        /// <summary>
        /// Handles a node losing focus by resetting the selected task.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBlock_LostFocus(object sender, RoutedEventArgs e)
        {
            _selectedTask = null;
            btnRemove.IsEnabled = false;
        }

        #endregion

        /// <summary>
        /// Handles the WindowLoaded event by creating the application's view model and setting
        /// some properties.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _vmTasks = new AppViewModel();

            _forLabels = new List<Label>()
            {
                new Label() { Content = "Name: " },
                new Label() { Content = "Description: " },
                new Label() { Content = "Date Started: " },
                new Label() { Content = "Date Due: " },
                new Label() { Content = "Status: " },
                new Label() { Content = "Priority: " },
                new Label() { Content = "Details: " },
            };

            // Build up the task lists from the view model's `TaskMap`, one for each priority
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

            BuildForLabels();
        }

        /// <summary>
        /// Handles the WindowClosing event by persisting the tasks to disk.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _vmTasks.DumpToDisk();
        }
    }
}
