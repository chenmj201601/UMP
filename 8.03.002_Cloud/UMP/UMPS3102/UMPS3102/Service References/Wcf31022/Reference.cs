﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.18444
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace UMPS3102.Wcf31022 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://www.voicecyber.com/UMP/Services/2015/03", ConfigurationName="Wcf31022.IService31022")]
    public interface IService31022 {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.voicecyber.com/UMP/Services/2015/03/IService31022/DoOperation", ReplyAction="http://www.voicecyber.com/UMP/Services/2015/03/IService31022/DoOperationResponse")]
        VoiceCyber.UMP.Communications.WebReturn DoOperation(VoiceCyber.UMP.Communications.WebRequest webRequest);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IService31022Channel : UMPS3102.Wcf31022.IService31022, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class Service31022Client : System.ServiceModel.ClientBase<UMPS3102.Wcf31022.IService31022>, UMPS3102.Wcf31022.IService31022 {
        
        public Service31022Client() {
        }
        
        public Service31022Client(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public Service31022Client(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Service31022Client(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Service31022Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public VoiceCyber.UMP.Communications.WebReturn DoOperation(VoiceCyber.UMP.Communications.WebRequest webRequest) {
            return base.Channel.DoOperation(webRequest);
        }
    }
}
