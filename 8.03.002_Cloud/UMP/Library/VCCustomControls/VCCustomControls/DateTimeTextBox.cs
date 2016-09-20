//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d1f2007d-ed72-40d3-aa5b-ae3c5c230f98
//        CLR Version:              4.0.30319.18063
//        Name:                     DateTimeTextBox
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls
//        File Name:                DateTimeTextBox
//
//        created by Charley at 2014/4/1 15:20:04
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace VoiceCyber.CustomControls
{
    /// <summary>
    /// 一个日期时间的输入文本框
    /// </summary>
    public class DateTimeTextBox :TextBox
    {
        private Brush mBrush;    //存放原始文本框的边框样式

        /// <summary>
        /// 日期时间格式
        /// </summary>
        [Category("Text")]
        public DateTimeFormat Format
        {
            get { return (DateTimeFormat)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        /// <summary>
        /// 获取或设置文本框的值，若设置值不是有效的日期时间型数据，则显示空值
        /// </summary>
        [Category("Text")]
        new public string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = checkDateTime(value) ? value : GetEmptyValue();
            }
        }

        /// <summary>
        /// 增加一个依赖属性
        /// </summary>
        public static DependencyProperty FormatProperty = DependencyProperty.Register("Format", typeof(DateTimeFormat), typeof(DateTimeTextBox), new PropertyMetadata(DateTimeFormat.Default));

        /// <summary>
        /// 初始化控件显示文本
        /// </summary>
        public override void EndInit()
        {
            base.EndInit();
            Text = GetDefaultValue();
            mBrush = BorderBrush;
        }

        /// <summary>
        /// 截获按键输入消息，只响应数字键、方向键等，并使用按键值替换文本
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.Right)
            {
                base.OnPreviewKeyDown(e);
                return;
            }
            if (!((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)))
            {
                e.Handled = true;
                return;
            }
            if (CaretIndex > Text.Length - 1 || Text[CaretIndex] == '-' || Text[CaretIndex] == ':' || Text[CaretIndex] == ' ')
            {
                e.Handled = true;
                return;
            }
            Select(CaretIndex, 1);    //选择插入点当前位置的字符，以便用按键值替换它
            base.OnPreviewKeyDown(e);
        }

        /// <summary>
        /// 鼠标释放
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            SetInputIndex();
        }
        /// <summary>
        /// 文本内容更新
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            SetInputIndex();
            base.OnTextChanged(e);
        }

        /// <summary>
        /// 设置插入点，遇到分隔符，自动跳到下一位置
        /// </summary>
        private void SetInputIndex()
        {
            if (CaretIndex < Text.Length - 1)
            {
                if (Text[CaretIndex] == '-' || Text[CaretIndex] == ':' || Text[CaretIndex] == ' ')
                {
                    CaretIndex++;
                }
            }
        }
        /// <summary>
        /// 失去焦点
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (!checkDateTime(Text))
            {
                BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)); //红色边框指示输入无效
            }
            else
            {
                BorderBrush = mBrush;
            }
            base.OnLostFocus(e);
        }

        /// <summary>
        /// 验证输入
        /// </summary>
        /// <returns></returns>
        private bool checkDateTime(string strDatetime)
        {
            DateTime dt;
            return DateTime.TryParse(strDatetime, out dt);
        }

        private string GetEmptyValue()
        {
            switch (Format)
            {
                case DateTimeFormat.Default:
                    return "yyyy-MM-dd HH:mm:ss";
                case DateTimeFormat.Date:
                    return "yyyy-MM-dd";
                case DateTimeFormat.Time:
                    return "HH:mm:ss";
                default:
                    return "yyyy-MM-dd HH:mm:ss";
            }
        }

        private string GetDefaultValue()
        {
            switch (Format)
            {
                case DateTimeFormat.Default:
                    return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                case DateTimeFormat.Date:
                    return DateTime.Now.ToString("yyyy-MM-dd");
                case DateTimeFormat.Time:
                    return DateTime.Now.ToString("HH:mm:ss");
                default:
                    return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
    }

    /// <summary>
    /// 日期时间显示格式
    /// </summary>
    public enum DateTimeFormat
    {
        /// <summary>
        /// exp:2012-03-04 23:05:12
        /// </summary>
        Default,    //exp:2012-03-04 23:05:12
        /// <summary>
        /// exp:2012-03-04
        /// </summary>
        Date,       //exp:2012-03-04
        /// <summary>
        /// exp:23:05:12
        /// </summary>
        Time        //exp:23:05:12
    }
}
