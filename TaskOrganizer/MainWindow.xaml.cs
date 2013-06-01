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
        ViewModel _vmTasks;

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
    }
}
