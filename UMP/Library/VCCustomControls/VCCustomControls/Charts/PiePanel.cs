using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VoiceCyber.Wpf.CustomControls.Charts
{
    /// <summary>
    /// 
    /// </summary>
    public class PiePanel : Panel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInitialized(EventArgs e)
        {
            ParentControl = ((ItemsControl)((FrameworkElement)VisualTreeHelper.GetParent(this)).TemplatedParent);
            base.OnInitialized(e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void OnValuesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            PiePanel p = sender as PiePanel;
            if (p != null && GetValues(p) != null)
            {
                p.InvalidateArrange();
                GetValues(p).CollectionChanged += p.PiePanel_CollectionChanged;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PiePanel_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            InvalidateArrange();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            for (int i = 0; i < InternalChildren.Count; i++)
            {
                InternalChildren[i].Measure(availableSize);
            }
            return base.MeasureOverride(availableSize);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (GetValues(this) != null)
            {
                double total = 0.0;
                int leftCount = 0;      //饼的左侧拥有的扇形数
                int rightCount = 0;     //饼的右侧拥有的扇形数

                for (int i = 0; i < InternalChildren.Count; i++)
                {
                    total += GetValues(this)[i];
                }
                double offsetAngle = 0.0;

                double radius = finalSize.Width < finalSize.Height ? finalSize.Width / 2 : finalSize.Height / 2;
                radius -= 2;
                Point beginFigure = new Point(finalSize.Width / 2, finalSize.Height / 2);
                Point lineToBeforeTransform = new Point(beginFigure.X + radius, beginFigure.Y);
                Point centerBeforeTransform = new Point(beginFigure.X + radius / 2, beginFigure.Y);
                for (int i = 0; i < InternalChildren.Count; i++)
                {
                    ContentControl container = InternalChildren[i] as ContentControl;
                    if (container == null)
                    {
                        throw new NoNullAllowedException(string.Format("Child control is null."));
                    }
                    double proportion = GetValues(this)[i] / total * 1.0;
                    double wedgeAngle = proportion * 360;
                    RotateTransform rt = new RotateTransform(offsetAngle, beginFigure.X, beginFigure.Y);
                    container.SetValue(BeginFigurePointProperty, beginFigure);
                    container.SetValue(LineToPointProperty, rt.Transform(lineToBeforeTransform));
                    container.SetValue(WedgeAngleProperty, wedgeAngle);
                    container.SetValue(ProportionProperty, proportion);

                    double wedgeAngle2 = wedgeAngle / 2 + offsetAngle;
                    RotateTransform rt2 = new RotateTransform(wedgeAngle2, beginFigure.X, beginFigure.Y);
                    Point centerPoint = rt2.Transform(centerBeforeTransform);
                    container.SetValue(CenterPointProperty, centerPoint);
                    container.SetValue(TranslateXProperty, centerPoint.X - beginFigure.X);
                    container.SetValue(TranslateYProperty, centerPoint.Y - beginFigure.Y);

                    if (centerPoint.X < 0)
                    {
                        leftCount++;
                    }
                    if (centerPoint.X >= 0)
                    {
                        rightCount++;
                    }
                    container.SetValue(LeftCountProperty, leftCount);
                    container.SetValue(RightCountProperty, rightCount);

                    offsetAngle += wedgeAngle;
                    Rect r = new Rect(finalSize);
                    container.Arrange(r);
                }
            }
            return finalSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetWedgeAngle(DependencyObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            return (double)obj.GetValue(WedgeAngleProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetWedgeAngle(DependencyObject obj, double value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            obj.SetValue(WedgeAngleProperty, value);
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty WedgeAngleProperty =
            DependencyProperty.RegisterAttached("WedgeAngle", typeof(double), typeof(PiePanel), new UIPropertyMetadata(0.0));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Point GetBeginFigurePoint(DependencyObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            return (Point)obj.GetValue(BeginFigurePointProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetBeginFigurePoint(DependencyObject obj, Point value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            obj.SetValue(BeginFigurePointProperty, value);
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty BeginFigurePointProperty =
            DependencyProperty.RegisterAttached("BeginFigurePoint", typeof(Point), typeof(PiePanel), new UIPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Point GetLineToPoint(DependencyObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            return (Point)obj.GetValue(LineToPointProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetLineToPoint(DependencyObject obj, Point value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            obj.SetValue(LineToPointProperty, value);
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LineToPointProperty =
            DependencyProperty.RegisterAttached("LineToPoint", typeof(Point), typeof(PiePanel), new UIPropertyMetadata(null));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Point GetCenterPoint(DependencyObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            return (Point)obj.GetValue(CenterPointProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetCenterPoint(DependencyObject obj, Point value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            obj.SetValue(CenterPointProperty, value);
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CenterPointProperty =
            DependencyProperty.RegisterAttached("CenterPoint", typeof(Point), typeof(PiePanel), new PropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetTranslateX(DependencyObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            return (double)obj.GetValue(TranslateXProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetTranslateX(DependencyObject obj, double value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            obj.SetValue(TranslateXProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TranslateXProperty =
            DependencyProperty.RegisterAttached("TranslateX", typeof(double), typeof(PiePanel), new PropertyMetadata(default(double)));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetTranslateY(DependencyObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            return (double)obj.GetValue(TranslateYProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetTranslateY(DependencyObject obj, double value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            obj.SetValue(TranslateYProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TranslateYProperty =
            DependencyProperty.RegisterAttached("TranslateY", typeof(double), typeof(PiePanel), new PropertyMetadata(default(double)));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetProportion(DependencyObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            return (double)obj.GetValue(ProportionProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void SetProportion(DependencyObject obj, double value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            obj.SetValue(ProportionProperty, value);
        }
       
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ProportionProperty =
            DependencyProperty.RegisterAttached("Proportion", typeof(double), typeof(PiePanel), new PropertyMetadata(default(double)));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int GetLeftCount(DependencyObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            return (int)obj.GetValue(LeftCountProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetLeftCount(DependencyObject obj, int value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            obj.SetValue(LeftCountProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LeftCountProperty =
            DependencyProperty.RegisterAttached("LeftCount", typeof(int), typeof(PiePanel), new PropertyMetadata(default(int)));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int GetRightCount(DependencyObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            return (int)obj.GetValue(RightCountProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetRightCount(DependencyObject obj, int value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            obj.SetValue(RightCountProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RightCountProperty =
            DependencyProperty.Register("RightCount", typeof (int), typeof (PiePanel), new PropertyMetadata(default(int)));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetTipX(DependencyObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            return (double)obj.GetValue(TipXProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetTipX(DependencyObject obj, double value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            obj.SetValue(TipXProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TipXProperty =
            DependencyProperty.Register("TipX", typeof (double), typeof (PiePanel), new PropertyMetadata(default(double)));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetTipY(DependencyObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            return (double)obj.GetValue(TipYProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetTipY(DependencyObject obj, double value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            obj.SetValue(TipYProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TipYProperty =
            DependencyProperty.Register("TipY", typeof (double), typeof (PiePanel), new PropertyMetadata(default(double)));


        /// <summary>
        /// 
        /// </summary>
        public PropertyPath ValuePath
        {
            get { return (PropertyPath)GetValue(ValuePathProperty); }
            set { SetValue(ValuePathProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ValuePathProperty =
            DependencyProperty.RegisterAttached("ValuePath", typeof(PropertyPath), typeof(ContinuousAxisPanel), new UIPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ObservableCollection<double> GetValues(DependencyObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            return (ObservableCollection<double>)obj.GetValue(ValuesProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetValues(DependencyObject obj, ObservableCollection<double> value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            obj.SetValue(ValuesProperty, value);
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ValuesProperty =
            DependencyProperty.RegisterAttached("Values", typeof(ObservableCollection<double>), typeof(PiePanel), new FrameworkPropertyMetadata(null, OnValuesChanged));

        /// <summary>
        /// 
        /// </summary>
        public ItemsControl ParentControl;
    }
}
