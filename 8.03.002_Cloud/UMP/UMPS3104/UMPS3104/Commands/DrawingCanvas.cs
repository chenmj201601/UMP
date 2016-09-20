using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UMPS3104.Commands
{
    public class DrawingCanvas : Canvas
    {
        private List<Visual> visuals = new List<Visual>();

        protected override int VisualChildrenCount
        {
            get { return visuals.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            return visuals[index];
        }

        public void AddVisual(Visual visual)
        {
            visuals.Add(visual);

            AddVisualChild(visual);
            AddLogicalChild(visual);
        }

        public void DeleteVisual(Visual visual)
        {
            visuals.Remove(visual);

            RemoveVisualChild(visual);
            RemoveLogicalChild(visual);
        }

        public DrawingVisual GetVisual(Point point)
        {
            HitTestResult result = VisualTreeHelper.HitTest(this, point);
            return result.VisualHit as DrawingVisual;
        }
    }
}
