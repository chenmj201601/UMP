//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e31d8e34-5866-48dc-bb36-e2087a80e33a
//        CLR Version:              4.0.30319.18444
//        Name:                     FormDemos
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VCCustomControlsDemo
//        File Name:                FormDemos
//
//        created by Charley at 2014/5/26 10:22:10
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace VCCustomControlsDemo
{
    /// <summary>
    /// FormDemos.xaml 的交互逻辑
    /// </summary>
    public partial class FormDemos
    {
        private Employee mRoot;

        public FormDemos()
        {
            InitializeComponent();

            mRoot = new Employee();

            this.Loaded += FormDemos_Loaded;
        }

        void FormDemos_Loaded(object sender, RoutedEventArgs e)
        {
            BtnTest.Click += BtnTest_Click;
            BtnExpand.Click += BtnExpand_Click;
            BtnCollaspe.Click += BtnCollaspe_Click;

            InitEmployees();
            mRoot.Init();
            TvEmployees.ItemsSource = mRoot.Children;
            TvCheckableTree.ItemsSource = mRoot.Children;
            InitColumns();
            mRoot.IsChecked = false;
        }

        void BtnCollaspe_Click(object sender, RoutedEventArgs e)
        {
            var emp = TvEmployees.SelectedItem as Employee;
            if (emp != null)
            {
                emp.IsExpanded = false;
            }
        }

        void BtnExpand_Click(object sender, RoutedEventArgs e)
        {
            var emp = TvEmployees.SelectedItem as Employee;
            if (emp != null)
            {
                emp.IsExpanded = true;
            }
        }

        void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            if (mRoot.Children.Count > 0)
            {
                mRoot.Children[0].IsExpanded = true;
            }
        }

        private void InitEmployees()
        {
            Employee item=new Employee();
            item.Name = "Item1";
            item.Job = "Master";
            item.Age = 20;
            item.Icon = "/Images/MediaTypeScreen.png";

            Employee child=new Employee();
            child.Name = "Child1";
            child.Job = "Woker";
            child.Age = 22;
            child.Icon = "/Images/MediaTypeVoice.png";
            item.Children.Add(child);

            child = new Employee();
            child.Name = "Child2";
            child.Job = "Team";
            child.Age = 23;
            child.PropertyChanged += child_PropertyChanged;
            child.Icon = "/Images/MediaTypeVoice.png";
            item.Children.Add(child);

            mRoot.Children.Add(item);

            item = new Employee();
            item.Name = "Item2";
            item.Job = "Master";
            item.Age = 25;
            item.Icon = "/Images/MediaTypeScreen.png";

            child = new Employee();
            child.Name = "Child3";
            child.Job = "Woker";
            child.Age = 22;
            child.Icon = "/Images/MediaTypeVoice.png";
            item.Children.Add(child);

            child = new Employee();
            child.Name = "Child4";
            child.Job = "Team";
            child.Age = 23;
            child.Icon = "/Images/MediaTypeVoice.png";
            item.Children.Add(child);

            mRoot.Children.Add(item);
        }

        void child_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var temp = e.PropertyName;
        }

        private void InitColumns()
        {
            List<GridViewColumn> listColumns = new List<GridViewColumn>();
            listColumns.Add(new GridViewColumn
            {
                Header = "Age",
                DisplayMemberBinding = new Binding("Age")
            });
            listColumns.Add(new GridViewColumn
            {
                Header = "Job",
                DisplayMemberBinding = new Binding("Job")
            });
            listColumns.Add(new GridViewColumn
            {
                Header = "IsExpanded",
                DisplayMemberBinding = new Binding("IsExpanded")
            });
            listColumns.Add(new GridViewColumn
            {
                Header = "IsChecked",
                DisplayMemberBinding = new Binding("IsChecked")
            });
            GridViewColumnHeader nameHeader=new GridViewColumnHeader();
            nameHeader.Content = "Name";
            TvEmployees.SetColumns(nameHeader, 120, listColumns);
        }
    }
}
