using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace VoiceCyber.Wpf.CustomControls.Charts
{
    /// <summary>
    /// 
    /// </summary>
    public class DiscreteAxis : ItemsControl
    {
        static DiscreteAxis()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiscreteAxis), new FrameworkPropertyMetadata(typeof(DiscreteAxis)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            //return new DiscreteAxisItem();
            return new ContentControl();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ContentControl;
            //return item is DiscreteAxisItem;
        }

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<double> TickPositions
        {
            get { return (ObservableCollection<double>)GetValue(TickPositionsProperty); }
            set { SetValue(TickPositionsProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TickPositionsProperty =
            DependencyProperty.Register("TickPositions", typeof(ObservableCollection<double>), typeof(DiscreteAxis), new UIPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public double Origin
        {
            get { return (double)GetValue(OriginProperty); }
            set { SetValue(OriginProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty OriginProperty =
            DependencyProperty.Register("Origin", typeof(double), typeof(DiscreteAxis), new UIPropertyMetadata(0.0));

        /// <summary>
        /// 
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(DiscreteAxis), new UIPropertyMetadata(Orientation.Horizontal));

        /// <summary>
        /// 
        /// </summary>
        public double TickLength
        {
            get { return (double)GetValue(TickLengthProperty); }
            set { SetValue(TickLengthProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TickLengthProperty =
            DependencyProperty.Register("TickLength", typeof(double), typeof(DiscreteAxis), new UIPropertyMetadata(null));
    }
}
