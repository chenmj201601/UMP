﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.18444
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace UMPS1101.OUMServices2 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="OUMServices2.IService11002")]
    public interface IService11002 {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService11002/OUMOperation", ReplyAction="http://tempuri.org/IService11002/OUMOperationResponse")]
        VoiceCyber.UMP.Communications.WebReturn OUMOperation(VoiceCyber.UMP.Communications.WebRequest webRequest);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IService11002Channel : UMPS1101.OUMServices2.IService11002, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class Service11002Client : System.ServiceModel.ClientBase<UMPS1101.OUMServices2.IService11002>, UMPS1101.OUMServices2.IService11002 {
        
        public Service11002Client() {
        }
        
        public Service11002Client(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public Service11002Client(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Service11002Client(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Service11002Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public VoiceCyber.UMP.Communications.WebReturn OUMOperation(VoiceCyber.UMP.Communications.WebRequest webRequest) {
            return base.Channel.OUMOperation(webRequest);
        }
    }
}
