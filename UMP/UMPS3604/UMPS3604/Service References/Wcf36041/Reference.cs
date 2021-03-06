﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UMPS3604.Wcf36041 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="Wcf36041.IService36041")]
    public interface IService36041 {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService36041/UmpTaskOperation", ReplyAction="http://tempuri.org/IService36041/UmpTaskOperationResponse")]
        VoiceCyber.UMP.Communications.WebReturn UmpTaskOperation(VoiceCyber.UMP.Communications.WebRequest webRequest);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService36041/UmpTaskOperation", ReplyAction="http://tempuri.org/IService36041/UmpTaskOperationResponse")]
        System.Threading.Tasks.Task<VoiceCyber.UMP.Communications.WebReturn> UmpTaskOperationAsync(VoiceCyber.UMP.Communications.WebRequest webRequest);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService36041/UmpUpOperation", ReplyAction="http://tempuri.org/IService36041/UmpUpOperationResponse")]
        VoiceCyber.UMP.Communications.WebReturn UmpUpOperation(Common3604.UpRequest upRequest);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService36041/UmpUpOperation", ReplyAction="http://tempuri.org/IService36041/UmpUpOperationResponse")]
        System.Threading.Tasks.Task<VoiceCyber.UMP.Communications.WebReturn> UmpUpOperationAsync(Common3604.UpRequest upRequest);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IService36041Channel : UMPS3604.Wcf36041.IService36041, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class Service36041Client : System.ServiceModel.ClientBase<UMPS3604.Wcf36041.IService36041>, UMPS3604.Wcf36041.IService36041 {
        
        public Service36041Client() {
        }
        
        public Service36041Client(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public Service36041Client(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Service36041Client(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Service36041Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public VoiceCyber.UMP.Communications.WebReturn UmpTaskOperation(VoiceCyber.UMP.Communications.WebRequest webRequest) {
            return base.Channel.UmpTaskOperation(webRequest);
        }
        
        public System.Threading.Tasks.Task<VoiceCyber.UMP.Communications.WebReturn> UmpTaskOperationAsync(VoiceCyber.UMP.Communications.WebRequest webRequest) {
            return base.Channel.UmpTaskOperationAsync(webRequest);
        }
        
        public VoiceCyber.UMP.Communications.WebReturn UmpUpOperation(Common3604.UpRequest upRequest) {
            return base.Channel.UmpUpOperation(upRequest);
        }
        
        public System.Threading.Tasks.Task<VoiceCyber.UMP.Communications.WebReturn> UmpUpOperationAsync(Common3604.UpRequest upRequest) {
            return base.Channel.UmpUpOperationAsync(upRequest);
        }
    }
}
