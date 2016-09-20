//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    39bb26c1-185b-428d-8e83-1d5277f5796d
//        CLR Version:              4.0.30319.18444
//        Name:                     DiagramPanel
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.TreeViews.Implementation
//        File Name:                DiagramPanel
//
//        created by Charley at 2014/9/15 14:20:04
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VoiceCyber.Wpf.CustomControls
{
    public class DiagramPanel : StackPanel
    {
        static DiagramPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramPanel),
                new FrameworkPropertyMetadata(typeof(DiagramPanel)));
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            double x = 0.0;
            for (int i = 0; i < InternalChildren.Count; i++)
            {
                FrameworkElement child = InternalChildren[i] as FrameworkElement;
                if (child != null)
                {
                    Point ponit1 = new Point();
                    ponit1.X = child.ActualWidth / 2.0 + x;
                    ponit1.Y = 50;
                    Point point3 = new Point();
                    point3.X = ponit1.X;
                    point3.Y = 5.0;
                    Point point2 = new Point();
                    point2.X = ActualWidth / 2.0;
                    point2.Y = -15;
                    Point point4 = new Point();
                    point4.X = point2.X;
                    point4.Y = 5.0;
                    x += child.ActualWidth;
                    dc.DrawLine(new Pen(Brushes.Gray, 2.0), ponit1, point3);
                    dc.DrawLine(new Pen(Brushes.Gray, 2.0), point3, point4);
                    dc.DrawLine(new Pen(Brushes.Gray, 2.0), point4, point2);
                }
            }
        }
    }
}
