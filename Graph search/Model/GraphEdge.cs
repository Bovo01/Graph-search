using System;

namespace Graph_search.Model
{
    /// <summary>
    /// Graph edge: links 2 nodes with a weight
    /// </summary>
    internal class GraphEdge
    {
        internal readonly GraphNode from, to;
        internal readonly double weight;

        internal GraphEdge(GraphNode from, GraphNode to, double weight)
        {
            this.from = from;
            this.to = to;
            this.weight = weight;
        }

        internal GraphNode Opposite(GraphNode node)
        {
            if (node == from)
                return to;
            if (node == to)
                return from;
            throw new ArgumentException("invalid node");
        }
    }
}
