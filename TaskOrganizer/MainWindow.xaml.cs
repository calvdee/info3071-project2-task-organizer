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
        private TaskFactory _taskFactory = new TaskFactory();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //TaskForm form = TaskForm.CreateForRead(new Classes.Task()
            //{
            //    TaskName = "TestTask",
            //    Description = "Mydesc",
            //    DateStarted = DateTime.Today,
            //    DueDate = DateTime.Today,
            //    Details = "Some details blah blah",
            //    Priority = TaskPriority.HIGH,
            //    Status = Classes.TaskStatus.CREATED
            //});

            //form.DetailsLabel.SetValue(Grid.ColumnSpanProperty, 2);

            //form.LabelCollection.ForEach(l => gridTaskDetail.Children.Add(l));
            _vmTasks = new ViewModel();           
        }

        /// <summary>
        /// Asynchronously creates a new EditableTaskForm.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            _taskFactory.StartNew(() => ClearReadOnlyForm());

            EditableTaskForm form = EditableTaskForm.CreateNew();
            form.Status.ItemsSource = _vmTasks.TaskStatuses;
            form.Priority.ItemsSource = _vmTasks.TaskPriorities;
            form.Details.SetValue(Grid.ColumnSpanProperty, 2);

            _taskFactory.StartNew(() =>
            {
                AddToDetailForm(form.DateDue);
                AddToDetailForm(form.DateStarted);
                AddToDetailForm(form.Description);
                AddToDetailForm(form.Details);
                AddToDetailForm(form.Name);
                AddToDetailForm(form.Priority);
                AddToDetailForm(form.Status);

                Dispatcher.BeginInvoke(new Action(() => btnSave.Visibility = System.Windows.Visibility.Visible));
            })
            .ContinueWith((result) => 
            {
                Dispatcher.BeginInvoke(new Action(() => SetTabIndexes(form)));
                Dispatcher.BeginInvoke(new Action(() => form.Name.Focus()));
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
                    Dispatcher.BeginInvoke(new Action(() => child = gridTaskDetail.FindName(label.Name) as UIElement));
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
    }
}
