using Graph_search.Components;
using Graph_search.Model;
using Graph_search.View.Components;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

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
            // Register the Edge toggle state Event Handler (triggered when the user clicks the button "Add edge")
            AddHandler(LeftMenuComponent.ToggleEdgeStateEvent,
                new RoutedEventHandler(UserControl_ToggleEdgeStateEventHandler));
            // Register the Edge added Event Handler
            AddHandler(GraphNodeComponent.AddEdgeEvent,
                new RoutedEventHandler(UserControl_AddEdgeEventHandler));
            // Register the Node added Event Handler
            AddHandler(LeftMenuComponent.AddNodeEvent,
                new RoutedEventHandler(UserControl_AddNodeEventHandler));
        }

        /// <summary>
        /// Adds a node with the given name to the canvas
        /// </summary>
        /// <param name="name">The node's name</param>
        internal bool AddNode(string name)
        {
            // Check that the name is unique
            foreach (GraphNode n in nodes)
                if (n.name == name)
                {
                    MessageBox.Show($"Name {name} was already taken");
                    return false;
                }
            GraphNode node = new GraphNode(name);
            nodes.Add(node);
            GraphNodeComponent nodeElem = new GraphNodeComponent(node, addEdgeState);
            Canvas.SetLeft(nodeElem, 0);
            Canvas.SetTop (nodeElem, 0);
            RootCanvas.Children.Add(nodeElem);
            return true;
        }
        /// <summary>
        /// Links 2 nodes together with an edge
        /// </summary>
        /// <param name="from">Starting node</param>
        /// <param name="to">Ending node</param>
        private bool AddEdge(GraphNodeComponent from, GraphNodeComponent to)
        {
            // Check if edge already exists
            foreach (GraphEdgeComponent e in from.graphEdges)
                if (e.edge.Opposite(from.node) == to.node)
                {
                    MessageBox.Show("This edge already exists");
                    return false;
                }
            // Create edge
            GraphEdgeComponent gec = new GraphEdgeComponent(from, to);
            from.AddEdge(gec); to.AddEdge(gec);
            from.node.AddLink(gec.edge);
            // Insert in canvas
            Canvas.SetLeft(gec, Math.Min(gec.from.X, gec.to.X));
            Canvas.SetTop (gec, Math.Min(gec.from.Y, gec.to.Y));
            RootCanvas.Children.Add(gec);
            // Send event to update button on the left
            leftMenuComponent.RaiseEvent(new RoutedEventArgs(LeftMenuComponent.ToggledEdgeStateEvent, false));
            return true;
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
        /// Sets the current addEdgeState to all the nodes in the canvas
        /// </summary>
        private void SetAddEdgeStates()
        {
            foreach (UIElement child in RootCanvas.Children)
                if (child is GraphNodeComponent c)
                    c.addEdgeState = addEdgeState;
        }
        private void UserControl_ToggleEdgeStateEventHandler(object sender, RoutedEventArgs e)
        {
            bool addEdge = (bool)e.OriginalSource;
            if (addEdge)
            {
                addEdgeState = AddEdgeState.FirstVertex;
                ShowCurrentAction("Adding edge: select the first node");
            }
            else
            {
                addEdgeState = AddEdgeState.None;
                firstVertex = null;
                HideCurrentAction();
            }
            SetAddEdgeStates();
        }
        /// <summary>
        /// Event that get raised when a children is selected as vertex for an edge connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_AddEdgeEventHandler(object sender, RoutedEventArgs e)
        {
            if (addEdgeState == AddEdgeState.None) return;
            GraphNodeComponent? vertex = e.OriginalSource as GraphNodeComponent;
            if (vertex != null)
            {
                switch (addEdgeState)
                {
                    case AddEdgeState.FirstVertex:
                        firstVertex = vertex;
                        addEdgeState = AddEdgeState.SecondVertex;
                        ShowCurrentAction($"Adding edge: selected \"{firstVertex.node.name}\", now select the second node");
                        break;
                    case AddEdgeState.SecondVertex:
                        if (firstVertex == vertex)
                            MessageBox.Show("You must select a different node");
                        else if (AddEdge(firstVertex, vertex)) // Adds the edge, if possible
                        {
                            addEdgeState = AddEdgeState.None;
                            SetAddEdgeStates();
                            HideCurrentAction();
                        }
                        break;
                }
            }
            e.Handled = true;
        }
        private void UserControl_AddNodeEventHandler(object sender, RoutedEventArgs e)
        {
            string name = (string)e.OriginalSource;
            AddNode(name);
        }

        private void ShowCurrentAction(string action)
        {
            CommunicationBox.Text = action;
            CommunicationBox.Width = RootCanvas.ActualWidth;
            Canvas.SetTop(CommunicationBox, 0);
        }
        private void HideCurrentAction()
        {
            Canvas.SetTop(CommunicationBox, -100);
        }
    }
}
