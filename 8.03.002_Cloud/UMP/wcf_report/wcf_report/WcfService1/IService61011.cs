using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using VoiceCyber.UMP.Communications;

namespace Wcf61012
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService1”。
    [ServiceContract]
    public interface IService61011
    {

        [OperationContract]
        WebReturn DoOperation (WebRequest RResult);

        // TODO: 在此添加您的服务操作
    }


    // 使用下面示例中说明的数据约定将复合类型添加到服务操作。
    [DataContract]
    public class ReportResult
    {
        bool boolValue = true;
        string stringValue = "Hello ";
        string strValue;
        int intValue;
        DataSet data_setValue;
        DataSet DSValue;
        List<string> ListStr;

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }

        [DataMember]
        public string StrValue
        {
            get { return strValue; }
            set { strValue = value; }
        }

        [DataMember]
        public int IntValue
        {
            get { return intValue; }
            set { intValue = value; }
        }

        [DataMember]
        public DataSet DataSetValue
        {
            get { return data_setValue; }
            set { data_setValue = value; }
        }

        [DataMember]
        public DataSet DataSet_Value
        {
            get { return DSValue; }
            set { DSValue = value; }
        }

        [DataMember]
        public List<string> ListStrValue
        {
            get { return ListStr; }
            set { ListStr = value; }
        }
    }

    [DataContract]
    public class OperationReturn
    {
        public int Code { get; set; }
        //
        // 摘要:
        //     返回值，使用的时候可通过 as 转换成对应的对象
        public object Data { get; set; }
        //
        // 摘要:
        //     操作异常
        public Exception Exception { get; set; }
        //
        // 摘要:
        //     返回值，整型
        public int IntValue { get; set; }
        //
        // 摘要:
        //     返回消息
        public string Message { get; set; }
        //
        // 摘要:
        //     返回值，数值型
        public decimal NumericValue { get; set; }
        //
        // 摘要:
        //     操作结果
        public bool Result { get; set; }
        //
        // 摘要:
        //     返回值，文本型
        public string StringValue { get; set; }
        public string StringValue2 { get; set; }
        
    }
}
