﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.18063
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace UMPS4601.Wcf46011 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://www.voicecyber.com/UMP/Services/2015/03", ConfigurationName="Wcf46011.IService46011")]
    public interface IService46011 {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.voicecyber.com/UMP/Services/2015/03/IService46011/DoOperation", ReplyAction="http://www.voicecyber.com/UMP/Services/2015/03/IService46011/DoOperationResponse")]
        VoiceCyber.UMP.Communications.WebReturn DoOperation(VoiceCyber.UMP.Communications.WebRequest webRequest);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IService46011Channel : UMPS4601.Wcf46011.IService46011, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class Service46011Client : System.ServiceModel.ClientBase<UMPS4601.Wcf46011.IService46011>, UMPS4601.Wcf46011.IService46011 {
        
        public Service46011Client() {
        }
        
        public Service46011Client(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public Service46011Client(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Service46011Client(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Service46011Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public VoiceCyber.UMP.Communications.WebReturn DoOperation(VoiceCyber.UMP.Communications.WebRequest webRequest) {
            return base.Channel.DoOperation(webRequest);
        }
    }
}
