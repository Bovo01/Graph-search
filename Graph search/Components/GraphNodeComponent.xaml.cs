using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Graph_search.Components
{
    public partial class GraphNodeComponent : UserControl
    {
        public GraphNodeComponent(string name)
        {
            InitializeComponent();
            this.DataContext = this;
            this.NodeName = name;
        }

        public string NodeName { get; private set; }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed)
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

        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            base.OnGiveFeedback(e);
            Mouse.SetCursor(Cursors.Hand);
            e.Handled = true;
        }
    }
}
