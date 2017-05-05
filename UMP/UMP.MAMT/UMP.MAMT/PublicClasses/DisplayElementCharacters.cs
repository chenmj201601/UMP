using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace UMP.MAMT.PublicClasses
{
    public static class DisplayElementObjectCharacters
    {
        /// <summary>
        /// 从语言包中获取显示字符
        /// </summary>
        /// <param name="AObjectTarget">要显示的对象，目前支持TreeView，ListView, ComboBox</param>
        /// <param name="AVisualSource">在语言包中定义的Tag(string类型)，如 "UUCServerObjectBasic", "L0001"</param>
        public static void DisplayUIObjectCharacters(object AObjectTarget, Visual AVisualSource)
        {
            try
            {
                Type LTypeObject = AObjectTarget.GetType();
                Type LTypeVisual = AVisualSource.GetType();
                PropertyInfo LPropertyInfoVisual = LTypeVisual.GetProperty("Tag");
                if (LPropertyInfoVisual != null)
                {
                    string LStrVisualTag = (string)LPropertyInfoVisual.GetValue(AVisualSource, null); //取值
                    if (string.IsNullOrEmpty(LStrVisualTag)) { return; }

                    //if (LTypeObject == typeof(TreeView)) { DisplayObjectCharactersTreeView((TreeView)AObjectTarget, LStrVisualTag); }
                    if (LTypeObject == typeof(ListView)) { DislpayObjectCharactersListView((ListView)AObjectTarget, LStrVisualTag); }
                    //if (LTypeObject == typeof(ComboBox)) { DisplayObjectCharactersComboBox((ComboBox)AObjectTarget, LStrVisualTag); }
                }
            }
            catch { }
        }

        /// <summary>
        /// 加载ListViewColumnHeader语言包
        /// </summary>
        /// <param name="AStrListViewTag">ListView.Tag</param>
        /// <returns>DataTable</returns>
        private static DataTable LoadListViewColumnHeaderLanguage(string AStrListViewTag)
        {
            DataTable LDataTable = new DataTable();
            string LStrListViewHeaderXmlFile = string.Empty;

            try
            {
                LStrListViewHeaderXmlFile = System.IO.Path.Combine(App.GStrApplicationDirectory, @"Languages\L" + App.GStrLoginUserCurrentLanguageID + "-" + AStrListViewTag + ".xml");
                DataSet LDataSetTemp = new DataSet();
                LDataSetTemp.ReadXml(LStrListViewHeaderXmlFile);
                LDataTable = LDataSetTemp.Tables[0];
            }
            catch { LDataTable = null; }
            return LDataTable;
        }

        private static void DislpayObjectCharactersListView(ListView AListView, string AStrSystemTag)
        {
            string LStrHeaderContent = string.Empty;
            string LStrHeaderName = string.Empty;

            try
            {
                DataTable LDataTableHeaderLanguage = LoadListViewColumnHeaderLanguage(AStrSystemTag);
                if (LDataTableHeaderLanguage == null) { return; }
                GridView LGridView = (GridView)AListView.View;
                foreach (GridViewColumn LGridViewColumn in LGridView.Columns)
                {
                    GridViewColumnHeader LGridViewColumnHeader = (GridViewColumnHeader)LGridViewColumn.Header;
                    LStrHeaderName = LGridViewColumnHeader.Name;
                    try
                    {
                        LStrHeaderContent = LDataTableHeaderLanguage.Select("HeaderName = '" + LStrHeaderName + "'").FirstOrDefault().Field<string>(1);
                    }
                    catch { LStrHeaderContent = string.Empty; }
                    if (string.IsNullOrEmpty(LStrHeaderContent)) { continue; }
                    LGridViewColumnHeader.Content = " " + LStrHeaderContent;
                }

                return;
            }
            catch { }
        }
    }
}
