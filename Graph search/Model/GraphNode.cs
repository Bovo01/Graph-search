using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph_search
{
    /// <summary>
    /// Weighted graph
    /// </summary>
    internal class Graph
    {
        internal readonly List<GraphNode> nodes;

        internal Graph()
        {
            nodes = new List<GraphNode>();
        }

        internal GraphNode AddNode(string name)
        {
            GraphNode node = new GraphNode(name);
            nodes.Add(node);
            return node;
        }
        internal GraphEdge SetEdge(GraphNode node1, GraphNode node2, int weight)
        {
            GraphEdge edge = new GraphEdge(node1, node2, weight);
            node1.AddLink(edge);
            node2.AddLink(edge);
            return edge;
        }
    }

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
            links.Add(link);
        }
    }

    /// <summary>
    /// Graph edge: links 2 nodes with a weight
    /// </summary>
    internal class GraphEdge
    {
        private readonly GraphNode node1, node2;
        internal readonly int weight;

        internal GraphEdge(GraphNode node1, GraphNode node2, int weight)
        {
            this.node1 = node1;
            this.node2 = node2;
            this.weight = weight;
        }

        internal GraphNode Opposite(GraphNode node)
        {
            if (node == node1)
                return node2;
            if (node == node2)
                return node1;
            throw new ArgumentException("invalid node");
        }
    }
}
