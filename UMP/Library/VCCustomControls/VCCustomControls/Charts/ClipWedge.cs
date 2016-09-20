using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VoiceCyber.Wpf.CustomControls.Charts
{
    /// <summary>
    /// 
    /// </summary>
    public class ClipWedge : ContentControl
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static ClipWedge()
        {
            r = new Random();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void OnWedgeShapeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ClipWedge c = sender as ClipWedge;
            if (c != null)
                c.InvalidateArrange();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arrangeBounds"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            Clip = GetClipGeometry(arrangeBounds);
            return base.ArrangeOverride(arrangeBounds);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arrangeBounds"></param>
        /// <returns></returns>
        public StreamGeometry GetClipGeometry(Size arrangeBounds)
        {
            StreamGeometry clip = new StreamGeometry();
            StreamGeometryContext clipGC = clip.Open();
            clipGC.BeginFigure(BeginFigurePoint, true, true);
            clipGC.LineTo(LineToPoint, false, true);
            Vector v = LineToPoint - BeginFigurePoint;
            RotateTransform rt = new RotateTransform(WedgeAngle, BeginFigurePoint.X, BeginFigurePoint.Y);
            bool isLargeArc = WedgeAngle > 180.0;
            clipGC.ArcTo(rt.Transform(LineToPoint), new Size(v.Length, v.Length), WedgeAngle, isLargeArc, SweepDirection.Clockwise, false, true);
            clipGC.Close();
            return clip;
        }
        /// <summary>
        /// 
        /// </summary>
        public double WedgeAngle
        {
            get { return (double)GetValue(WedgeAngleProperty); }
            set { SetValue(WedgeAngleProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for WedgeAngle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WedgeAngleProperty =
            DependencyProperty.Register("WedgeAngle", typeof(double), typeof(ClipWedge), new UIPropertyMetadata(0.0, OnWedgeShapeChanged));

        /// <summary>
        /// 
        /// </summary>
        public Point BeginFigurePoint
        {
            get { return (Point)GetValue(BeginFigurePointProperty); }
            set { SetValue(BeginFigurePointProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for BeginFigurePoint.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BeginFigurePointProperty =
            DependencyProperty.Register("BeginFigurePoint", typeof(Point), typeof(ClipWedge), new UIPropertyMetadata(new Point(), OnWedgeShapeChanged));

        /// <summary>
        /// 
        /// </summary>
        public Point LineToPoint
        {
            get { return (Point)GetValue(LineToPointProperty); }
            set { SetValue(LineToPointProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LineTo.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LineToPointProperty =
            DependencyProperty.Register("LineToPoint", typeof(Point), typeof(ClipWedge), new UIPropertyMetadata(new Point(), OnWedgeShapeChanged));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CenterPointProperty =
            DependencyProperty.Register("CenterPoint", typeof(Point), typeof(ClipWedge), new PropertyMetadata(default(Point)));
        /// <summary>
        /// 
        /// </summary>
        public Point CenterPoint
        {
            get { return (Point)GetValue(CenterPointProperty); }
            set { SetValue(CenterPointProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TranslateXProperty =
            DependencyProperty.Register("TranslateX", typeof(double), typeof(ClipWedge), new PropertyMetadata(default(double)));
        /// <summary>
        /// 
        /// </summary>
        public double TranslateX
        {
            get { return (double)GetValue(TranslateXProperty); }
            set { SetValue(TranslateXProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TranslateYProperty =
            DependencyProperty.Register("TranslateY", typeof(double), typeof(ClipWedge), new PropertyMetadata(default(double)));
        /// <summary>
        /// 
        /// </summary>
        public double TranslateY
        {
            get { return (double)GetValue(TranslateYProperty); }
            set { SetValue(TranslateYProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ProportionProperty =
            DependencyProperty.Register("Proportion", typeof(double), typeof(ClipWedge), new PropertyMetadata(default(double)));
        /// <summary>
        /// 
        /// </summary>
        public double Proportion
        {
            get { return (double)GetValue(ProportionProperty); }
            set { SetValue(ProportionProperty, value); }
        }

        private static Random r;
    }
}
