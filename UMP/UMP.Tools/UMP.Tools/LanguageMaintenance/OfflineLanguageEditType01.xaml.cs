﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UMP.Tools.PublicClasses;

namespace UMP.Tools.LanguageMaintenance
{
    public partial class OfflineLanguageEditType01 : Window
    {
        public OfflineLanguageFileType01 IOfflineLanguageFileType01 = null;

        private ListViewItemSingle IListViewItemSingle = null;
        private string IStrContentOrTip = string.Empty;

        public OfflineLanguageEditType01()
        {
            InitializeComponent();
            this.Loaded += OfflineLanguageEditType01_Loaded;
            this.MouseLeftButtonDown += OfflineLanguageEditType01_MouseLeftButtonDown;

            ButtonCloseThis.Click += WindowsButtonClicked;
            ButtonApplyEdit.Click += WindowsButtonClicked;
            ButtonCloseWindow.Click += WindowsButtonClicked;
        }

        public void ShowSingleLanguageDetail(ListViewItemSingle AListViewItemSingleLanguage, string AStrCOrT)
        {
            IListViewItemSingle = AListViewItemSingleLanguage;
            IStrContentOrTip = AStrCOrT;

            TabItemLanguageEdit.Header = " " + AListViewItemSingleLanguage.MessageID + " ";
            if (AStrCOrT == "C")
            {
                TextBoxEditBody.Text = AListViewItemSingleLanguage.MessageContentText;
                LabelLanguageEditTip.Content = string.Format(App.GetDisplayCharater("M02001"), App.GetDisplayCharater("L001C003"));
            }
            else
            {
                TextBoxEditBody.Text = AListViewItemSingleLanguage.MessageTipDisplay;
                LabelLanguageEditTip.Content = string.Format(App.GetDisplayCharater("M02001"), App.GetDisplayCharater("L001C004"));
            }
        }

        private void DisplayElementCharacters(bool ABoolLanguageChange)
        {
            ButtonApplyEdit.Content = App.GetDisplayCharater("M02002");
            ButtonCloseWindow.Content = App.GetDisplayCharater("M02003");
        }

        private void WindowsButtonClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                Button LButtonClicked = sender as Button;
                string LStrClickedName = LButtonClicked.Name;

                switch (LStrClickedName)
                {
                    case "ButtonCloseThis":
                        CloseThisWindow();
                        break;
                    case "ButtonApplyEdit":
                        ApplyLanguageChanged();
                        break;
                    case "ButtonCloseWindow":
                        CloseThisWindow();
                        break;
                    default:
                        break;
                }

            }
            catch { }
        }

        private void ApplyLanguageChanged()
        {
            if (IOfflineLanguageFileType01.ChangeModifiedLanguage(TextBoxEditBody.Text))
            {
                CloseThisWindow();
            }
        }

        private void CloseThisWindow()
        {
            this.Close();
        }

        private void OfflineLanguageEditType01_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void OfflineLanguageEditType01_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                App.DrawWindowsBackGround(this);
                ImageLanguageEdit.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000044.ico"), UriKind.RelativeOrAbsolute));
                DisplayElementCharacters(false);
            }
            catch { }
        }
        
    }
}
