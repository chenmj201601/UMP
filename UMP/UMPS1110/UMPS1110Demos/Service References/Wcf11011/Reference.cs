﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.18444
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace UMPS1110Demos.Wcf11011 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="Wcf11011.IService11011")]
    public interface IService11011 {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService11011/DoOperation", ReplyAction="http://tempuri.org/IService11011/DoOperationResponse")]
        VoiceCyber.UMP.Communications.WebReturn DoOperation(VoiceCyber.UMP.Communications.WebRequest webRequest);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IService11011Channel : UMPS1110Demos.Wcf11011.IService11011, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class Service11011Client : System.ServiceModel.ClientBase<UMPS1110Demos.Wcf11011.IService11011>, UMPS1110Demos.Wcf11011.IService11011 {
        
        public Service11011Client() {
        }
        
        public Service11011Client(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public Service11011Client(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Service11011Client(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Service11011Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public VoiceCyber.UMP.Communications.WebReturn DoOperation(VoiceCyber.UMP.Communications.WebRequest webRequest) {
            return base.Channel.DoOperation(webRequest);
        }
    }
}
