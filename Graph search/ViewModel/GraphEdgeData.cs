using Graph_search.Components;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Graph_search.ViewModel
{
    public class GraphEdgeData : ObservableObject
    {
        /* Editable variables */
        private Point _fromPos, _toPos;
        internal Point fromPos
        {
            get { return _fromPos; }
            set { _fromPos = value; AllPropertiesChanged(); }
        }
        internal Point toPos
        {
            get { return _toPos; }
            set { _toPos = value; AllPropertiesChanged(); }
        }
        /* Computed variables */
        internal double weight { get { return Point.Subtract(fromPos, toPos).Length; } }
        /* xaml variables */
        public Point from { get { return fromPos.X < toPos.X ? fromPos : toPos; } }
        public Point to { get { return fromPos.X < toPos.X ? toPos : fromPos; } }
        public double width { get { return Math.Abs(fromPos.X - toPos.X); } }
        public double height { get { return Math.Abs(fromPos.Y - toPos.Y); } }
        public int approxWeight { get { return (int)Math.Round(weight, 0); } }

        public GraphEdgeData(GraphNodeComponent from, GraphNodeComponent to)
        {
            fromPos = GetCenterPosition(from);
            toPos = GetCenterPosition(to);
        }

        private static Point GetCenterPosition(GraphNodeComponent e)
        {
            return new Point((double)e.GetValue(Canvas.LeftProperty) + e.ActualWidth / 2.0, (double)e.GetValue(Canvas.TopProperty) + e.ActualHeight / 2.0);
        }
        /// <summary>
        /// Updates all the properties to the view
        /// </summary>
        private void AllPropertiesChanged()
        {
            OnPropertyChanged("from");
            OnPropertyChanged("to");
            OnPropertyChanged("width");
            OnPropertyChanged("height");
            OnPropertyChanged("approxWeight");
        }

        internal void SetFrom(GraphNodeComponent from)
        {
            fromPos = GetCenterPosition(from);
        }
        internal void SetTo(GraphNodeComponent to)
        {
            toPos = GetCenterPosition(to);
        }
    }
}
