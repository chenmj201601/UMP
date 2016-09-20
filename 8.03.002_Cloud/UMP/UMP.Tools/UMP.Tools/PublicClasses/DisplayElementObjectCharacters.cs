using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace UMP.Tools.PublicClasses
{
    public static class DisplayElementObjectCharacters
    {
        /// <summary>
        /// 从语言包中获取显示字符
        /// </summary>
        /// <param name="AObjectTarget">要显示的对象，目前支持ListView</param>
        /// <param name="AVisualSource">在语言包中定义的Tag(string类型)，如 "UUCServerObjectBasic", "L001"</param>
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

                    if (LTypeObject == typeof(ListView)) { DislpayObjectCharactersListView((ListView)AObjectTarget, LStrVisualTag); }
                }
            }
            catch { }
        }


        private static void DislpayObjectCharactersListView(ListView AListView, string AStrSystemTag)
        {
            string LStrListViewTag = string.Empty;
            string LStrHeaderContent = string.Empty;
            string LStrHeaderTag = string.Empty;

            try
            {
                LStrListViewTag = AListView.Tag.ToString();
                GridView LGridView = (GridView)AListView.View;
                foreach (GridViewColumn LGridViewColumn in LGridView.Columns)
                {
                    GridViewColumnHeader LGridViewColumnHeader = (GridViewColumnHeader)LGridViewColumn.Header;
                    LStrHeaderTag = LGridViewColumnHeader.Tag.ToString();
                    LStrHeaderContent = App.GetDisplayCharater(LStrListViewTag + LStrHeaderTag);
                    if (string.IsNullOrEmpty(LStrHeaderContent)) { continue; }
                    LGridViewColumnHeader.Content = " " + LStrHeaderContent;
                }

                return;
            }
            catch { }
        }
    }
}
