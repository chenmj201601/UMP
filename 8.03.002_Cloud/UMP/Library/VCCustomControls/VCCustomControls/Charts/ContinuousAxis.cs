using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace VoiceCyber.Wpf.CustomControls.Charts
{
    /// <summary>
    /// 
    /// </summary>
    public class ContinuousAxis : ItemsControl
    {
        static ContinuousAxis()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ContinuousAxis), new FrameworkPropertyMetadata(typeof(ContinuousAxis)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
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
        }
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<double> SourceValues
        {
            get { return (ObservableCollection<double>)GetValue(SourceValuesProperty); }
            set { SetValue(SourceValuesProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for SourceValues.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SourceValuesProperty =
            DependencyProperty.Register("SourceValues", typeof(ObservableCollection<double>), typeof(ContinuousAxis), new UIPropertyMetadata(null));
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<double> Values
        {
            get { return (ObservableCollection<double>)GetValue(ValuesProperty); }
            set { SetValue(ValuesProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ValuesProperty =
            DependencyProperty.Register("Values", typeof(ObservableCollection<double>), typeof(ContinuousAxis), new UIPropertyMetadata(new ObservableCollection<double>()));
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
            DependencyProperty.Register("TickPositions", typeof(ObservableCollection<double>), typeof(ContinuousAxis), new UIPropertyMetadata(null));
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
            DependencyProperty.Register("Origin", typeof(double), typeof(ContinuousAxis), new UIPropertyMetadata(0.0));
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
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ContinuousAxis), new UIPropertyMetadata(Orientation.Vertical));
        /// <summary>
        /// 
        /// </summary>
        public double ReferenceLineSeperation
        {
            get { return (double)GetValue(ReferenceLineSeperationProperty); }
            set { SetValue(ReferenceLineSeperationProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ReferenceLineSeperationProperty =
            DependencyProperty.Register("ReferenceLineSeperation", typeof(double), typeof(ContinuousAxis), new UIPropertyMetadata(null));

    }
}
