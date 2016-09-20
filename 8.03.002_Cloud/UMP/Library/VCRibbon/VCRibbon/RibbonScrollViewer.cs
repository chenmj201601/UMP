//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    921efb9c-57c1-408d-9e2d-14e132e7d11d
//        CLR Version:              4.0.30319.18444
//        Name:                     RibbonScrollViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Ribbon
//        File Name:                RibbonScrollViewer
//
//        created by Charley at 2014/5/27 22:44:22
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace VoiceCyber.Ribbon
{
    /// <summary>
    /// Represents ScrollViewer with modified hit test
    /// </summary>
    public class RibbonScrollViewer : ScrollViewer
    {
        #region Overrides

        /// <summary>
        /// Performs a hit test to determine whether the specified 
        /// points are within the bounds of this ScrollViewer
        /// </summary>
        /// <returns>The result of the hit test</returns>
        /// <param name="hitTestParameters">The parameters for hit testing within a visual object</param>
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            if (VisualChildrenCount > 0)
            {
                return VisualTreeHelper.HitTest(GetVisualChild(0), hitTestParameters.HitPoint);
            }
            return base.HitTestCore(hitTestParameters);
        }

        #endregion
    }
}
