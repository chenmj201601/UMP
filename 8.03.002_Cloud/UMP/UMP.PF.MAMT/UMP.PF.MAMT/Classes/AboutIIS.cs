using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.Administration;

namespace UMP.PF.MAMT.Classes
{
    public class AboutIIS
    {
        /// <summary>
        /// 检查是否存在UMP.PF站点 
        /// </summary>
        /// <returns></returns>
        public static bool VerifyWebSiteIsExist()
        {
            using (ServerManager mgr = new ServerManager())
            {
                for (int i = 0; i < mgr.Sites.Count; i++)
                {
                    if (mgr.Sites[i].Name.ToUpper().Equals("UMP.PF"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

    }
}
