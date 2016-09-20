using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Wcf00000
{
    [ServiceContract]
    public interface IService00000
    {
        [OperationContract]
        OperationDataArgs OperationMethodA(int AIntOperationID, List<string> AListStringArgs);
    }

    
    [DataContract]
    public class OperationDataArgs
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
