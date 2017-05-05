//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    895d1cda-afb4-4f11-9380-86f45902a11e
//        CLR Version:              4.0.30319.18444
//        Name:                     DrawingCanvas
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Codes
//        File Name:                DrawingCanvas
//
//        created by Charley at 2014/12/9 17:52:31
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UMPS3102.Codes
{
    public class DrawingCanvas:Canvas
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
