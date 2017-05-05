﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.34209
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace UMPS1600.Service16001 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="Service16001.IService16001", CallbackContract=typeof(UMPS1600.Service16001.IService16001Callback))]
    public interface IService16001 {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IService16001/LoginSystem")]
        void LoginSystem(VoiceCyber.UMP.Common.SessionInfo session);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IService16001/SendHeartMsg")]
        void SendHeartMsg();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IService16001/LogOff")]
        void LogOff(VoiceCyber.UMP.Common.SessionInfo session);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IService16001/SendChatMessage")]
        void SendChatMessage(Common1600.ChatMessage msgObj, bool bNewCookie);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IService16001/EndCookieByID")]
        void EndCookieByID(System.Collections.Generic.List<string> lstParams);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IService16001Callback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IService16001/SendSysMessage")]
        void SendSysMessage(Common1600.S1600MessageType mesType, System.Collections.Generic.List<string> lstArgs);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IService16001/InitFriendList")]
        void InitFriendList(System.Collections.Generic.List<string> lstFriends);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IService16001/ReceiveChatMsg")]
        void ReceiveChatMsg(Common1600.ChatMessage chatMsg);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IService16001Channel : UMPS1600.Service16001.IService16001, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class Service16001Client : System.ServiceModel.DuplexClientBase<UMPS1600.Service16001.IService16001>, UMPS1600.Service16001.IService16001 {
        
        public Service16001Client(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public Service16001Client(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public Service16001Client(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public Service16001Client(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public Service16001Client(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public void LoginSystem(VoiceCyber.UMP.Common.SessionInfo session) {
            base.Channel.LoginSystem(session);
        }
        
        public void SendHeartMsg() {
            base.Channel.SendHeartMsg();
        }
        
        public void LogOff(VoiceCyber.UMP.Common.SessionInfo session) {
            base.Channel.LogOff(session);
        }
        
        public void SendChatMessage(Common1600.ChatMessage msgObj, bool bNewCookie) {
            base.Channel.SendChatMessage(msgObj, bNewCookie);
        }
        
        public void EndCookieByID(System.Collections.Generic.List<string> lstParams) {
            base.Channel.EndCookieByID(lstParams);
        }
    }
}