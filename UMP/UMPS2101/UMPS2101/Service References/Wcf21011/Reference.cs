﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UMPS2101.Wcf21011 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="Wcf21011.IService21011")]
    public interface IService21011 {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService21011/DoOperation", ReplyAction="http://tempuri.org/IService21011/DoOperationResponse")]
        VoiceCyber.UMP.Communications.WebReturn DoOperation(VoiceCyber.UMP.Communications.WebRequest webRequest);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IService21011Channel : UMPS2101.Wcf21011.IService21011, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class Service21011Client : System.ServiceModel.ClientBase<UMPS2101.Wcf21011.IService21011>, UMPS2101.Wcf21011.IService21011 {
        
        public Service21011Client() {
        }
        
        public Service21011Client(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public Service21011Client(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Service21011Client(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Service21011Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public VoiceCyber.UMP.Communications.WebReturn DoOperation(VoiceCyber.UMP.Communications.WebRequest webRequest) {
            return base.Channel.DoOperation(webRequest);
        }
    }
}