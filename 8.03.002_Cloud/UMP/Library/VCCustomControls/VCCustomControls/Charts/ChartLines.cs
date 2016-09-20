using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media;

namespace VoiceCyber.Wpf.CustomControls.Charts
{
    /// <summary>
    /// 
    /// </summary>
    public class ChartLines : FrameworkElement
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="drawingContext"></param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (drawingContext == null)
            {
                throw new ArgumentNullException("drawingContext");
            }

            base.OnRender(drawingContext);
            if (VerticalAxisTickPositions != null)
            {
                if (DrawVerticalAxisReferenceLines)
                {
                    for (int i = 0; i < VerticalAxisTickPositions.Count; i++)
                        drawingContext.DrawLine(ReferenceLinePen, new Point(0, VerticalAxisTickPositions[i]), new Point(RenderSize.Width, VerticalAxisTickPositions[i]));
                }
                else if (DrawVerticalAxisTickMarks)
                {
                    for (int i = 0; i < VerticalAxisTickPositions.Count; i++)
                        drawingContext.DrawLine(ReferenceLinePen, new Point(VerticalAxis - TickMarksLength / 2, VerticalAxisTickPositions[i]), new Point(VerticalAxis + TickMarksLength / 2, VerticalAxisTickPositions[i]));
                }
            }
            drawingContext.DrawLine(ReferenceLinePen, new Point(VerticalAxis, 0), new Point(VerticalAxis, RenderSize.Height));
            if (HorizontalAxisTickPositions != null)
            {
                if (DrawHorizontalAxisReferenceLines)
                {
                    for (int i = 0; i < HorizontalAxisTickPositions.Count; i++)
                        drawingContext.DrawLine(ReferenceLinePen, new Point(HorizontalAxisTickPositions[i], 0), new Point(HorizontalAxisTickPositions[i], RenderSize.Height));
                }
                else if (DrawHorizontalAxisTickMarks)
                {
                    for (int i = 0; i < HorizontalAxisTickPositions.Count; i++)
                        drawingContext.DrawLine(ReferenceLinePen, new Point(HorizontalAxisTickPositions[i], HorizontalAxis - TickMarksLength / 2), new Point(HorizontalAxisTickPositions[i], HorizontalAxis + TickMarksLength / 2));
                }
            }
            drawingContext.DrawLine(ReferenceLinePen, new Point(0, HorizontalAxis), new Point(RenderSize.Width, HorizontalAxis));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void OnVerticalAxisTickValuesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ChartLines obj = sender as ChartLines;
            if (obj != null && obj.VerticalAxisTickPositions != null)
            {
                obj.InvalidateVisual();
                obj.VerticalAxisTickPositions.CollectionChanged += obj.VerticalAxisTickPositions_CollectionChanged;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void VerticalAxisTickPositions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            InvalidateVisual();
        }
        /// <summary>
        /// 
        /// </summary>
        public Pen ReferenceLinePen
        {
            get { return (Pen)GetValue(ReferenceLinePenProperty); }
            set { SetValue(ReferenceLinePenProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for ReferenceLinePen.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ReferenceLinePenProperty =
            DependencyProperty.Register("ReferenceLinePen", typeof(Pen), typeof(ChartLines), new UIPropertyMetadata(new Pen(Brushes.Black, 1.0)));
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<double> VerticalAxisTickPositions
        {
            get { return (ObservableCollection<double>)GetValue(VerticalAxisTickPositionsProperty); }
            set { SetValue(VerticalAxisTickPositionsProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for VerticalAxisTickPositions.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VerticalAxisTickPositionsProperty =
            DependencyProperty.Register("VerticalAxisTickPositions", typeof(ObservableCollection<double>), typeof(ChartLines), new FrameworkPropertyMetadata(null, OnVerticalAxisTickValuesChanged));
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<double> HorizontalAxisTickPositions
        {
            get { return (ObservableCollection<double>)GetValue(HorizontalAxisTickPositionsProperty); }
            set { SetValue(HorizontalAxisTickPositionsProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for HorizontalAxisTickPositions.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HorizontalAxisTickPositionsProperty =
            DependencyProperty.Register("HorizontalAxisTickPositions", typeof(ObservableCollection<double>), typeof(ChartLines), new UIPropertyMetadata(null));
        /// <summary>
        /// 
        /// </summary>
        public double TickMarksLength
        {
            get { return (double)GetValue(TickMarksLengthProperty); }
            set { SetValue(TickMarksLengthProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for TickMarksLength.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TickMarksLengthProperty =
            DependencyProperty.Register("TickMarksLength", typeof(double), typeof(ChartLines), new UIPropertyMetadata(8.0));
        /// <summary>
        /// 
        /// </summary>
        public bool DrawVerticalAxisTickMarks
        {
            get { return (bool)GetValue(DrawVerticalAxisTickMarksProperty); }
            set { SetValue(DrawVerticalAxisTickMarksProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for DrawVerticalAxisTickMarks.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DrawVerticalAxisTickMarksProperty =
            DependencyProperty.Register("DrawVerticalAxisTickMarks", typeof(bool), typeof(ChartLines), new UIPropertyMetadata(true));
        /// <summary>
        /// 
        /// </summary>
        public bool DrawVerticalAxisReferenceLines
        {
            get { return (bool)GetValue(DrawVerticalAxisReferenceLinesProperty); }
            set { SetValue(DrawVerticalAxisReferenceLinesProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for DrawVerticalAxisReferenceLines.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DrawVerticalAxisReferenceLinesProperty =
            DependencyProperty.Register("DrawVerticalAxisReferenceLines", typeof(bool), typeof(ChartLines), new UIPropertyMetadata(true));
        /// <summary>
        /// 
        /// </summary>
        public bool DrawHorizontalAxisTickMarks
        {
            get { return (bool)GetValue(DrawHorizontalAxisTickMarksProperty); }
            set { SetValue(DrawHorizontalAxisTickMarksProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for DrawHorizontalAxisTickMarks.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DrawHorizontalAxisTickMarksProperty =
            DependencyProperty.Register("DrawHorizontalAxisTickMarks", typeof(bool), typeof(ChartLines), new UIPropertyMetadata(true));
        /// <summary>
        /// 
        /// </summary>
        public bool DrawHorizontalAxisReferenceLines
        {
            get { return (bool)GetValue(DrawHorizontalAxisReferenceLinesProperty); }
            set { SetValue(DrawHorizontalAxisReferenceLinesProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for DrawHorizontalAxisReferenceLines.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DrawHorizontalAxisReferenceLinesProperty =
            DependencyProperty.Register("DrawHorizontalAxisReferenceLines", typeof(bool), typeof(ChartLines), new UIPropertyMetadata(true));
        /// <summary>
        /// 
        /// </summary>
        public double HorizontalAxis
        {
            get { return (double)GetValue(HorizontalAxisProperty); }
            set { SetValue(HorizontalAxisProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for HorizontalAxis.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HorizontalAxisProperty =
            DependencyProperty.Register("HorizontalAxis", typeof(double), typeof(ChartLines), new UIPropertyMetadata(0.0));
        /// <summary>
        /// 
        /// </summary>
        public double VerticalAxis
        {
            get { return (double)GetValue(VerticalAxisProperty); }
            set { SetValue(VerticalAxisProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for VerticalAxis.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VerticalAxisProperty =
            DependencyProperty.Register("VerticalAxis", typeof(double), typeof(ChartLines), new UIPropertyMetadata(0.0));

    }
}
