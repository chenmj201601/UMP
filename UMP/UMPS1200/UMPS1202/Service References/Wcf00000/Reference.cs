﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace UMPS1202.Wcf00000 {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OperationDataArgs", Namespace="http://schemas.datacontract.org/2004/07/Wcf00000")]
    [System.SerializableAttribute()]
    public partial class OperationDataArgs : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool BoolReturnField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Data.DataSet DataSetReturnField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.List<System.Data.DataSet> ListDataSetReturnField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.List<string> ListStringReturnField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string StringReturnField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool BoolReturn {
            get {
                return this.BoolReturnField;
            }
            set {
                if ((this.BoolReturnField.Equals(value) != true)) {
                    this.BoolReturnField = value;
                    this.RaisePropertyChanged("BoolReturn");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Data.DataSet DataSetReturn {
            get {
                return this.DataSetReturnField;
            }
            set {
                if ((object.ReferenceEquals(this.DataSetReturnField, value) != true)) {
                    this.DataSetReturnField = value;
                    this.RaisePropertyChanged("DataSetReturn");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.List<System.Data.DataSet> ListDataSetReturn {
            get {
                return this.ListDataSetReturnField;
            }
            set {
                if ((object.ReferenceEquals(this.ListDataSetReturnField, value) != true)) {
                    this.ListDataSetReturnField = value;
                    this.RaisePropertyChanged("ListDataSetReturn");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.List<string> ListStringReturn {
            get {
                return this.ListStringReturnField;
            }
            set {
                if ((object.ReferenceEquals(this.ListStringReturnField, value) != true)) {
                    this.ListStringReturnField = value;
                    this.RaisePropertyChanged("ListStringReturn");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string StringReturn {
            get {
                return this.StringReturnField;
            }
            set {
                if ((object.ReferenceEquals(this.StringReturnField, value) != true)) {
                    this.StringReturnField = value;
                    this.RaisePropertyChanged("StringReturn");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="Wcf00000.IService00000")]
    public interface IService00000 {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService00000/OperationMethodA", ReplyAction="http://tempuri.org/IService00000/OperationMethodAResponse")]
        UMPS1202.Wcf00000.OperationDataArgs OperationMethodA(int AIntOperationID, System.Collections.Generic.List<string> AListStringArgs);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IService00000Channel : UMPS1202.Wcf00000.IService00000, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class Service00000Client : System.ServiceModel.ClientBase<UMPS1202.Wcf00000.IService00000>, UMPS1202.Wcf00000.IService00000 {
        
        public Service00000Client() {
        }
        
        public Service00000Client(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public Service00000Client(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Service00000Client(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Service00000Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public UMPS1202.Wcf00000.OperationDataArgs OperationMethodA(int AIntOperationID, System.Collections.Generic.List<string> AListStringArgs) {
            return base.Channel.OperationMethodA(AIntOperationID, AListStringArgs);
        }
    }
}
