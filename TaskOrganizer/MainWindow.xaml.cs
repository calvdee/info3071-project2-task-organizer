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
        private Dictionary<TaskPriority, List<Classes.Task>> _tree;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _vmTasks = new ViewModel();
            _vmTasks.TaskCollection.CollectionChanged += TaskCollection_CollectionChanged;
        }

        /// <summary>
        /// Handles the addition of new tasks by re-building the tree view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TaskCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            treeTasks.Items.Clear();

            // Create a new parent node for each task priority in the TreeCollection.
            foreach(KeyValuePair<Classes.TaskPriority, List<Classes.Task>> kvp in _vmTasks.TreeCollection)
            {
                TreeViewItem ndeParent = new TreeViewItem();
                ndeParent.Header = Enum.GetName(typeof(Classes.TaskPriority), kvp.Key);
                
                // Create a new child node for each task in the list and set the context.
                foreach (Classes.Task task in kvp.Value)
                {
                    TreeViewItem ndeName = new TreeViewItem();
                    TreeViewItem ndeStarted = new TreeViewItem();
                    TreeViewItem ndeDue = new TreeViewItem();
                    TreeViewItem ndeStatus = new TreeViewItem();
                    TreeViewItem ndeSubParent = new TreeViewItem();

                    // Create the sub parent node and hookup the click handler
                    ndeSubParent.Header = task;
                    ndeSubParent.MouseUp += ndeSubParent_MouseUp;

                    // Create the metadata nodes
                    ndeStarted.Header = "Started: " + task.DateStarted.ToString(Classes.Task.DateFormat);
                    ndeDue.Header = "Due: " + task.DueDate.ToString(Classes.Task.DateFormat);
                    ndeStatus.Header = "Status: " + Enum.GetName(typeof(Classes.TaskStatus), task.Status);

                    // Add metadata child nodes to the sub parent
                    ndeSubParent.Items.Add(ndeName);
                    ndeSubParent.Items.Add(ndeStarted);
                    ndeSubParent.Items.Add(ndeDue);
                    ndeSubParent.Items.Add(ndeStatus);

                    // Add the sub parent to the parent
                    ndeParent.Items.Add(ndeSubParent);
                }

                // Add to the tree view
                treeTasks.Items.Add(ndeParent);
            }
        }

        void ndeSubParent_MouseUp(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
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
        /// Handles the save by creating a new task object will cause view model events to fire and
        /// the new task will be renedered in the tree view and persisted to the disk.
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
