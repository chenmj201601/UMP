using PFShareClasses01;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

namespace Wcf11111
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service11111 : IService11111
    {
        public OperationDataArgs11111 OperationMethodA(int AIntOperationID, List<string> AListStringArgs)
        {
            OperationDataArgs11111 LOperationReturn = new OperationDataArgs11111();

            try
            {
                if (AIntOperationID == 0) { LOperationReturn = OperationA00(AListStringArgs); }
                if (AIntOperationID == 1) { LOperationReturn = OperationA01(AListStringArgs); }
                if (AIntOperationID == 2) { LOperationReturn = OperationA02(AListStringArgs); }
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }

        /// <summary>
        /// 获取所有资源类型属性定义列表
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：数据库类型
        /// 1：数据库连接串
        /// </param>
        /// <returns></returns>
        private OperationDataArgs11111 OperationA00(List<string> AListStringArgs)
        {
            OperationDataArgs11111 LOperationReturn = new OperationDataArgs11111();
            string LStrDynamicSQL = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperation01Return = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();

                LStrDynamicSQL = "SELECT * FROM T_00_009";
                LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                if (LDatabaseOperation01Return.BoolReturn)
                {
                    LOperationReturn.DataSetReturn = LDatabaseOperation01Return.DataSetReturn;
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                }
                else
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                }
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }

        /// <summary>
        /// 获取所有资源类型
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：数据库类型
        /// 1：数据库连接串
        /// 2：租户编码
        /// </param>
        /// <returns></returns>
        private OperationDataArgs11111 OperationA01(List<string> AListStringArgs)
        {
            OperationDataArgs11111 LOperationReturn = new OperationDataArgs11111();
            string LStrDynamicSQL = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperation01Return = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();

                LStrDynamicSQL = "SELECT * FROM T_00_010";
                LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                if (LDatabaseOperation01Return.BoolReturn)
                {
                    LOperationReturn.DataSetReturn = LDatabaseOperation01Return.DataSetReturn;
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                }
                else
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                }
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }

        /// <summary>
        /// 根据资源ID获取资源信息
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：数据库类型
        /// 1：数据库连接串
        /// 2：租户编码
        /// 3：资源ID编码（3位数）</param>
        /// <returns></returns>
        private OperationDataArgs11111 OperationA02(List<string> AListStringArgs)
        {
            OperationDataArgs11111 LOperationReturn = new OperationDataArgs11111();
            string LStrDynamicSQL = string.Empty;
            int LIntTypeID = 0;
            string LStrIDBegin = string.Empty, LStrIDEnd = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperation01Return = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();

                LIntTypeID = int.Parse(AListStringArgs[3]);
                LStrIDBegin = LIntTypeID.ToString() + "0000000000000001";
                LStrIDEnd = (LIntTypeID + 1).ToString() + "0000000000000000";
                LStrDynamicSQL = "SELECT * FROM T_11_101_" + AListStringArgs[2] + " WHERE C001 >= " + LStrIDBegin + " AND C001 < " + LStrIDEnd + " ORDER BY C001 ASC";
                LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                if (LDatabaseOperation01Return.BoolReturn)
                {
                    LOperationReturn.DataSetReturn = LDatabaseOperation01Return.DataSetReturn;
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                }
                else
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                }
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }
    }
}
