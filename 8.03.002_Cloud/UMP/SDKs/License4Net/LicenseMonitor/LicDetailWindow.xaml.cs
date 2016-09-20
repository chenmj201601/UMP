//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    949513ec-32cc-4446-8f2d-80fd79c442ae
//        CLR Version:              4.0.30319.18408
//        Name:                     LicDetailWindow
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                LicenseMonitor
//        File Name:                LicDetailWindow
//
//        created by Charley at 2016/4/26 12:33:31
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;

namespace LicenseMonitor
{
    /// <summary>
    /// LicDetailWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LicDetailWindow
    {
        public PropertyViewItem LicViewItem;
        public IList<PropertyViewItem> ListAllLicItems;

        private LicModelItem mRootLicItem;
        private List<LicModel> mListLicModels;
        private List<LicModelItem> mListLicItems;

        public LicDetailWindow()
        {
            InitializeComponent();

            mRootLicItem = new LicModelItem();
            mListLicModels = new List<LicModel>();
            mListLicItems = new List<LicModelItem>();

            Loaded += LicDetailWindow_Loaded;
        }

        void LicDetailWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                TreeLicDetail.ItemsSource = mRootLicItem.Children;

                if (LicViewItem != null)
                {
                    Title = LicViewItem.Name;
                }

                InitColumns();
                LoadLicModels();

                mListLicItems.Clear();
                mRootLicItem.Children.Clear();
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    InitLicModelItems();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void InitColumns()
        {
            try
            {
                GridViewColumnHeader nameHeader = new GridViewColumnHeader();
                nameHeader.Content = "Name";
                int nameColumnWidth = 280;
                List<GridViewColumn> listColumns = new List<GridViewColumn>();
                GridViewColumn gvc = new GridViewColumn();
                GridViewColumnHeader gvch = new GridViewColumnHeader();
                gvch.Content = "Licence ID";
                gvc.Header = gvch;
                gvc.Width = 120;
                gvc.DisplayMemberBinding = new Binding("LicID");
                listColumns.Add(gvc);
                gvc = new GridViewColumn();
                gvch = new GridViewColumnHeader();
                gvch.Content = "Value";
                gvc.Header = gvch;
                gvc.Width = 80;
                gvc.DisplayMemberBinding = new Binding("StrValue");
                listColumns.Add(gvc);
                gvc = new GridViewColumn();
                gvch = new GridViewColumnHeader();
                gvch.Content = "Licence Type";
                gvc.Header = gvch;
                gvc.Width = 80;
                gvc.DisplayMemberBinding = new Binding("StrLicType");
                listColumns.Add(gvc);
                gvc = new GridViewColumn();
                gvch = new GridViewColumnHeader();
                gvch.Content = "Expire Time";
                gvc.Header = gvch;
                gvc.Width = 180;
                gvc.DisplayMemberBinding = new Binding("StrExpireTime");
                listColumns.Add(gvc);
                TreeLicDetail.SetColumns(nameHeader, nameColumnWidth, listColumns);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void LoadLicModels()
        {
            try
            {
                mListLicModels.Clear();
                string strPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    string.Format("LicDefine_{0}.xml", MainWindow.LIC_ID_UMP_01));
                if (!File.Exists(strPath))
                {
                    ShowErrorMessage(string.Format("File not exist.\t{0}", strPath));
                    return;
                }
                XmlDocument doc = new XmlDocument();
                doc.Load(strPath);
                var licsNode = doc.SelectSingleNode("LicCollection/Lics");
                if (licsNode == null)
                {
                    ShowErrorMessage(string.Format("Lics node not exist!"));
                    return;
                }
                for (int i = 0; i < licsNode.ChildNodes.Count; i++)
                {
                    var licEle = licsNode.ChildNodes[i] as XmlElement;
                    if (licEle == null) { continue; }
                    LicModel licModel = new LicModel();
                    licModel.MasterID = Convert.ToInt32(licEle.GetAttribute("MasterID"));
                    licModel.OptID = Convert.ToInt64(licEle.GetAttribute("OptID"));
                    licModel.ParentID = Convert.ToInt64(licEle.GetAttribute("ParentID"));
                    licModel.SortID = Convert.ToInt32(licEle.GetAttribute("SortID"));
                    licModel.LicNo = Convert.ToInt32(licEle.GetAttribute("LicNo"));
                    licModel.LicID = Convert.ToInt64(licEle.GetAttribute("LicID"));
                    licModel.OptName = licEle.GetAttribute("OptName");
                    mListLicModels.Add(licModel);
                }
                mListLicModels = mListLicModels.OrderBy(l => l.LicNo).ToList();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void InitLicModelItems()
        {
            try
            {
                if (LicViewItem == null) { return; }
                string strValue = LicViewItem.Value;
                int licNo;
                long licID;
                string strTag;
                for (int i = 0; i < mListLicModels.Count; i++)
                {
                    var model = mListLicModels[i];
                    var temp = mListLicItems.FirstOrDefault(l => l.LicNo == model.LicNo);
                    if (temp == null)
                    {
                        temp = new LicModelItem();
                        temp.Name = string.Format("[{0}]{1}", model.LicNo, model.OptName);
                        temp.LicNo = model.LicNo;
                        temp.LicID = model.LicID;
                        temp.StrLicType = LicViewItem.LicType;

                        licNo = temp.LicNo;
                        if (strValue.Length > licNo)
                        {
                            strTag = strValue.Substring(licNo, 1);
                        }
                        else
                        {
                            strTag = "";
                        }

                        temp.StrValue = strTag;
                        temp.StrExpireTime = LicViewItem.Expiration;

                        if (strTag.ToUpper().Equals("T"))
                        {
                            licID = temp.LicID;
                            if (ListAllLicItems != null)
                            {
                                var specialItem = ListAllLicItems.FirstOrDefault(s => s.SerialNo == licID.ToString());
                                if (specialItem != null)
                                {
                                    temp.StrExpireTime = specialItem.Expiration;
                                }
                            }
                        }
                      
                        var list = mListLicModels.Where(l => l.ParentID == model.OptID).ToList();
                        for (int j = 0; j < list.Count; j++)
                        {
                            var child = list[j];
                            var temp2 = mListLicItems.FirstOrDefault(l => l.LicNo == child.LicNo);
                            if (temp2 == null)
                            {
                                temp2 = new LicModelItem();
                                temp2.Name = string.Format("[{0}]{1}", child.LicNo, child.OptName);
                                temp2.LicNo = child.LicNo;
                                temp2.LicID = child.LicID;
                                temp2.StrLicType = LicViewItem.LicType;

                                licNo = temp2.LicNo;
                                if (strValue.Length > licNo)
                                {
                                    strTag = strValue.Substring(licNo, 1);
                                }
                                else
                                {
                                    strTag = "";
                                }

                                temp2.StrValue = strTag;
                                temp2.StrExpireTime = LicViewItem.Expiration;

                                if (strTag.ToUpper().Equals("T"))
                                {
                                    licID = temp2.LicID;
                                    if (ListAllLicItems != null)
                                    {
                                        var specialItem = ListAllLicItems.FirstOrDefault(s => s.SerialNo == licID.ToString());
                                        if (specialItem != null)
                                        {
                                            temp2.StrExpireTime = specialItem.Expiration;
                                        }
                                    }
                                }
                              
                                AddChild(temp, temp2);
                                mListLicItems.Add(temp2);
                            }
                        }
                        AddChild(mRootLicItem, temp);
                        mListLicItems.Add(temp);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void AddChild(LicModelItem parent, LicModelItem child)
        {
            Dispatcher.Invoke(new Action(() => parent.AddChild(child)));
        }

        private void ShowErrorMessage(string msg)
        {
            MessageBox.Show(string.Format("{0}", msg), "License Monitor", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
