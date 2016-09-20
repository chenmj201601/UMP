using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Resources;
using System.Configuration;
using System.IO;

namespace UMP.PF.MAMT.Classes
{
    public class AboutLanguage
    {
        /// <summary>
        /// 根据默认语言加载语言包
        /// </summary>
        public static void LoadDefaultLanguage()
        {
            //获得默认语言
           App.GStrDefaultLanguage = ConfigurationManager.AppSettings["DefaultLanguage"];
           App.GStrCurrentLanguage = App.GStrDefaultLanguage;
           string languagefileName = App.GStrApplicationDirectory + "\\Languages\\" + App.GStrDefaultLanguage + ".xaml";
            Application.Current.Resources.MergedDictionaries[1] = new ResourceDictionary() { Source = new Uri(languagefileName, UriKind.RelativeOrAbsolute) };
        }

        /// <summary>
        /// 获得语言列表
        /// </summary>
        public static void GetLanguageList()
        {
            DirectoryInfo dir = new DirectoryInfo(App.GStrApplicationDirectory + "\\Languages");
            List<FileInfo> lstLanFiles = dir.GetFiles().ToList();
            string LStrFileName = string.Empty;
            for (int i = 0; i < lstLanFiles.Count; i++)
            {
                LStrFileName = lstLanFiles[i].Name;
                App.GLstLanguages.Add(LStrFileName.Substring(0, LStrFileName.LastIndexOf('.')));
            }
        }

        public static void ChangeLanguage(string strLanguage)
        {
            ConfigurationManager.AppSettings["DefaultLanguage"] = strLanguage;
            App.GStrCurrentLanguage = strLanguage;
            LoadDefaultLanguage();
        }
    }
}
