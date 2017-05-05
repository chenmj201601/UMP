//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7e957fb2-89b8-4905-91ec-43d6cc6de623
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreItemTipPreviewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls
//        File Name:                ScoreItemTipPreviewer
//
//        created by Charley at 2014/8/6 9:53:54
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;
using System.Windows.Controls;

namespace VoiceCyber.UMP.ScoreSheets.Controls
{
    [TemplatePart(Name = PART_Panel, Type = typeof(Border))]
    public class ScoreItemTipPreviewer : ScoreObjectPreViewer
    {
        public static readonly DependencyProperty ScoreItemProperty =
            DependencyProperty.Register("ScoreItem", typeof(ScoreItem), typeof(ScoreItemTipPreviewer), new PropertyMetadata(default(ScoreItem)));

        public ScoreItem ScoreItem
        {
            get { return (ScoreItem)GetValue(ScoreItemProperty); }
            set { SetValue(ScoreItemProperty, value); }
        }

        public static readonly DependencyProperty ScoreTipProperty =
            DependencyProperty.Register("ScoreTip", typeof(string), typeof(ScoreItemTipPreviewer), new PropertyMetadata(default(string)));

        public string ScoreTip
        {
            get { return (string)GetValue(ScoreTipProperty); }
            set { SetValue(ScoreTipProperty, value); }
        }

        static ScoreItemTipPreviewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScoreItemTipPreviewer),
                new FrameworkPropertyMetadata(typeof(ScoreItemTipPreviewer)));
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

        private const string PART_Panel = "PART_Panel";
        private Border mBorderPanel;

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
                            ScoreTip = string.Format("{0}/{1}", standard.Score, standard.PointSystem);
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
