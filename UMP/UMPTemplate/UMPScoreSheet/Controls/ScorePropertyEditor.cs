//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1091496f-1204-4f80-823e-bdb09e7d84f7
//        CLR Version:              4.0.30319.18444
//        Name:                     ScorePropertyEditor
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls
//        File Name:                ScorePropertyEditor
//
//        created by Charley at 2014/7/28 10:42:01
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using VoiceCyber.UMP.ScoreSheets.Converters;

namespace VoiceCyber.UMP.ScoreSheets.Controls
{
    public class ScorePropertyEditor : Control
    {
        private Border mBorderValue;

        public static readonly DependencyProperty ScoreObjectProperty =
            DependencyProperty.Register("ScoreObject", typeof(ScoreObject), typeof(ScorePropertyEditor), new PropertyMetadata(default(ScoreObject)));

        public ScoreObject ScoreObject
        {
            get { return (ScoreObject)GetValue(ScoreObjectProperty); }
            set { SetValue(ScoreObjectProperty, value); }
        }

        public static readonly DependencyProperty ScorePropertyProperty =
            DependencyProperty.Register("ScoreProperty", typeof(ScoreProperty), typeof(ScorePropertyEditor), new PropertyMetadata(default(ScoreProperty)));

        public ScoreProperty ScoreProperty
        {
            get { return (ScoreProperty)GetValue(ScorePropertyProperty); }
            set { SetValue(ScorePropertyProperty, value); }
        }

        public static readonly DependencyProperty ScorePropertyGridProperty =
            DependencyProperty.Register("ScorePropertyGrid", typeof (ScorePropertyGrid), typeof (ScorePropertyEditor), new PropertyMetadata(default(ScorePropertyGrid)));

        public ScorePropertyGrid ScorePropertyGrid
        {
            get { return (ScorePropertyGrid) GetValue(ScorePropertyGridProperty); }
            set { SetValue(ScorePropertyGridProperty, value); }
        }

        static ScorePropertyEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScorePropertyEditor), new FrameworkPropertyMetadata(typeof(ScorePropertyEditor)));
        }

        public ScorePropertyEditor()
        {
            this.Loaded += ScorePropertyEditor_Loaded;
        }

        void ScorePropertyEditor_Loaded(object sender, RoutedEventArgs e)
        {
            if (ScoreProperty == null) { return; }

            Binding binding;
            switch (ScoreProperty.DataType)
            {
                case ScorePropertyDataType.Double:
                    DoubleUpDownEditor doubleUpDown = new DoubleUpDownEditor();
                    binding = new Binding(ScoreProperty.PropertyName);
                    binding.Source = ScoreObject;
                    doubleUpDown.SetBinding(DoubleUpDownEditor.ValueProperty, binding);
                    if (mBorderValue != null)
                    {
                        mBorderValue.Child = doubleUpDown;
                    }
                    break;
                case ScorePropertyDataType.MString:
                    MultiLineEditor multiLineText = new MultiLineEditor();
                    multiLineText.Text = string.Empty;
                    binding = new Binding(ScoreProperty.PropertyName);
                    binding.Source = ScoreObject;
                    multiLineText.SetBinding(MultiLineEditor.TextProperty, binding);
                    binding = new Binding("Text");
                    binding.Source = multiLineText;
                    binding.Converter = new MultLineTextConverter();
                    multiLineText.SetBinding(MultiLineEditor.ContentProperty, binding);
                    if (mBorderValue != null)
                    {
                        mBorderValue.Child = multiLineText;
                    }
                    break;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mBorderValue = GetTemplateChild("PART_BorderValue") as Border;
            if (mBorderValue != null)
            {

            }
        }
    }
}
