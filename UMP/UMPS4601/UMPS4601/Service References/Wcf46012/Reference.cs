﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.34209
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace UMPS4601.Wcf46012 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="Wcf46012.IService46012")]
    public interface IService46012 {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService46012/UPMSOperation", ReplyAction="http://tempuri.org/IService46012/UPMSOperationResponse")]
        VoiceCyber.UMP.Communications.WebReturn UPMSOperation(VoiceCyber.UMP.Communications.WebRequest webRequest);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IService46012Channel : UMPS4601.Wcf46012.IService46012, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class Service46012Client : System.ServiceModel.ClientBase<UMPS4601.Wcf46012.IService46012>, UMPS4601.Wcf46012.IService46012 {
        
        public Service46012Client() {
        }
        
        public Service46012Client(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public Service46012Client(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Service46012Client(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Service46012Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public VoiceCyber.UMP.Communications.WebReturn UPMSOperation(VoiceCyber.UMP.Communications.WebRequest webRequest) {
            return base.Channel.UPMSOperation(webRequest);
        }
    }
}
