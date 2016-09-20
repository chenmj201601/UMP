//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e997d065-3fcc-433f-bb0e-f541d7fc8433
//        CLR Version:              4.0.30319.18444
//        Name:                     CollectionEditor
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Editors
//        File Name:                CollectionEditor
//
//        created by Charley at 2014/7/23 11:58:52
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Collections.Generic;
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

namespace VoiceCyber.Wpf.PropertyGrids.Editors
{
    /// <summary>
    /// Interaction logic for CollectionEditor.xaml
    /// </summary>
    public partial class CollectionEditor : UserControl, ITypeEditor
    {
        PropertyItem _item;

        public CollectionEditor()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //CollectionControlDialog editor = new CollectionControlDialog(_item.PropertyType, _item.DescriptorDefinition.NewItemTypes);
            //Binding binding = new Binding("Value");
            //binding.Source = _item;
            //binding.Mode = _item.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            //BindingOperations.SetBinding(editor, CollectionControlDialog.ItemsSourceProperty, binding);
            //editor.ShowDialog();
        }

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            _item = propertyItem;
            return this;
        }
    }
}
