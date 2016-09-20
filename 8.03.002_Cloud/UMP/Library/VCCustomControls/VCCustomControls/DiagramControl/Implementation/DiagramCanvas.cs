//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0385172e-f983-4a43-b1b4-be6a9917693c
//        CLR Version:              4.0.30319.18444
//        Name:                     DiagramCanvas
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.DiagramControl.Implementation
//        File Name:                DiagramCanvas
//
//        created by Charley at 2014/9/10 16:25:50
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VoiceCyber.Wpf.CustomControls
{
    /// <summary>
    /// 表示一个面板，其子元素与用直线连接起来
    /// </summary>
    public class DiagramCanvas : Canvas
    {
        #region Layout
        /// <summary>
        /// Any custom Panel must override ArrangeOverride and MeasureOverride
        /// </summary>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            foreach (UIElement element in InternalChildren)
            {
                double x;
                double y;
                double left = GetLeft(element);
                double top = GetTop(element);
                x = double.IsNaN(left) ? 0 : left;
                y = double.IsNaN(top) ? 0 : top;

                element.Arrange(new Rect(new Point(x, y), element.DesiredSize));
            }
            return arrangeSize;
        }


        protected override Size MeasureOverride(Size constraint)
        {
            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (UIElement element in InternalChildren)
            {
                element.Measure(size);
            }
            return new Size();
        }


        #endregion

        #region Render Methods
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            foreach (UIElement uiElement in Children)
            {
                if (uiElement is DiagramNode)
                {
                    DiagramNode node = (DiagramNode)uiElement;

                    if (node.Visibility == Visibility.Visible)
                    {
                        if (node.DiagramParent != null &&
                            node.DiagramParent.Visibility == Visibility.Visible)
                        {
                            dc.DrawLine(new Pen(Brushes.Black, 2.0),
                                node.Location, node.DiagramParent.Location);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
