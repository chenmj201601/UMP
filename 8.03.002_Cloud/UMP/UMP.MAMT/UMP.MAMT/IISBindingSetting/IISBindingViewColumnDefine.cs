using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMP.MAMT.IISBindingSetting
{
    public class IISBindingViewColumnDefine
    {
        public string BindingProtolIco { get; set; }
        public string BindingProtolName { get; set; }
        public string OpendServerName { get; set; }
        public string OpendServerPort { get; set; }
        public string IsUsed { get; set; }

        public IISBindingViewColumnDefine(string AStrProtolName, string AStrServerName, string AStrServerPort, string AStrIsUsed)
        {
            BindingProtolIco = System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000022.ico");
            BindingProtolName = AStrProtolName;
            OpendServerName = AStrServerName;
            OpendServerPort = AStrServerPort;
            IsUsed = App.GetConvertedData("IISIsUsed" + AStrIsUsed);
        }
    }
}
