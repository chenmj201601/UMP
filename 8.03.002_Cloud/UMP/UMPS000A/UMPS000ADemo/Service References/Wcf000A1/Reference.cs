﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.18063
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace UMPS000ADemo.Wcf000A1 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://www.voicecyber.com/UMP/Services/2015/03", ConfigurationName="Wcf000A1.IService000A1")]
    public interface IService000A1 {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.voicecyber.com/UMP/Services/2015/03/IService000A1/DoOperation", ReplyAction="http://www.voicecyber.com/UMP/Services/2015/03/IService000A1/DoOperationResponse")]
        VoiceCyber.UMP.Common000A1.SDKReturn DoOperation(VoiceCyber.UMP.Common000A1.SDKRequest webRequest);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IService000A1Channel : UMPS000ADemo.Wcf000A1.IService000A1, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class Service000A1Client : System.ServiceModel.ClientBase<UMPS000ADemo.Wcf000A1.IService000A1>, UMPS000ADemo.Wcf000A1.IService000A1 {
        
        public Service000A1Client() {
        }
        
        public Service000A1Client(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public Service000A1Client(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Service000A1Client(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Service000A1Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public VoiceCyber.UMP.Common000A1.SDKReturn DoOperation(VoiceCyber.UMP.Common000A1.SDKRequest webRequest) {
            return base.Channel.DoOperation(webRequest);
        }
    }
}
