using Graph_search.Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Graph_search.Components
{
    public partial class GraphNodeComponent : UserControl
    {
        // Register the add edge event, or the event that will be emitted to the parent to understand which child was selected
        public static readonly RoutedEvent AddEdgeEvent =
            EventManager.RegisterRoutedEvent("EdgeAdded", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(GraphNodeComponent));

        internal GraphNode node { get; private set; }
        internal AddEdgeState addEdgeState;
        internal readonly List<GraphEdgeComponent> graphEdges;

        /* xaml attributes */
        public string nodeName { get { return node.name; } }

        internal GraphNodeComponent(GraphNode node, AddEdgeState addEdgeState)
        {
            InitializeComponent();
            this.node = node;
            this.DataContext = this;
            this.addEdgeState = addEdgeState;
            this.graphEdges = new List<GraphEdgeComponent>();
        }

        /// <summary>
        /// Initializes the drag and drop operation
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed && addEdgeState == AddEdgeState.None)
            {
                DataObject data = new DataObject();
                data.SetData(DataFormats.Serializable, this);
                Point mouseOffset = Mouse.GetPosition(this);
                data.SetData("mouseOffset", mouseOffset);

                // Initiate the drag-and-drop operation.
                DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
            }
            e.Handled = true;
        }
        /// <summary>
        /// Gives feedback (on move) changing the cursor
        /// </summary>
        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            base.OnGiveFeedback(e);
            if (e.Effects.HasFlag(DragDropEffects.Move))
                Mouse.SetCursor(Cursors.Hand);
            else
                Mouse.SetCursor(Cursors.No);
            e.Handled = true;
        }
        /// <summary>
        /// Communicates the parent that will catch this event about the selection of this element
        /// </summary>
        private void RaiseAddEdgeEvent(object sender, MouseButtonEventArgs e)
        {
            if (addEdgeState != AddEdgeState.None)
            {
                // Raise the custom routed event, this fires the event from the UserControl
                RaiseEvent(new RoutedEventArgs(AddEdgeEvent, this));
            }
            e.Handled = true;
        }
        /// <summary>
        /// Registers and edge on this component (in order to update the UI)
        /// </summary>
        /// <param name="edge">The edge to be added</param>
        /// <param name="dir">The direction of this node ("From" or "To")</param>
        internal void AddEdge(GraphEdgeComponent edge)
        {
            graphEdges.Add(edge);
        }
        /// <summary>
        /// Updates the positions of every edge linked with this node
        /// </summary>
        internal void UpdateEdges()
        {
            foreach (GraphEdgeComponent gec in graphEdges)
            {
                if (gec.edge.from == node)
                    gec.SetFrom(this);
                else
                    gec.SetTo(this);
                Canvas.SetLeft(gec, Math.Min(gec.from.X, gec.to.X));
                Canvas.SetTop(gec, Math.Min(gec.from.Y, gec.to.Y));
            }
        }
    }
}
