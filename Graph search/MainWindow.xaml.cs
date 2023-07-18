using Graph_search.Components;
using System.Windows;
using System.Windows.Controls;

namespace Graph_search
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AddNode("Pluto");
        }

        private void AddNode(string name)
        {
            GraphNodeComponent node = new GraphNodeComponent(name);
            RootCanvas.Children.Add(node);
        }

        private void NodeDragOver(object sender, DragEventArgs e)
        {
            object draggedObj = e.Data.GetData(DataFormats.Serializable);

            if (draggedObj is GraphNodeComponent dragged)
            {
                Point dropPosition = e.GetPosition(RootCanvas);

                Point relativeObjPos = (Point)e.Data.GetData("mouseOffset");
                Point newPosition = GetNewPosition(dropPosition, relativeObjPos, dragged);

                Canvas.SetLeft(dragged, newPosition.X);
                Canvas.SetTop (dragged, newPosition.Y);
            }
        }

        private Point GetNewPosition(Point newPosition, Point mouseOffset, GraphNodeComponent element)
        {
            double newX, newY;
            (newX, newY) = (newPosition.X - mouseOffset.X, newPosition.Y - mouseOffset.Y);

            if (newX < 0) newX = 0;
            if (newY < 0) newY = 0;
            if (newX > RootCanvas.ActualWidth  - element.ActualWidth ) newX = RootCanvas.ActualWidth  - element.ActualWidth ;
            if (newY > RootCanvas.ActualHeight - element.ActualHeight) newY = RootCanvas.ActualHeight - element.ActualHeight;

            return new Point(newX, newY);
        }

        private void AddNodeClick(object sender, RoutedEventArgs e)
        {
            if (NameTextBox.Text != "")
            {
                AddNode(NameTextBox.Text);
                NameTextBox.Text = "";
            }
            else
                MessageBox.Show("Insert the node name");
        }
    }
}
