﻿using System.Windows;
using System.Windows.Controls;

namespace UMPS3603
{
    /// <summary>
    /// Shell.xaml 的交互逻辑
    /// </summary>
    public partial class Shell : Page
    {
        public Shell()
        {
            InitializeComponent();
            Loaded += Shell_Loaded;
        }

        void Shell_Loaded(object sender, RoutedEventArgs e)
        {
            ExamProductionView view = new ExamProductionView();
            view.CurrentApp = App.CurrentApp;
            BorderContent.Child = view;
        }
    }
}
