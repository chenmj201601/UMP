﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.34209
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace UMPS3106.Wcf11901 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="Wcf11901.IService11901")]
    public interface IService11901 {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService11901/OperationMethodA", ReplyAction="http://tempuri.org/IService11901/OperationMethodAResponse")]
        VoiceCyber.UMP.Controls.Wcf11901.OperationDataArgs OperationMethodA(int AIntOperationID, System.Collections.Generic.List<string> AListStringArgs);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IService11901Channel : UMPS3106.Wcf11901.IService11901, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class Service11901Client : System.ServiceModel.ClientBase<UMPS3106.Wcf11901.IService11901>, UMPS3106.Wcf11901.IService11901 {
        
        public Service11901Client() {
        }
        
        public Service11901Client(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public Service11901Client(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Service11901Client(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Service11901Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public VoiceCyber.UMP.Controls.Wcf11901.OperationDataArgs OperationMethodA(int AIntOperationID, System.Collections.Generic.List<string> AListStringArgs) {
            return base.Channel.OperationMethodA(AIntOperationID, AListStringArgs);
        }
    }
}
