using System;
using System.Collections.Generic;
using System.Data;
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
using UMP.MAMT.PublicClasses;

namespace UMP.MAMT.BasicControls
{
    public partial class UCObjectOperations : UserControl, MamtOperationsInterface
    {
        public event EventHandler<MamtOperationEventArgs> IOperationEvent;

        //目前存在的ObjectOperations列表
        private List<bool> IListBoolClear = new List<bool>();
        private List<string> IListStrObjectID = new List<string>();
        private List<object> IListObjectSource = new List<object>();
        private List<string> IListStrAddType = new List<string>();

        public UCObjectOperations()
        {
            InitializeComponent();
            this.Loaded += UCObjectOperations_Loaded;
            ImageCloseOperations.MouseLeftButtonDown += ImageCloseOperations_MouseLeftButtonDown;
        }

        private void ImageCloseOperations_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IOperationEvent != null)
            {
                MamtOperationEventArgs LEventArgs = new MamtOperationEventArgs();
                LEventArgs.StrElementTag = ((Image)sender).Tag.ToString();
                LEventArgs.ObjSource = (Image)sender;
                IOperationEvent(this, LEventArgs);
            }
        }

        private void UCObjectOperations_Loaded(object sender, RoutedEventArgs e)
        {
            ImageOperations.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000017.ico"), UriKind.RelativeOrAbsolute));
            ImageCloseOperations.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000007.ico"), UriKind.RelativeOrAbsolute));
            DisplayElementCharacters(false);
        }

        public void DisplayElementCharacters(bool ABoolLanguageChange)
        {
            LabelOperationsTitle.Content = App.GetDisplayCharater("M01013");
            ImageCloseOperations.ToolTip = App.GetDisplayCharater("M01006");
            ApplicationUILanguageChanged();
        }

        private void ApplicationUILanguageChanged()
        {
            try
            {
                StackPanelObjectOperations.Children.Clear();
                for (int LIntLoopObjectOperations = 0; LIntLoopObjectOperations < IListBoolClear.Count; LIntLoopObjectOperations++)
                {
                    if (IListStrAddType[LIntLoopObjectOperations] == "Add")
                    {
                        AddObjectOperations(IListBoolClear[LIntLoopObjectOperations], IListStrObjectID[LIntLoopObjectOperations], IListObjectSource[LIntLoopObjectOperations], true);
                    }
                    else
                    {
                        AppendObjectOperations(IListStrObjectID[LIntLoopObjectOperations], IListObjectSource[LIntLoopObjectOperations], true);
                    }
                }
            }
            catch { }
        }

        private DataTable LoadOperationsFromXmlFiles(string AStrObjectID)
        {
            string LStrOperationsXmlFile = string.Empty;
            DataTable LDataTableOperationsLoaded = new DataTable();

            try
            {
                LStrOperationsXmlFile = System.IO.Path.Combine(App.GStrApplicationDirectory, @"Languages\O" + App.GStrLoginUserCurrentLanguageID + "-" + AStrObjectID + ".xml");
                DataSet LDataSetTemp = new DataSet();
                LDataSetTemp.ReadXml(LStrOperationsXmlFile);
                LDataTableOperationsLoaded = LDataSetTemp.Tables[0];
            }
            catch { LDataTableOperationsLoaded = null; }
            return LDataTableOperationsLoaded;
        }

        public void AddObjectOperations(bool ABoolClearAll, string AStrObjectID, object AObjectSource, bool ABoolChangeLanguage)
        {
            try
            {
                if (ABoolClearAll && !ABoolChangeLanguage)
                {
                    StackPanelObjectOperations.Children.Clear();
                    IListBoolClear.Clear(); IListStrObjectID.Clear(); IListObjectSource.Clear(); IListStrAddType.Clear();
                }
                if (!ABoolChangeLanguage) { IListBoolClear.Add(ABoolClearAll); IListStrObjectID.Add(AStrObjectID); IListObjectSource.Add(AObjectSource); IListStrAddType.Add("Add"); }

                DataTable LDataTableOperationsLoaded = new DataTable();
                LDataTableOperationsLoaded = LoadOperationsFromXmlFiles(AStrObjectID);
                UCGroupOperations LUCGroupOperations = new UCGroupOperations();
                LUCGroupOperations.IOperationEvent += LUCAllOperationsPanel_IOperationEvent;
                LUCGroupOperations.ShowObjectAllOperations(LDataTableOperationsLoaded, AObjectSource, "O");
                StackPanelObjectOperations.Children.Add(LUCGroupOperations);
            }
            catch { }
        }

        public void AppendObjectOperations(string AStrObjectID, object AObjectSource, bool ABoolChangeLanguage)
        {
            try
            {
                if (!ABoolChangeLanguage)
                {
                    if (StackPanelObjectOperations.Children.Count > 1)
                    {
                        StackPanelObjectOperations.Children.RemoveAt(1);
                        IListBoolClear.RemoveAt(1); IListStrObjectID.RemoveAt(1); IListObjectSource.RemoveAt(1); IListStrAddType.RemoveAt(1);
                    }
                    IListBoolClear.Add(false); IListStrObjectID.Add(AStrObjectID); IListObjectSource.Add(AObjectSource); IListStrAddType.Add("Append");
                }
                DataTable LDataTableOperationsLoaded = new DataTable();
                LDataTableOperationsLoaded = LoadOperationsFromXmlFiles(AStrObjectID);
                UCGroupOperations LUCGroupOperations = new UCGroupOperations();
                LUCGroupOperations.Margin = new Thickness(0, 2, 0, 0);
                LUCGroupOperations.IOperationEvent += LUCAllOperationsPanel_IOperationEvent;
                LUCGroupOperations.ShowObjectAllOperations(LDataTableOperationsLoaded, AObjectSource, "B");
                StackPanelObjectOperations.Children.Add(LUCGroupOperations);
            }
            catch { }
        }

        private void LUCAllOperationsPanel_IOperationEvent(object sender, MamtOperationEventArgs e)
        {
            if (IOperationEvent != null) { IOperationEvent(this, e); }
        }
    }
}
