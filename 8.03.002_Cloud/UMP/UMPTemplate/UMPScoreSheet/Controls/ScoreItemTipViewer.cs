//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c13276ef-d3b1-49cb-933f-0722eddd8612
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreItemTipViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls
//        File Name:                ScoreItemTipViewer
//
//        created by Charley at 2014/8/12 10:58:54
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;
using System.Windows.Controls;

namespace VoiceCyber.UMP.ScoreSheets.Controls
{
      [TemplatePart(Name = PART_Panel, Type = typeof(Border))]
    public class ScoreItemTipViewer:ScoreObjectViewer
    {
        public static readonly DependencyProperty ScoreItemProperty =
            DependencyProperty.Register("ScoreItem", typeof (ScoreItem), typeof (ScoreItemTipViewer), new PropertyMetadata(default(ScoreItem)));

        public ScoreItem ScoreItem
        {
            get { return (ScoreItem) GetValue(ScoreItemProperty); }
            set { SetValue(ScoreItemProperty, value); }
        }

        public static readonly DependencyProperty ScoreTipProperty =
            DependencyProperty.Register("ScoreTip", typeof (string), typeof (ScoreItemTipViewer), new PropertyMetadata(default(string)));

        public string ScoreTip
        {
            get { return (string) GetValue(ScoreTipProperty); }
            set { SetValue(ScoreTipProperty, value); }
        }

        private const string PART_Panel = "PART_Panel";
        private Border mBorderPanel;

        static ScoreItemTipViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScoreItemTipViewer),
                new FrameworkPropertyMetadata(typeof(ScoreItemTipViewer)));
        }

        public override void Init()
        {
            base.Init();

            if (ScoreItem != null)
            {
                ScoreItem.OnPropertyChanged += ScoreItem_OnPropertyChanged;
            }

            InitTipValue();
        }

        void ScoreItem_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetTipValue();
        }

        private void InitTipValue()
        {
            Standard standard = ScoreItem as Standard;
            if (standard != null)
            {
                ScoreSheet scoreSheet = standard.ScoreSheet;
                if (scoreSheet != null)
                {
                    if (scoreSheet.UsePointSystem)
                    {
                        if (ViewMode == 1 || ViewMode == 2)
                        {
                            ScoreTip = string.Format("{0}/{1}", standard.Score, standard.PointSystem);
                        }
                        else
                        {
                            ScoreTip = string.Format("{0}/{1}", standard.GetDefaultValue(), standard.PointSystem);
                        }
                    }
                    else
                    {
                        if (ViewMode == 1 || ViewMode == 2)
                        {
                            ScoreTip = string.Format("{0}/{1}", standard.Score, standard.TotalScore);
                        }
                        else
                        {
                            ScoreTip = string.Format("{0}/{1}", standard.GetDefaultValue(), standard.TotalScore);
                        }
                    }
                }
            }
        }

        private void SetTipValue()
        {
            Standard standard = ScoreItem as Standard;
            if (standard != null)
            {
                ScoreSheet scoreSheet = standard.ScoreSheet;
                if (scoreSheet != null)
                {
                    if (scoreSheet.UsePointSystem)
                    {
                        ScoreTip = string.Format("{0}/{1}", standard.Score, standard.PointSystem);
                    }
                    else
                    {
                        ScoreTip = string.Format("{0}/{1}", standard.Score, standard.TotalScore);
                    }
                }
            }
        }
    }
}
