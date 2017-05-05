using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace UMP.MAMT.CreateDatabaseObject
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
            string LStrImageFolder = string.Format("Style{0}", App.GStrSeasonCode);
            string path = App.GStrApplicationDirectory.Substring(0, App.GStrApplicationDirectory.LastIndexOf("\\"));
            LStrImageFolder = System.IO.Path.Combine(path, @"Themes", LStrImageFolder, @"Images\S0001");

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

            VStrObjectName = ADataRowObject["C001"].ToString();
            VStrObjectType = App.GetDisplayCharater("TYPE-" + ADataRowObject["C002"].ToString());
            VStrObjectVersion = ADataRowObject["C004"].ToString();
            VStrStatusDesc = AStrStatusDesc;
            if (VStrObjectName.Substring(0, 1).ToUpper() == "V" && AActionIcoPath == "0")
            {
                VActionIcoPath = System.IO.Path.Combine(LStrImageFolder, "S0001010.png");
                BoolIsSuccess = true;
                VStrObjectName = ADataRowObject["C001"].ToString();
                VStrObjectType = App.GetDisplayCharater("TYPE-" + ADataRowObject["C002"].ToString());
                VStrObjectVersion = ADataRowObject["C004"].ToString();
                VStrStatusDesc = string.Empty; ;
            }
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
            string LStrImageFolder = string.Format("Style{0}", App.GStrSeasonCode);
            string path = App.GStrApplicationDirectory.Substring(0, App.GStrApplicationDirectory.LastIndexOf("\\"));
            LStrImageFolder = System.IO.Path.Combine(path, @"Themes", LStrImageFolder, @"Images\S0001");
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
