using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace UMPS0001
{
    public class CreateObjectColumnDefine
    {
        public string VActionIcoPath { get; set; }
        public string VStrObjectName { get; set; }
        public string VStrObjectType { get; set; }
        public string VStrObjectVersion { get; set; }
        public string VStrStatusDesc { get; set; }
        public bool BoolIsSuccess { get; set; }

        public CreateObjectColumnDefine(string AActionIcoPath, string AStrStatusDesc, DataRow ADataRowObject)
        {
            string LStrImageFolder = System.IO.Path.Combine(App.GClassSessionInfo.LocalMachineInfo.StrCommonApplicationData, @"UMP.Client\Themes", App.GClassSessionInfo.ThemeInfo.Name, @"Images\S0001");

            if(AActionIcoPath == "1")
            {
                VActionIcoPath = System.IO.Path.Combine(LStrImageFolder, "S0001010.png");
                BoolIsSuccess = true;
            }
            else if(AActionIcoPath == "2")
            {
                VActionIcoPath = System.IO.Path.Combine(LStrImageFolder, "S0001009.png");
            }
            else if (AActionIcoPath == "0")
            {
                VActionIcoPath = System.IO.Path.Combine(LStrImageFolder, "S0001011.png");
                BoolIsSuccess = false;
            }
            else if (string.IsNullOrEmpty(AActionIcoPath))
            {
                VActionIcoPath = string.Empty;
                BoolIsSuccess = false;
            }

            VStrObjectName = ADataRowObject["C001"].ToString();
            VStrObjectType = App.GetDisplayCharater("TYPE-" + ADataRowObject["C002"].ToString());
            VStrObjectVersion = ADataRowObject["C004"].ToString();
            VStrStatusDesc = AStrStatusDesc;
        }
    }

    public class InitObjectColumnDefine
    {
        public string VActionIcoPath { get; set; }
        public string VStrTableName { get; set; }
        public string VStrObjectVersion { get; set; }
        public string VStrEffectRows { get; set; }
        public string VStrStatusDesc { get; set; }
        public bool BoolIsSuccess { get; set; }

        public InitObjectColumnDefine(string AActionIcoPath, string AStrStatusDesc, DataRow ADataRowObject, string AStrEffectRows)
        {
            string LStrImageFolder = System.IO.Path.Combine(App.GClassSessionInfo.LocalMachineInfo.StrCommonApplicationData, @"UMP.Client\Themes", App.GClassSessionInfo.ThemeInfo.Name, @"Images\S0001");
            if (AActionIcoPath == "1")
            {
                VActionIcoPath = System.IO.Path.Combine(LStrImageFolder, "S0001010.png");
                BoolIsSuccess = true;
            }
            else if (AActionIcoPath == "2")
            {
                VActionIcoPath = System.IO.Path.Combine(LStrImageFolder, "S0001009.png");
            }
            else if (AActionIcoPath == "0")
            {
                VActionIcoPath = System.IO.Path.Combine(LStrImageFolder, "S0001011.png");
                BoolIsSuccess = false;
            }
            else if (string.IsNullOrEmpty(AActionIcoPath))
            {
                VActionIcoPath = string.Empty;
                BoolIsSuccess = false;
            }

            VStrTableName = ADataRowObject["C001"].ToString();
            VStrObjectVersion = ADataRowObject["C003"].ToString();
            VStrEffectRows = AStrEffectRows;
            VStrStatusDesc = AStrStatusDesc;
        }
    }
}
