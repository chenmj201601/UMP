using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data;

namespace WCF_LanguagePackOperation
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService1”。
    [ServiceContract]
    public interface IService00001
    {
        [OperationContract]
        ReturnResult OperationMethodA(int AIntOperationID, List<string> AListStringArgs);
    }


    // 使用下面示例中说明的数据约定将复合类型添加到服务操作。
    [DataContract]
    public class ReturnResult
    {
        bool LBoolValue = true;
        string LStrValue = string.Empty;
        DataSet LDataSetValue = new DataSet();
        List<string> LListStrValue = new List<string>();
        List<DataSet> LListDataSetValue = new List<DataSet>();


        [DataMember]
        public bool BoolReturn
        {
            get { return LBoolValue; }
            set { LBoolValue = value; }
        }

        [DataMember]
        public string StringReturn
        {
            get { return LStrValue; }
            set { LStrValue = value; }
        }

        [DataMember]
        public DataSet DataSetReturn
        {
            get { return LDataSetValue; }
            set { LDataSetValue = value; }
        }

        [DataMember]
        public List<string> ListStringReturn
        {
            get { return LListStrValue; }
            set { LListStrValue = value; }
        }

        [DataMember]
        public List<DataSet> ListDataSetReturn
        {
            get { return LListDataSetValue; }
            set { LListDataSetValue = value; }
        }
    }
}
