using System.Collections.Generic;

namespace Graph_search.Model
{
    /// <summary>
    /// Graph node: contains name and edges
    /// </summary>
    internal class GraphNode
    {
        internal readonly string name;
        internal readonly List<GraphEdge> links;

        internal GraphNode(string name)
        {
            this.name = name;
            links = new List<GraphEdge>();
        }

        internal void AddLink(GraphEdge link)
        {
            // Add the link to both nodes
            links.Add(link);
            link.Opposite(this).links.Add(link);
        }
    }
}
