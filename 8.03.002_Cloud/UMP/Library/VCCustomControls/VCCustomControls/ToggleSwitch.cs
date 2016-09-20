//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6f403a54-6b47-4026-a29f-edbfbf699d8b
//        CLR Version:              4.0.30319.18444
//        Name:                     ToggleSwitch
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls
//        File Name:                ToggleSwitch
//
//        created by Charley at 2014/6/20 14:07:10
//        http://www.voicecyber.com 
//
//======================================================================
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace VoiceCyber.CustomControls
{
    /// <summary>
    /// 自定义开关
    /// </summary>
    [TemplatePart(Name = "PART_ContentBorder", Type = typeof(Border))]
    [TemplatePart(Name = "PART_RootGrid", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_ContentGrid", Type = typeof(Grid))]
    public class ToggleSwitch : ToggleButton
    {
        #region Fields

        Grid mRootGrid;
        Border mContentBorder;
        Grid mContentGrid;
        double mContentBorderMargin;

        #endregion

        #region Constants

        private const double DEFAULT_THUMB_WIDTH = 40.0;
        private const double MIN_THUMB_WIDTH = 10.0;
        private const double MAX_THUMB_WIDTH = 90.0;

        #endregion

        #region Dependency Properties

        #region CheckedText

        /// <summary>
        /// CheckedText Dependency Property
        /// </summary>
        public static readonly DependencyProperty CheckedTextProperty =
            DependencyProperty.Register("CheckedText", typeof(string), typeof(ToggleSwitch),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the CheckedText property. This dependency property 
        /// indicates the on text.
        /// </summary>
        public string CheckedText
        {
            get { return (string)GetValue(CheckedTextProperty); }
            set { SetValue(CheckedTextProperty, value); }
        }

        #endregion

        #region CheckedBackground

        /// <summary>
        /// CheckedBackground Dependency Property
        /// </summary>
        public static readonly DependencyProperty CheckedBackgroundProperty =
            DependencyProperty.Register("CheckedBackground", typeof(Brush), typeof(ToggleSwitch),
                new PropertyMetadata(Brushes.White));

        /// <summary>
        /// Gets or sets the CheckedBackground property. This dependency property 
        /// indicates Background of the Checked Text.
        /// </summary>
        public Brush CheckedBackground
        {
            get { return (Brush)GetValue(CheckedBackgroundProperty); }
            set { SetValue(CheckedBackgroundProperty, value); }
        }

        #endregion

        #region CheckedForeground

        /// <summary>
        /// CheckedForeground Dependency Property
        /// </summary>
        public static readonly DependencyProperty CheckedForegroundProperty =
            DependencyProperty.Register("CheckedForeground", typeof(Brush), typeof(ToggleSwitch),
                new PropertyMetadata(Brushes.Black));

        /// <summary>
        /// Gets or sets the CheckedForeground property. This dependency property 
        /// indicates Foreground of the Checked Text.
        /// </summary>
        public Brush CheckedForeground
        {
            get { return (Brush)GetValue(CheckedForegroundProperty); }
            set { SetValue(CheckedForegroundProperty, value); }
        }

        #endregion

        #region CornerRadius

        /// <summary>
        /// CornerRadius Dependency Property
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ToggleSwitch),
                new PropertyMetadata(new CornerRadius()));

        /// <summary>
        /// Gets or sets the CornerRadius property. This dependency property 
        /// indicates the corner radius of the outer most border of the ToggleSwitch.
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        #endregion

        #region TargetColumnInternal

        /// <summary>
        /// TargetColumnInternal Dependency Property
        /// </summary>
        public static readonly DependencyProperty TargetColumnInternalProperty =
            DependencyProperty.Register("TargetColumnInternal", typeof(int), typeof(ToggleSwitch),
                new FrameworkPropertyMetadata((OnTargetColumnInternalChanged)));

        /// <summary>
        /// Gets or sets the TargetColumnInternal property. This dependency property 
        /// indicates the target column to which the contentborder moves when the control is in unchecked state.
        /// This property is used internally.
        /// </summary>
        public int TargetColumnInternal
        {
            get { return (int)GetValue(TargetColumnInternalProperty); }
            set { SetValue(TargetColumnInternalProperty, value); }
        }

        /// <summary>
        /// Handles changes to the TargetColumnInternal property.
        /// </summary>
        /// <param name="d">ToggleSwitch</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnTargetColumnInternalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToggleSwitch ts = (ToggleSwitch)d;
            int oldTargetColumnInternal = (int)e.OldValue;
            int newTargetColumnInternal = ts.TargetColumnInternal;
            ts.OnTargetColumnInternalChanged(oldTargetColumnInternal, newTargetColumnInternal);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the TargetColumnInternal property.
        /// </summary>
        /// <param name="oldTargetColumnInternal">Old Value</param>
        /// <param name="newTargetColumnInternal">New Value</param>
        protected virtual void OnTargetColumnInternalChanged(int oldTargetColumnInternal, int newTargetColumnInternal)
        {

        }

        #endregion

        #region ThumbBackground

        /// <summary>
        /// ThumbBackground Dependency Property
        /// </summary>
        public static readonly DependencyProperty ThumbBackgroundProperty =
            DependencyProperty.Register("ThumbBackground", typeof(Brush), typeof(ToggleSwitch),
                new PropertyMetadata((Brushes.Black)));

        /// <summary>
        /// Gets or sets the ThumbBackground property. This dependency property 
        /// indicates the Background of the Thumb.
        /// </summary>
        public Brush ThumbBackground
        {
            get { return (Brush)GetValue(ThumbBackgroundProperty); }
            set { SetValue(ThumbBackgroundProperty, value); }
        }

        #endregion

        #region ThumbBorderBrush

        /// <summary>
        /// ThumbBorderBrush Dependency Property
        /// </summary>
        public static readonly DependencyProperty ThumbBorderBrushProperty =
            DependencyProperty.Register("ThumbBorderBrush", typeof(Brush), typeof(ToggleSwitch),
                new PropertyMetadata(Brushes.Gray));

        /// <summary>
        /// Gets or sets the ThumbBorderBrush property. This dependency property 
        /// indicates the BorderBrush of the Thumb.
        /// </summary>
        public Brush ThumbBorderBrush
        {
            get { return (Brush)GetValue(ThumbBorderBrushProperty); }
            set { SetValue(ThumbBorderBrushProperty, value); }
        }

        #endregion

        #region ThumbBorderThickness

        /// <summary>
        /// ThumbBorderThickness Dependency Property
        /// </summary>
        public static readonly DependencyProperty ThumbBorderThicknessProperty =
            DependencyProperty.Register("ThumbBorderThickness", typeof(Thickness), typeof(ToggleSwitch),
                new PropertyMetadata(new Thickness()));

        /// <summary>
        /// Gets or sets the ThumbBorderThickness property. This dependency property 
        /// indicates the BorderThickness of the Thumb.
        /// </summary>
        public Thickness ThumbBorderThickness
        {
            get { return (Thickness)GetValue(ThumbBorderThicknessProperty); }
            set { SetValue(ThumbBorderThicknessProperty, value); }
        }

        #endregion

        #region ThumbCornerRadius

        /// <summary>
        /// ThumbCornerRadius Dependency Property
        /// </summary>
        public static readonly DependencyProperty ThumbCornerRadiusProperty =
            DependencyProperty.Register("ThumbCornerRadius", typeof(CornerRadius), typeof(ToggleSwitch),
                new PropertyMetadata(new CornerRadius()));

        /// <summary>
        /// Gets or sets the ThumbCornerRadius property. This dependency property 
        /// indicates the corner radius of the Thumb.
        /// </summary>
        public CornerRadius ThumbCornerRadius
        {
            get { return (CornerRadius)GetValue(ThumbCornerRadiusProperty); }
            set { SetValue(ThumbCornerRadiusProperty, value); }
        }

        #endregion

        #region ThumbGlowColor

        /// <summary>
        /// ThumbGlowColor Dependency Property
        /// </summary>
        public static readonly DependencyProperty ThumbGlowColorProperty =
            DependencyProperty.Register("ThumbGlowColor", typeof(Color), typeof(ToggleSwitch),
                new PropertyMetadata(Colors.LawnGreen));

        /// <summary>
        /// Gets or sets the ThumbGlowColor property. This dependency property 
        /// indicates the GlowColor of the Thumb.
        /// </summary>
        public Color ThumbGlowColor
        {
            get { return (Color)GetValue(ThumbGlowColorProperty); }
            set { SetValue(ThumbGlowColorProperty, value); }
        }

        #endregion

        #region ThumbShineCornerRadius

        /// <summary>
        /// ThumbShineCornerRadius Dependency Property
        /// </summary>
        public static readonly DependencyProperty ThumbShineCornerRadiusProperty =
            DependencyProperty.Register("ThumbShineCornerRadius", typeof(CornerRadius), typeof(ToggleSwitch),
                new FrameworkPropertyMetadata(new CornerRadius()));

        /// <summary>
        /// Gets or sets the ThumbShineCornerRadius property. This dependency property 
        /// indicates the corner radius of the shine over the thumb.
        /// </summary>
        public CornerRadius ThumbShineCornerRadius
        {
            get { return (CornerRadius)GetValue(ThumbShineCornerRadiusProperty); }
            set { SetValue(ThumbShineCornerRadiusProperty, value); }
        }

        #endregion

        #region ThumbWidth

        /// <summary>
        /// ThumbWidth Dependency Property
        /// </summary>
        public static readonly DependencyProperty ThumbWidthProperty =
            DependencyProperty.Register("ThumbWidth", typeof(double), typeof(ToggleSwitch),
                new FrameworkPropertyMetadata(DEFAULT_THUMB_WIDTH,
                    FrameworkPropertyMetadataOptions.AffectsMeasure,
                    OnThumbWidthChanged,
                    CoerceThumbWidth));

        /// <summary>
        /// Gets or sets the ThumbWidth property. This dependency property 
        /// indicates the width  of the Thumb as a percentage of the total width of the ToggleSwitch.
        /// </summary>
        public double ThumbWidth
        {
            get { return (double)GetValue(ThumbWidthProperty); }
            set { SetValue(ThumbWidthProperty, value); }
        }

        /// <summary>
        /// Coerces the Thumb Width to an acceptable value
        /// </summary>
        /// <param name="d">Dependency Object</param>
        /// <param name="value">Value</param>
        /// <returns>Coerced Value</returns>
        private static object CoerceThumbWidth(DependencyObject d, object value)
        {
            double percentage = (double)value;

            if (percentage < MIN_THUMB_WIDTH)
            {
                return MIN_THUMB_WIDTH;
            }

            if (percentage > MAX_THUMB_WIDTH)
            {
                return MAX_THUMB_WIDTH;
            }

            return percentage;
        }

        /// <summary>
        /// Handles changes to the ThumbWidth property.
        /// </summary>
        /// <param name="d">ToggleSwitch</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnThumbWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToggleSwitch ts = (ToggleSwitch)d;
            double oldThumbWidth = (double)e.OldValue;
            double newThumbWidth = ts.ThumbWidth;
            ts.OnThumbWidthChanged(oldThumbWidth, newThumbWidth);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the ThumbWidth property.
        /// </summary>
        /// <param name="oldThumbWidth">Old Value</param>
        /// <param name="newThumbWidth">New Value</param>
        protected virtual void OnThumbWidthChanged(double oldThumbWidth, double newThumbWidth)
        {
            CalculateLayout();
        }

        #endregion

        #region UncheckedBackground

        /// <summary>
        /// UncheckedBackground Dependency Property
        /// </summary>
        public static readonly DependencyProperty UncheckedBackgroundProperty =
            DependencyProperty.Register("UncheckedBackground", typeof(Brush), typeof(ToggleSwitch),
                new PropertyMetadata(Brushes.White));

        /// <summary>
        /// Gets or sets the UncheckedBackground property. This dependency property 
        /// indicates the Background of the Unchecked Text.
        /// </summary>
        public Brush UncheckedBackground
        {
            get { return (Brush)GetValue(UncheckedBackgroundProperty); }
            set { SetValue(UncheckedBackgroundProperty, value); }
        }

        #endregion

        #region UncheckedForeground

        /// <summary>
        /// UncheckedForeground Dependency Property
        /// </summary>
        public static readonly DependencyProperty UncheckedForegroundProperty =
            DependencyProperty.Register("UncheckedForeground", typeof(Brush), typeof(ToggleSwitch),
                new PropertyMetadata(Brushes.Black));

        /// <summary>
        /// Gets or sets the UncheckedForeground property. This dependency property 
        /// indicates the Foreground of the Unchecked Text.
        /// </summary>
        public Brush UncheckedForeground
        {
            get { return (Brush)GetValue(UncheckedForegroundProperty); }
            set { SetValue(UncheckedForegroundProperty, value); }
        }

        #endregion

        #region UncheckedText

        /// <summary>
        /// UncheckedText Dependency Property
        /// </summary>
        public static readonly DependencyProperty UncheckedTextProperty =
            DependencyProperty.Register("UncheckedText", typeof(string), typeof(ToggleSwitch),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the UncheckedText property. This dependency property 
        /// indicates the off text.
        /// </summary>
        public string UncheckedText
        {
            get { return (string)GetValue(UncheckedTextProperty); }
            set { SetValue(UncheckedTextProperty, value); }
        }

        #endregion

        #endregion

        #region Construction

        static ToggleSwitch()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleSwitch), new FrameworkPropertyMetadata(typeof(ToggleSwitch)));
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Override which is called when the template is applied
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Get all the controls in the template
            GetTemplateParts();

            // Calculate the layout of the ToggleSwitch contents
            CalculateLayout();
        }

        /// <summary>
        /// Handler for the event raised when the size of the ToggleSwitch changes
        /// </summary>
        /// <param name="sizeInfo">Size Changed Info</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            // Calculate the margin of the content border
            CalculateContentBorderMargin();
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Gets the required controls in the template
        /// </summary>
        protected void GetTemplateParts()
        {
            // PART_RootGrid
            mRootGrid = GetChildControl<Grid>("PART_RootGrid");
            // PART_ContentBorder
            mContentBorder = GetChildControl<Border>("PART_ContentBorder");
            // PART_ContentGrid
            mContentGrid = GetChildControl<Grid>("PART_ContentGrid");
        }

        /// <summary>
        /// Generic method to get a control from the template
        /// </summary>
        /// <typeparam name="T">Type of the control</typeparam>
        /// <param name="ctrlName">Name of the control in the template</param>
        /// <returns>Control</returns>
        protected T GetChildControl<T>(string ctrlName) where T : DependencyObject
        {
            T ctrl = GetTemplateChild(ctrlName) as T;
            return ctrl;
        }

        /// <summary>
        /// Calculates the width and margin of the contents of the ContentBorder
        /// The following calculation is used: (Here p is the value of ThumbWidth)
        /// p = [10, 90]
        /// Margin = 1 - p
        /// Left = (1 - p)/(2 - p)
        /// Right = (1 - p)/(2 - p)
        /// Center = 0.03
        /// CenterLeft = 0.485 - Left
        /// CenterRight = 0.485 - Left
        /// </summary>
        private void CalculateLayout()
        {
            if ((mRootGrid == null) || (mContentGrid == null))
                return;

            // Convert the ThumbWidth value to a percentage
            double thumbPercentage = ThumbWidth / 100.0;
            // Calculate the percentage of Total width available for the content
            double contentPercentage = 1 - thumbPercentage;

            if (thumbPercentage <= 0.5)
            {
                // Calculate the width of the RootGrid Columns
                mRootGrid.ColumnDefinitions[0].Width = new GridLength(thumbPercentage, GridUnitType.Star);
                mRootGrid.ColumnDefinitions[1].Width = new GridLength(1.0 - (2 * thumbPercentage), GridUnitType.Star);
                mRootGrid.ColumnDefinitions[2].Width = new GridLength(thumbPercentage, GridUnitType.Star);

                // Adjust the thumb
                TargetColumnInternal = 2;
                Grid.SetColumnSpan(mContentBorder, 1);
            }
            else
            {
                // Calculate the width of the RootGrid Columns
                mRootGrid.ColumnDefinitions[0].Width = new GridLength(contentPercentage, GridUnitType.Star);
                mRootGrid.ColumnDefinitions[1].Width = new GridLength(1.0 - (2 * contentPercentage), GridUnitType.Star);
                mRootGrid.ColumnDefinitions[2].Width = new GridLength(contentPercentage, GridUnitType.Star);

                // Adjust the thumb
                TargetColumnInternal = 1;
                Grid.SetColumnSpan(mContentBorder, 2);
            }

            // Calculate the width of the ContentGrid Columns
            double leftRight = (1 - thumbPercentage) / (2 - thumbPercentage);
            double centerLeftRight = 0.485 - leftRight;
            double center = 0.03;
            mContentGrid.ColumnDefinitions[0].Width = new GridLength(leftRight, GridUnitType.Star);
            mContentGrid.ColumnDefinitions[1].Width = new GridLength(centerLeftRight, GridUnitType.Star);
            mContentGrid.ColumnDefinitions[2].Width = new GridLength(center, GridUnitType.Star);
            mContentGrid.ColumnDefinitions[3].Width = new GridLength(centerLeftRight, GridUnitType.Star);
            mContentGrid.ColumnDefinitions[4].Width = new GridLength(leftRight, GridUnitType.Star);

            mContentBorderMargin = contentPercentage;

            CalculateContentBorderMargin();

            InvalidateVisual();
        }

        /// <summary>
        /// Calculates the margin of the contentBorder
        /// </summary>
        private void CalculateContentBorderMargin()
        {
            if (mContentBorder != null)
            {
                // Change the margin of the content border so that its size is (1 + contentBorderMargin) times the width of
                // the Toggle switch
                mContentBorder.Margin = new Thickness(-(Width * mContentBorderMargin), 0, -(Width * mContentBorderMargin), 0);
            }
        }

        #endregion
    }
}
