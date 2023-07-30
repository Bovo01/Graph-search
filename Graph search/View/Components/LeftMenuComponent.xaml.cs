using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Graph_search.View.Components
{
    /// <summary>
    /// Logica di interazione per LeftMenuComponent.xaml
    /// </summary>
    public partial class LeftMenuComponent : UserControl
    {
        // Register the add node event, or the event that will be emitted to the parent in order to add a new node
        public static readonly RoutedEvent AddNodeEvent =
            EventManager.RegisterRoutedEvent("NodeAdded", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(LeftMenuComponent));
        // Register the toggle edge state event, or the event that will be emitted to the parent in order to switch between adding the edge and not adding it
        public static readonly RoutedEvent ToggleEdgeStateEvent =
            EventManager.RegisterRoutedEvent("ToggleEdgeState", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(LeftMenuComponent));
        // Register the toggled edge state event, or the event that will be emitted by the parent in order to communicate when an edge has been added
        public static readonly RoutedEvent ToggledEdgeStateEvent =
            EventManager.RegisterRoutedEvent("ToggledEdgeState", RoutingStrategy.Tunnel,
            typeof(RoutedEventHandler), typeof(LeftMenuComponent));


        private bool isAddingEdge;

        public LeftMenuComponent()
        {
            InitializeComponent();
            isAddingEdge = false;
            AddHandler(ToggledEdgeStateEvent,
                new RoutedEventHandler(Parent_ToggledEdgeStateEventHandler));
        }
        /// <summary>
        /// Function event that triggers when the user tries to add a new node to the graph
        /// </summary>
        private void AddNodeClick(object sender, RoutedEventArgs e)
        {
            if (NameTextBox.Text != "")
            {
                RaiseEvent(new RoutedEventArgs(AddNodeEvent, NameTextBox.Text));
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
            isAddingEdge = !isAddingEdge;
            SetAddEdgeButtonText();
            RaiseEvent(new RoutedEventArgs(ToggleEdgeStateEvent, isAddingEdge));
        }
        private void SetAddEdgeButtonText()
        {
            if (isAddingEdge)
                AddEdgeButton.Content = "Cancel edge insertion";
            else
                AddEdgeButton.Content = "Add edge";
        }

        private void Parent_ToggledEdgeStateEventHandler(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("ToggledEdge");
            bool addingEdge = (bool)e.OriginalSource;
            isAddingEdge = addingEdge;
            SetAddEdgeButtonText();
            e.Handled = true;
        }
    }
}
