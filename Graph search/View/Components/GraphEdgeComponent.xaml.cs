using Graph_search.Model;
using Graph_search.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Graph_search.Components
{
    /// <summary>
    /// Logica di interazione per GraphEdgeComponent.xaml
    /// </summary>
    public partial class GraphEdgeComponent : UserControl
    {
        /* xaml data */
        private readonly GraphEdgeData edgeData;
        public GraphEdgeComponent(GraphNodeComponent from, GraphNodeComponent to)
        {
            InitializeComponent();
            edgeData = new GraphEdgeData(from, to);
            DataContext = edgeData;
            edge = new GraphEdge(from.node, to.node, edgeData.weight);
        }

        internal readonly GraphEdge edge;
        internal Point from { get { return edgeData.from; } }
        internal Point to { get { return edgeData.to; } }
        internal double weight { get { return edgeData.weight; } }

        internal void SetFrom(GraphNodeComponent from)
        {
            edgeData.SetFrom(from);
        }
        internal void SetTo(GraphNodeComponent to)
        {
            edgeData.SetTo(to);
        }
    }
}
