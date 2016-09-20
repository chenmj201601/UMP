using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data;

namespace WcfService31032
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService1”。
    [ServiceContract]
    public interface IService31032
    {

        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        // TODO: 在此添加您的服务操作


        //查询当前人所属的机构
        [OperationContract]
        Service02Return GetUserControlOrg(string dbType, string dbURL, string UserID, string ParentID);

        [OperationContract]
        Service02Return GetUserControlAgentOrExtension(string dbType, string dbURL, string UserID, string ParentID, string ObjectType);

        [OperationContract]
        Service02Return GetUserOperation(string dbType, string dbURL, string UserID);


    }


    // 使用下面示例中说明的数据约定将复合类型添加到服务操作。
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";


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
    }



    //数据库的信息
    [DataContract]
    public class Service02Return
    {
        Boolean BoolReturn = true;
        int IntReturn = 0;
        String StringReturn = string.Empty;
        string stringFlag = "T";
        string StringErrorMessage=string.Empty;
        List<String> ListStringReturn = new List<string>();
        DataSet DataSetReturn = new DataSet();
        List<DataSet> ListDataSetReturn = new List<DataSet>();

        [DataMember]
        public string ErrorFlag 
        {
            get { return stringFlag; }
            set { stringFlag = value; }
        }
        [DataMember]
        public string ErrorMessage 
        {
            get { return StringErrorMessage; }
            set
            {
                StringErrorMessage = value;
            }
        }


        [DataMember]
        public int ReturnValueInt
        {
            get { return IntReturn; }
            set { IntReturn = value; }
        }

        [DataMember]
        public bool ReturnValueBool
        {
            get { return BoolReturn; }
            set { BoolReturn = value; }
        }

        [DataMember]
        public string ReturnValueString
        {
            get { return StringReturn; }
            set { StringReturn = value; }
        }

        [DataMember]
        public List<String> ReturnValueListString
        {
            get { return ListStringReturn; }
            set { ListStringReturn = value; }
        }

        [DataMember]
        public DataSet ReturnValueDataSet
        {
            get { return DataSetReturn; }
            set { DataSetReturn = value; }
        }

        [DataMember]
        public List<DataSet> ReturnValueListDataSet
        {
            get { return ListDataSetReturn; }
            set { ListDataSetReturn = value; }
        }
    }
}
