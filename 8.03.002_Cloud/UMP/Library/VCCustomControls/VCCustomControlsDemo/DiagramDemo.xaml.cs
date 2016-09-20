//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    57cddbb4-964b-4b72-a2c4-3463dd1e2376
//        CLR Version:              4.0.30319.18444
//        Name:                     DiagramDemo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VCCustomControlsDemo
//        File Name:                DiagramDemo
//
//        created by Charley at 2014/9/10 15:53:11
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using VoiceCyber.Wpf.CustomControls;
using System.Windows;

namespace VCCustomControlsDemo
{
    /// <summary>
    /// DiagramDemo.xaml 的交互逻辑
    /// </summary>
    public partial class DiagramDemo
    {
        private ObjectItem mRootItem;

        public DiagramDemo()
        {
            InitializeComponent();

            mRootItem=new ObjectItem();
            this.Loaded += (s, e) =>
            {
                TvObject.ItemsSource = mRootItem.Children;
                LoadDiagramNodes();
            };
        }

        public void LoadDiagramNodes()
        {

            ObjectItem root = new ObjectItem();
            root.Name = "Root";
            root.Description = "This is Root";
            root.Icon = "/Images/DiagramRootNode.png";
            mRootItem.AddChild(root);

            ObjectItem A = new ObjectItem();
            A.Name = "A";
            A.Description = "This is A";
            A.Icon = "/Images/DiagramNode.png";
            root.AddChild(A);

            ObjectItem B = new ObjectItem();
            B.Name = "B";
            B.Description = "This is B";
            B.Icon = "/Images/DiagramNode.png";
            root.AddChild(B);

            ObjectItem C = new ObjectItem();
            C.Name = "C";
            C.Description = "This is C";
            C.Icon = "/Images/DiagramNode.png";
            root.AddChild(C);

            ObjectItem A1 = new ObjectItem();
            A1.Name = "A1";
            A1.Description = "This is A1";
            A1.Icon = "/Images/DiagramNode.png";
            A.AddChild(A1);

            ObjectItem A2 = new ObjectItem();
            A2.Name = "A2";
            A2.Description = "This is A2";
            A2.Icon = "/Images/DiagramNode.png";
            A.AddChild(A2);

            ObjectItem A3 = new ObjectItem();
            A3.Name = "A3";
            A3.Description = "This is A3";
            A3.Icon = "/Images/DiagramNode.png";
            A.AddChild(A3);

            ObjectItem A4 = new ObjectItem();
            A4.Name = "A4";
            A4.Description = "This is A4";
            A4.Icon = "/Images/DiagramNode.png";
            A.AddChild(A4);

            ObjectItem B1 = new ObjectItem();
            B1.Name = "B1";
            B1.Description = "This is B1";
            B1.Icon = "/Images/DiagramNode.png";
            B.AddChild(B1);

            ObjectItem B2 = new ObjectItem();
            B2.Name = "B2";
            B2.Description = "This is B2";
            B2.Icon = "/Images/DiagramNode.png";
            B.AddChild(B2);

            ObjectItem B3 = new ObjectItem();
            B3.Name = "B3";
            B3.Description = "This is B3";
            B3.Icon = "/Images/DiagramNode.png";
            B.AddChild(B3);


            ObjectItem C1 = new ObjectItem();
            C1.Name = "C1";
            C1.Description = "This is C1";
            C1.Icon = "/Images/DiagramNode.png";
            C.AddChild(C1);

            ObjectItem A1a = new ObjectItem();
            A1a.Name = "A1a";
            A1a.Description = "This is A1a";
            A1a.Icon = "/Images/DiagramNode.png";
            A1.AddChild(A1a);

            ObjectItem A1b = new ObjectItem();
            A1b.Name = "A1b";
            A1b.Description = "This is A1b";
            A1b.Icon = "/Images/DiagramNode.png";
            A1.AddChild(A1b);

            ObjectItem A1c = new ObjectItem();
            A1c.Name = "A1c";
            A1c.Description = "This is A1c";
            A1c.Icon = "/Images/DiagramNode.png";
            A1.AddChild(A1c);
        }
    }

    public class ObjectItem : INotifyPropertyChanged
    {
        private string mName;
        private string mDescription;
        private bool mIsExpanded;
        private string mIcon;

        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }

        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; OnPropertyChanged("Description"); }
        }

        public string Icon
        {
            get { return mIcon; }
            set { mIcon = value; OnPropertyChanged("Icon"); }
        }

        public bool IsExpanded
        {
            get { return mIsExpanded; }
            set { mIsExpanded = value; OnPropertyChanged("IsExpanded"); }
        }

        private ObjectItem mParent;

        public ObjectItem Parent
        {
            get { return mParent; }
            set { mParent = value; }
        }

        private ObservableCollection<ObjectItem> mChildren;

        public ObservableCollection<ObjectItem> Children
        {
            get { return mChildren; }
        }

        public ObjectItem()
        {
            mChildren = new ObservableCollection<ObjectItem>();
        }

        public void AddChild(ObjectItem child)
        {
            child.Parent = this;
            mChildren.Add(child);
        }

        public void RemoveChild(ObjectItem child)
        {
            if (mChildren.Contains(child))
            {
                mChildren.Remove(child);
            }
        }

        public void ClearChildren()
        {
            mChildren.Clear();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
