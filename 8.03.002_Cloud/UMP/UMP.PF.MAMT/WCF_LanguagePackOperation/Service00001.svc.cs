using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.ServiceModel.Activation;

namespace WCF_LanguagePackOperation
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service00001 : IService00001
    {
        /// <summary>
        /// 入口函数
        /// AIntOperationID的值如下
        /// 1：获得已经支持的UMP的语种列表
        /// 2：获得所有的语言
        /// 3：更新语言
        /// 4：根据languageCode删除一种类型的语言
        /// 5：插入一行记录
        /// 6：获得已经支持和未支持的所有语种列表
        /// 7：设置languagecode为已经支持的语种
        /// 8：试图连接到数据库服务器
        /// 9:获得要创建的表、函数、存储过程
        /// 10:创建数据库对象
        /// 11:获得某个语种的语言
        /// </summary>
        /// <param name="AIntOperationID"></param>
        /// <param name="AListStringArgs"></param>
        /// <returns></returns>
        public ReturnResult OperationMethodA(int AIntOperationID, List<string> AListStringArgs)
        {
            ReturnResult LOperationReturn = new ReturnResult();
            try
            {
                if (AIntOperationID == 1) { LOperationReturn = OperationA01(AListStringArgs); }
                if (AIntOperationID == 2) { LOperationReturn = OperationA02(AListStringArgs); }
                if (AIntOperationID == 3) { LOperationReturn = OperationA03(AListStringArgs); }
                if (AIntOperationID == 4) { LOperationReturn = OperationA04(AListStringArgs); }
                if (AIntOperationID == 5) { LOperationReturn = OperationA05(AListStringArgs); }
                if (AIntOperationID == 6) { LOperationReturn = OperationA06(AListStringArgs); }
                if (AIntOperationID == 7) { LOperationReturn = OperationA07(AListStringArgs); }
                if (AIntOperationID == 8) { LOperationReturn = OperationA08(AListStringArgs); }
                if (AIntOperationID == 9) { LOperationReturn = OperationA09(AListStringArgs); }
                if (AIntOperationID == 10) { LOperationReturn = OperationA10(AListStringArgs); }
                if (AIntOperationID == 11) { LOperationReturn = OperationA11(AListStringArgs); }
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }
            return LOperationReturn;
        }

        /// <summary>
        /// 获得已经支持的UMP的语种列表
        /// </summary>
        /// <param name="lstArgs">list中的顺序如下</param>
        /// 0：数据库类型
        /// 1：serverhost
        /// 2：serverport
        /// 3：servicename/dbname
        /// 4：loginname
        /// 5：loginpwd
        /// <returns></returns>
        private ReturnResult OperationA01(List<string> lstArgs)
        {
            ReturnResult result = new ReturnResult();
            switch (lstArgs[0])
            {
                case "3":   //Oracle
                    result = DBConnectInOracle.GetLanguageTypesSupported(lstArgs[1], lstArgs[2], lstArgs[3], lstArgs[4], lstArgs[5]);
                    break;
                case "2":
                    result = DBConnectInMsSql.GetLanguageTypesSupported(lstArgs[1], lstArgs[2], lstArgs[3], lstArgs[4], lstArgs[5]);
                    break;
            }
            return result;
        }

        /// <summary>
        /// 获得所有的语言
        /// </summary>
        /// <param name="lstArgs"></param>
        /// <returns></returns>
        private ReturnResult OperationA02(List<string> lstArgs)
        {
            ReturnResult result = new ReturnResult();
            switch (lstArgs[0])
            {
                case "3":   //Oracle
                    result = DBConnectInOracle.GetAllLanguage(lstArgs[1], lstArgs[2], lstArgs[3], lstArgs[4], lstArgs[5]);
                    break;
                case "2":
                    result = DBConnectInMsSql.GetAllLanguage(lstArgs[1], lstArgs[2], lstArgs[3], lstArgs[4], lstArgs[5]);
                    break;
            }
            return result;
        }

        /// <summary>
        /// 更新语言 
        /// </summary>
        /// <param name="lstArgs">参数顺序</param>
        /// 0：数据库类型
        /// 1：serverhost
        /// 2：serverport
        /// 3：servicename/dbname
        /// 4：loginname
        /// 5：loginpwd
        /// 6: LanguageCode
        /// 7: MessageId
        /// 8: Display1
        /// 9: Display2
        /// 10: Tip1
        /// 11: Tip2
        /// <returns></returns>
        private ReturnResult OperationA03(List<string> lstArgs)
        {
            ReturnResult result = new ReturnResult();
            switch (lstArgs[0])
            {
                case "3":   //Oracle
                    result = DBConnectInOracle.UpdateLanguageItem(lstArgs[1], lstArgs[2], lstArgs[3], lstArgs[4]
                        , lstArgs[5], lstArgs[6], lstArgs[7], lstArgs[8], lstArgs[9], lstArgs[10], lstArgs[11]);
                    break;
                case "2":
                     result = DBConnectInMsSql.UpdateLanguageItem(lstArgs[1], lstArgs[2], lstArgs[3], lstArgs[4]
                        , lstArgs[5], lstArgs[6], lstArgs[7], lstArgs[8], lstArgs[9], lstArgs[10], lstArgs[11]);
                    break;
            }
            return result;
        }

        /// <summary>
        /// 根据languageCode删除一种类型的语言
        /// 0：数据库类型
        /// 1：serverhost
        /// 2：serverport
        /// 3：servicename/dbname
        /// 4：loginname
        /// 5：loginpwd
        /// 6: LanguageCode
        /// </summary>
        /// <param name="lstArgs">参数顺序</param>
        /// <returns></returns>
        private ReturnResult OperationA04(List<string> lstArgs)
        {
            ReturnResult result = new ReturnResult();
            switch (lstArgs[0])
            {
                case "3":   //Oracle
                    result = DBConnectInOracle.RemoveLanguagesByLanguageCode(lstArgs[1], lstArgs[2], lstArgs[3], lstArgs[4], lstArgs[5], lstArgs[6]);
                    break;
                case "2":
                    result = DBConnectInMsSql.RemoveLanguagesByLanguageCode(lstArgs[1], lstArgs[2], lstArgs[3], lstArgs[4], lstArgs[5], lstArgs[6]);
                    break;
            }
            return result;
        }

        /// <summary>
        /// 插入一行记录
        /// 0：数据库类型
        /// 1：serverhost
        /// 2：serverport
        /// 3：servicename/dbname
        /// 4：loginname
        /// 5：loginpwd
        /// 6: LanguageCode
        /// 7: MessageID
        /// 8: Dis1
        /// 9: Dis2
        /// 10: Tip1
        /// 11: Tip2
        /// 12: ModuleID
        /// 13: ChildModuleID
        /// 14: Frame
        /// </summary>
        /// <param name="lstArgs"></param>
        /// <returns></returns>
        private ReturnResult OperationA05(List<string> lstArgs)
        {
            ReturnResult result = new ReturnResult();
            switch (lstArgs[0])
            {
                case "3":   //Oracle
                    result = DBConnectInOracle.Insert(lstArgs[1], lstArgs[2], lstArgs[3], lstArgs[4], lstArgs[5],
                         lstArgs[6], lstArgs[7], lstArgs[8], lstArgs[9], lstArgs[10], lstArgs[11], lstArgs[12], lstArgs[13], lstArgs[14]);
                    break;
                case "2":
                    result = DBConnectInMsSql.Insert(lstArgs[1], lstArgs[2], lstArgs[3], lstArgs[4], lstArgs[5],
                         lstArgs[6], lstArgs[7], lstArgs[8], lstArgs[9], lstArgs[10], lstArgs[11], lstArgs[12], lstArgs[13], lstArgs[14]);
                    break;
            }
            return result;
        }

        /// <summary>
        /// 获得已经支持和未支持的所有语种列表
        /// </summary>
        /// <param name="lstArgs">list中的顺序如下</param>
        /// 0：数据库类型
        /// 1：serverhost
        /// 2：serverport
        /// 3：servicename/dbname
        /// 4：loginname
        /// 5：loginpwd
        /// <returns></returns>
        private ReturnResult OperationA06(List<string> lstArgs)
        {
            ReturnResult result = new ReturnResult();
            switch (lstArgs[0])
            {
                case "3":   //Oracle
                    result = DBConnectInOracle.GetLanguageTypes(lstArgs[1], lstArgs[2], lstArgs[3], lstArgs[4], lstArgs[5]);
                    break;
                case "2":
                    result = DBConnectInMsSql.GetLanguageTypes(lstArgs[1], lstArgs[2], lstArgs[3], lstArgs[4], lstArgs[5]);
                    break;
            }
            return result;
        }

        /// <summary>
        /// 设置languagecode为已经支持的语种
        /// <param name="lstArgs">list中的顺序如下</param>
        /// 0：数据库类型
        /// 1：serverhost
        /// 2：serverport
        /// 3：servicename/dbname
        /// 4：loginname
        /// 5：loginpwd
        /// </summary>
        /// <param name="lstArgs"></param>
        /// <returns></returns>
        private ReturnResult OperationA07(List<string> lstArgs)
        {
            ReturnResult result = new ReturnResult();
            switch (lstArgs[0])
            {
                case "3":   //Oracle
                    result = DBConnectInOracle.SupportLanguage(lstArgs[1], lstArgs[2], lstArgs[3], lstArgs[4], lstArgs[5], lstArgs[6]);
                    break;
                case "2":
                    result = DBConnectInMsSql.SupportLanguage(lstArgs[1], lstArgs[2], lstArgs[3], lstArgs[4], lstArgs[5], lstArgs[6]);
                    break;
            }
            return result;
        }

        /// <summary>
        /// 尝试连接到数据库服务器
        /// </summary>
        /// <param name="lstArgs"></param>
        /// <returns></returns>
        public ReturnResult OperationA08(List<string> lstArgs)
        {
            ReturnResult result = new ReturnResult();
            switch (lstArgs[0])
            {
                case "3":   //Oracle
                    result = DBConnectInOracle.ConnectToServer(lstArgs[1], lstArgs[2], lstArgs[3], lstArgs[4], lstArgs[5]);
                    break;
            }
            return result;
        }

        /// <summary>
        /// 获得要创建的表、函数、存储过程
        /// </summary>
        /// <param name="lstArgs"></param>
        /// 1:数据库类型
        /// <returns></returns>
        public ReturnResult OperationA09(List<string> lstArgs)
        {
            ReturnResult result = new ReturnResult();
            switch (lstArgs[0])
            {
                case "3":   //Oracle
                    result = DBConnectInOracle.GetAllDBObjects();
                    break;
            }
            return result;
        }

        /// <summary>
        /// 创建数据库对象
        /// </summary>
        /// <param name="lstArgs"></param>
        /// 1：数据库类型
        /// 2：数据库对象名
        /// 3：数据库对象类型
        /// <returns></returns>
        public ReturnResult OperationA10(List<string> lstArgs)
        {
            ReturnResult result = new ReturnResult();
            switch (lstArgs[0])
            {
                case "3":   //Oracle
                    result = DBConnectInOracle.GetAllDBObjects();
                    break;
            }
            return result;
        }
        public ReturnResult OperationA11(List<string> lstArgs)
        {
            ReturnResult result = new ReturnResult();
            switch (lstArgs[0])
            {
                case "3":   //Oracle
                    result = DBConnectInOracle.GetSingleLanguage(lstArgs[1], lstArgs[2], lstArgs[3], lstArgs[4], lstArgs[5], lstArgs[6]);
                    break;
                case "2":
                    result =DBConnectInMsSql.GetSingleLanguage(lstArgs[1], lstArgs[2], lstArgs[3], lstArgs[4], lstArgs[5], lstArgs[6]);
                    break;
            }
            return result;
        }
    }
}
