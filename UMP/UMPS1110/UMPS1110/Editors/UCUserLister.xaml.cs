using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using UMPS1110.Models;

namespace UMPS1110.Editors
{
    /// <summary>
    /// UCUserLister.xaml 的交互逻辑
    /// </summary>
    public partial class UCUserLister
    {
        public UCUserLister()
        {
            InitializeComponent();
        }

        private void ObjectItem_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var panel = sender as Border;
                if (panel == null) { return; }
                var item = panel.Tag as ObjectItem;
                if (item == null) { return; }
                PathListerEventEventArgs args = new PathListerEventEventArgs();
                args.Code = 3;
                args.Data = item;
                //OnPathListerEvent(args);
            }
        }
    }
}
