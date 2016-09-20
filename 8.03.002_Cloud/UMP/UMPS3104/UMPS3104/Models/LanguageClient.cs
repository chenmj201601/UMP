using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPS3104.Models
{
    public class LanguageClient
    {
        public static List<string> Langs(int LangID)
        {
            List<string> langs=new List<string>();
            switch (LangID)
            {
                case 1028://繁体
                    langs.Add("強制登錄");
                    langs.Add("皮膚");
                    break;
                case 1033://U.S
                    langs.Add("Force Log");
                    langs.Add("Client Style");
                    break;
                case 1041://日语
                    langs.Add("強制登録");
                    langs.Add("皮膚");
                    break;
                case 2052://简体
                    langs.Add("强制登录");
                    langs.Add("皮肤");
                    break;
                default:
                    langs = null;
                    break;
            }
            return langs;
        }
    }
}
