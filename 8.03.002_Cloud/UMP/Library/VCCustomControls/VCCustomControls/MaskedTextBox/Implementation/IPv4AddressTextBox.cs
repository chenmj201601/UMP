//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    164674f3-464f-41a9-bed0-8821775ef2f4
//        CLR Version:              4.0.30319.18444
//        Name:                     IPv4AddressTextBox
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.MaskedTextBox.Implementation
//        File Name:                IPv4AddressTextBox
//
//        created by Charley at 2015/1/16 18:30:31
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows;
using System.Windows.Controls;

namespace VoiceCyber.Wpf.CustomControls
{
    /// <summary>
    /// IP地址输入框
    /// </summary>
    public class IPv4AddressTextBox : UserControl
    {
        #region ValueProperty
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(IPv4AddressTextBox), new PropertyMetadata(default(string)));

        /// <summary>
        /// 
        /// </summary>
        public string Value
        {
            get
            {
                return (string)GetValue(ValueProperty);
            }
            set { SetValue(ValueProperty, value); }
        }

        #endregion
        

        static IPv4AddressTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IPv4AddressTextBox),
                new FrameworkPropertyMetadata(typeof(IPv4AddressTextBox)));
        }

        /// <summary>
        /// 
        /// </summary>
        public IPv4AddressTextBox()
        {
            Loaded += IPv4AddressTextBox_Loaded;
            DataObject.AddPastingHandler(this, IPTextBox_Paste);
        }

        void IPv4AddressTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (mTxtParagraph1 != null &&
                mTxtParagraph2 != null &&
                mTxtParagraph3 != null &&
                mTxtParagraph4 != null)
            {
                mTxtParagraph1.SetNeighbour(null, mTxtParagraph2);
                mTxtParagraph2.SetNeighbour(mTxtParagraph1, mTxtParagraph3);
                mTxtParagraph3.SetNeighbour(mTxtParagraph2, mTxtParagraph4);
                mTxtParagraph4.SetNeighbour(mTxtParagraph3, null);
            }
            SetIP(Value);
        }


        #region EventHandlers

        // 处理粘贴 类似ip的形式才能粘贴
        private void IPTextBox_Paste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string value = e.DataObject.GetData(typeof(string)).ToString();
                SetIP(value);
                Value = value;
                OnValueChanged(value);
            }
            e.CancelCommand();
        }

        void mTxtParagraph_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (mTxtParagraph1 != null &&
                 mTxtParagraph2 != null &&
                 mTxtParagraph3 != null &&
                 mTxtParagraph4 != null)
            {
                string value = GetIP();
                OnValueChanged(value);
            }
        }

        #endregion


        #region Others
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetIP()
        {
            if (mTxtParagraph1 != null &&
                 mTxtParagraph2 != null &&
                 mTxtParagraph3 != null &&
                 mTxtParagraph4 != null)
            {
                string str = string.Format("{0}.{1}.{2}.{3}",
                    string.IsNullOrEmpty(mTxtParagraph1.Text) ? "0" : mTxtParagraph1.Text,
                    string.IsNullOrEmpty(mTxtParagraph2.Text) ? "0" : mTxtParagraph2.Text,
                    string.IsNullOrEmpty(mTxtParagraph3.Text) ? "0" : mTxtParagraph3.Text,
                    string.IsNullOrEmpty(mTxtParagraph4.Text) ? "0" : mTxtParagraph4.Text);
                return str;
            }
            return "0.0.0.0";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        public void SetIP(string address)
        {
            try
            {
                string[] IPs = address.Split('.');
                if (IPs.Length == 4)
                {
                    int intValue;
                    for (int i = 0; i < IPs.Length; i++)
                    {
                        if (!Int32.TryParse(IPs[i], out intValue) || intValue > 255 || intValue < 0) { return; }
                    }
                    if (mTxtParagraph1 != null)
                    {
                        mTxtParagraph1.Text = IPs[0];
                    }
                    if (mTxtParagraph2 != null)
                    {
                        mTxtParagraph2.Text = IPs[1];
                    }
                    if (mTxtParagraph3 != null)
                    {
                        mTxtParagraph3.Text = IPs[2];
                    }
                    if (mTxtParagraph4 != null)
                    {
                        mTxtParagraph4.Text = IPs[3];
                    }
                }
            }
            catch { }
        }

        #endregion


        #region Templates

        private const string PART_Paragraph1 = "PART_Paragraph1";
        private const string PART_Paragraph2 = "PART_Paragraph2";
        private const string PART_Paragraph3 = "PART_Paragraph3";
        private const string PART_Paragraph4 = "PART_Paragraph4";

        private IPv4ParagraphTextBox mTxtParagraph1;
        private IPv4ParagraphTextBox mTxtParagraph2;
        private IPv4ParagraphTextBox mTxtParagraph3;
        private IPv4ParagraphTextBox mTxtParagraph4;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mTxtParagraph1 = GetTemplateChild(PART_Paragraph1) as IPv4ParagraphTextBox;
            if (mTxtParagraph1 != null)
            {
                mTxtParagraph1.TextChanged += mTxtParagraph_TextChanged;
                SetIP(Value);
            }
            mTxtParagraph2 = GetTemplateChild(PART_Paragraph2) as IPv4ParagraphTextBox;
            if (mTxtParagraph2 != null)
            {
                mTxtParagraph2.TextChanged += mTxtParagraph_TextChanged;
                SetIP(Value);
            }
            mTxtParagraph3 = GetTemplateChild(PART_Paragraph3) as IPv4ParagraphTextBox;
            if (mTxtParagraph3 != null)
            {
                mTxtParagraph3.TextChanged += mTxtParagraph_TextChanged;
                SetIP(Value);
            }
            mTxtParagraph4 = GetTemplateChild(PART_Paragraph4) as IPv4ParagraphTextBox;
            if (mTxtParagraph4 != null)
            {
                mTxtParagraph4.TextChanged += mTxtParagraph_TextChanged;
                SetIP(Value);
            }
        }

        #endregion


        #region ValueChangedEvent
        /// <summary>
        /// 
        /// </summary>
        public event Action<string> ValueChanged;

        private void OnValueChanged(string value)
        {
            if (ValueChanged != null)
            {
                ValueChanged(value);
            }
        }

        #endregion
    }
}
