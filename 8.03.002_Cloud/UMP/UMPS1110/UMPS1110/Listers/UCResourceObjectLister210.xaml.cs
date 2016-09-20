//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    41bf5202-c018-4e12-87a5-55b6968e6e77
//        CLR Version:              4.0.30319.18052
//        Name:                     UCResourceObjectLister210
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Listers
//        File Name:                UCResourceObjectLister210
//
//        created by Charley at 2015/4/18 18:40:53
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using UMPS1110.Models.ConfigObjects;

namespace UMPS1110.Listers
{
    /// <summary>
    /// UCResourceObjectLister210.xaml 的交互逻辑
    /// </summary>
    public partial class UCResourceObjectLister210
    {
        #region Members

        public ObjectItem ObjectItem;

        private ConfigObject mParentObject;
        

        #endregion
       

        public UCResourceObjectLister210()
        {
            InitializeComponent();

            Loaded += UCResourceObjectLister210_Loaded;
        }

        void UCResourceObjectLister210_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
