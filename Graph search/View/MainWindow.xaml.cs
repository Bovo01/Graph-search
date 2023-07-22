using Graph_search.Components;
using Graph_search.Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Graph_search
{
    /// <summary>
    /// Indicates the state of an adding edge (None when it's not being added, FirstVertex and SecondVertex when the user is adding that vertex)
    /// </summary>
    public enum AddEdgeState { None, FirstVertex, SecondVertex }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AddEdgeState addEdgeState = AddEdgeState.None;
        private GraphNodeComponent? firstVertex = null;
        private readonly List<GraphNode> nodes;

        public MainWindow()
        {
            InitializeComponent();
            nodes = new List<GraphNode>();
            // Register the Bubble Event Handler
            AddHandler(GraphNodeComponent.AddEdgeEvent,
                new RoutedEventHandler(UserControl_AddEdgeEventHandler));
        }

        /// <summary>
        /// Adds a node with the given name to the canvas
        /// </summary>
        /// <param name="name">The node's name</param>
        private void AddNode(string name)
        {
            GraphNode node = new GraphNode(name);
            nodes.Add(node);
            GraphNodeComponent nodeElem = new GraphNodeComponent(node, addEdgeState);
            Canvas.SetLeft(nodeElem, 0);
            Canvas.SetTop (nodeElem, 0);
            RootCanvas.Children.Add(nodeElem);
        }
        /// <summary>
        /// Links 2 nodes together with an edge
        /// </summary>
        /// <param name="from">Starting node</param>
        /// <param name="to">Ending node</param>
        private void AddEdge(GraphNodeComponent from, GraphNodeComponent to)
        {
            // Create edge
            GraphEdgeComponent gec = new GraphEdgeComponent(from, to);
            from.AddEdge(gec); to.AddEdge(gec);
            from.node.AddLink(gec.edge);
            // Insert in canvas
            Canvas.SetLeft(gec, Math.Min(gec.from.X, gec.to.X));
            Canvas.SetTop (gec, Math.Min(gec.from.Y, gec.to.Y));
            RootCanvas.Children.Add(gec);
        }
        /// <summary>
        /// Animation for moving a node
        /// </summary>
        private void NodeDragOver(object sender, DragEventArgs e)
        {
            object draggedObj = e.Data.GetData(DataFormats.Serializable);

            if (draggedObj is GraphNodeComponent dragged)
            {
                Point dropPosition = e.GetPosition(RootCanvas);

                Point mouseOffset = (Point)e.Data.GetData("mouseOffset");
                Point newPosition = GetAdjustedPosition(dropPosition, mouseOffset, dragged);

                Canvas.SetLeft(dragged, newPosition.X);
                Canvas.SetTop (dragged, newPosition.Y);

                dragged.UpdateEdges();
            }
        }
        /// <summary>
        /// Returns the adjusted position of the dragging operation. It's adjusted based on the current mouse position, the initial offset and the size of the element moved.
        /// </summary>
        /// <param name="currMousePosition">The current mouse position, or the position where the element was "dropped".</param>
        /// <param name="mouseOffset">The initial offset of the mouse inside the element. Must be 0 <= x <= element.width and same for y.</param>
        /// <param name="element">The element the user is trying to move (used for the size).</param>
        /// <returns>A point such that all the node is still inside the canvas (and in the position it was moved from the user).</returns>
        private Point GetAdjustedPosition(Point currMousePosition, Point mouseOffset, GraphNodeComponent element)
        {
            double newX = currMousePosition.X - mouseOffset.X,
                   newY = currMousePosition.Y - mouseOffset.Y;

            if (newX < 0) newX = 0;
            if (newY < 0) newY = 0;
            if (newX > RootCanvas.ActualWidth  - element.ActualWidth ) newX = RootCanvas.ActualWidth  - element.ActualWidth ;
            if (newY > RootCanvas.ActualHeight - element.ActualHeight) newY = RootCanvas.ActualHeight - element.ActualHeight;

            return new Point(newX, newY);
        }
        /// <summary>
        /// Function event that triggers when the user tries to add a new node to the graph
        /// </summary>
        private void AddNodeClick(object sender, RoutedEventArgs e)
        {
            if (NameTextBox.Text != "")
            {
                AddNode(NameTextBox.Text);
                NameTextBox.Text = "";
            }
            else
                MessageBox.Show("Insert the node name");
            e.Handled = true;
        }
        /// <summary>
        /// Function event that triggers when the user tries to add a new node to the graph (pressing the enter key while typing)
        /// </summary>
        private void AddNodeEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                AddNodeClick(NameTextBox.Text, e);
                NameTextBox.Text = "";
            }
        }
        /// <summary>
        /// Prepares all the nodes to add a new edge. If waiting for the action, cancels it completely
        /// </summary>
        private void AddEdgeClick(object sender, RoutedEventArgs e)
        {
            if (addEdgeState == AddEdgeState.None)
                addEdgeState = AddEdgeState.FirstVertex;
            else
            {
                addEdgeState = AddEdgeState.None;
                firstVertex = null;
            }
            SetAddEdgeStates();
        }
        /// <summary>
        /// Sets the current addEdgeState to all the nodes in the canvas
        /// </summary>
        private void SetAddEdgeStates()
        {
            foreach (UIElement child in RootCanvas.Children)
                if (child is GraphNodeComponent c)
                    c.addEdgeState = addEdgeState;
        }
        /// <summary>
        /// Event that get raised when a children is selected as vertex for an edge connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_AddEdgeEventHandler(object sender, RoutedEventArgs e)
        {
            if (addEdgeState == AddEdgeState.None) return;
            object? sourceNode = e.OriginalSource as GraphNodeComponent;
            if (sourceNode != null)
            {
                GraphNodeComponent vertex = (GraphNodeComponent)sourceNode;
                switch (addEdgeState)
                {
                    case AddEdgeState.FirstVertex:
                        firstVertex = vertex;
                        addEdgeState = AddEdgeState.SecondVertex;
                        break;
                    case AddEdgeState.SecondVertex:
                        if (firstVertex == vertex)
                            MessageBox.Show("You must select a different node");
                        else
                        {
                            AddEdge(firstVertex, vertex);
                            addEdgeState = AddEdgeState.None;
                            SetAddEdgeStates();
                        }
                        break;
                }
            }
            e.Handled = true;
        }
    }
}
