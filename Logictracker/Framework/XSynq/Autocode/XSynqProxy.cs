[assembly: System.Runtime.Serialization.ContractNamespaceAttribute("http://walks.ath.cx/xsynq", ClrNamespace="walks.ath.cx.xsynq")]

namespace Logictracker.xsynq
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="XSynqFaultCode", Namespace="http://walks.ath.cx/xsynq")]
    public enum XSynqFaultCode
    {
        
        [System.Runtime.Serialization.EnumMemberAttribute]
        DocumentNotFound = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute]
        Undefined = 1,
    }
}


[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(Namespace="http://walks.ath.cx/xsynq", ConfigurationName="XSynqPublisher", CallbackContract=typeof(XSynqPublisherCallback), SessionMode=System.ServiceModel.SessionMode.Required)]
public interface XSynqPublisher
{
    
    [System.ServiceModel.OperationContractAttribute(ProtectionLevel=System.Net.Security.ProtectionLevel.EncryptAndSign, Action="http://walks.ath.cx/xsynq/XSynqPublisher/Subscribe", ReplyAction="http://walks.ath.cx/xsynq/XSynqPublisher/SubscribeResponse")]
    [System.ServiceModel.FaultContractAttribute(typeof(Logictracker.xsynq.XSynqFaultCode), Action = "http://walks.ath.cx/xsynq/XSynqPublisher/SubscribeXSynqFaultCodeFault", ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign, Name = "XSynqFaultCode")]
    string Subscribe(string DocumentName, string ClientName, string PublisherBranch, string SubscriberBranch, bool BranchOwner);
    
    [System.ServiceModel.OperationContractAttribute(IsTerminating=true, ProtectionLevel=System.Net.Security.ProtectionLevel.EncryptAndSign, Action="http://walks.ath.cx/xsynq/XSynqPublisher/Unsubscribe", ReplyAction="http://walks.ath.cx/xsynq/XSynqPublisher/UnsubscribeResponse")]
    [System.ServiceModel.FaultContractAttribute(typeof(Logictracker.xsynq.XSynqFaultCode), Action = "http://walks.ath.cx/xsynq/XSynqPublisher/UnsubscribeXSynqFaultCodeFault", ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign, Name = "XSynqFaultCode")]
    void Unsubscribe();
    
    [System.ServiceModel.OperationContractAttribute(IsInitiating=false, ProtectionLevel=System.Net.Security.ProtectionLevel.EncryptAndSign, Action="http://walks.ath.cx/xsynq/XSynqPublisher/PendigsNotify", ReplyAction="http://walks.ath.cx/xsynq/XSynqPublisher/PendigsNotifyResponse")]
    [System.ServiceModel.FaultContractAttribute(typeof(Logictracker.xsynq.XSynqFaultCode), Action = "http://walks.ath.cx/xsynq/XSynqPublisher/PendigsNotifyXSynqFaultCodeFault", ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign, Name = "XSynqFaultCode")]
    void PendigsNotify(int Count);
    
    [System.ServiceModel.OperationContractAttribute(IsInitiating=false, ProtectionLevel=System.Net.Security.ProtectionLevel.EncryptAndSign, Action="http://walks.ath.cx/xsynq/XSynqPublisher/LockRequest", ReplyAction="http://walks.ath.cx/xsynq/XSynqPublisher/LockRequestResponse")]
    [System.ServiceModel.FaultContractAttribute(typeof(Logictracker.xsynq.XSynqFaultCode), Action = "http://walks.ath.cx/xsynq/XSynqPublisher/LockRequestXSynqFaultCodeFault", ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign, Name = "XSynqFaultCode")]
    void LockRequest(string BranchXPath);
    
    [System.ServiceModel.OperationContractAttribute(IsInitiating=false, ProtectionLevel=System.Net.Security.ProtectionLevel.EncryptAndSign, Action="http://walks.ath.cx/xsynq/XSynqPublisher/UnlockRequest", ReplyAction="http://walks.ath.cx/xsynq/XSynqPublisher/UnlockRequestResponse")]
    [System.ServiceModel.FaultContractAttribute(typeof(Logictracker.xsynq.XSynqFaultCode), Action = "http://walks.ath.cx/xsynq/XSynqPublisher/UnlockRequestXSynqFaultCodeFault", ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign, Name = "XSynqFaultCode")]
    void UnlockRequest(string BranchXPath);
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
public interface XSynqPublisherCallback
{
    
    [System.ServiceModel.OperationContractAttribute(ProtectionLevel=System.Net.Security.ProtectionLevel.EncryptAndSign, Action="http://walks.ath.cx/xsynq/XSynqPublisher/Reset", ReplyAction="http://walks.ath.cx/xsynq/XSynqPublisher/ResetResponse")]
    [System.ServiceModel.FaultContractAttribute(typeof(Logictracker.xsynq.XSynqFaultCode), Action = "http://walks.ath.cx/xsynq/XSynqPublisher/ResetXSynqFaultCodeFault", ProtectionLevel = System.Net.Security.ProtectionLevel.None, Name = "XSynqFaultCode")]
    void Reset(string xDocument);
    
    [System.ServiceModel.OperationContractAttribute(ProtectionLevel=System.Net.Security.ProtectionLevel.EncryptAndSign, Action="http://walks.ath.cx/xsynq/XSynqPublisher/GetChangeSet", ReplyAction="http://walks.ath.cx/xsynq/XSynqPublisher/GetChangeSetResponse")]
    [System.ServiceModel.FaultContractAttribute(typeof(Logictracker.xsynq.XSynqFaultCode), Action = "http://walks.ath.cx/xsynq/XSynqPublisher/GetChangeSetXSynqFaultCodeFault", ProtectionLevel = System.Net.Security.ProtectionLevel.None, Name = "XSynqFaultCode")]
    string GetChangeSet();
    
    [System.ServiceModel.OperationContractAttribute(ProtectionLevel=System.Net.Security.ProtectionLevel.EncryptAndSign, Action="http://walks.ath.cx/xsynq/XSynqPublisher/ApplyChangeSet", ReplyAction="http://walks.ath.cx/xsynq/XSynqPublisher/ApplyChangeSetResponse")]
    [System.ServiceModel.FaultContractAttribute(typeof(Logictracker.xsynq.XSynqFaultCode), Action = "http://walks.ath.cx/xsynq/XSynqPublisher/ApplyChangeSetXSynqFaultCodeFault", ProtectionLevel = System.Net.Security.ProtectionLevel.None, Name = "XSynqFaultCode")]
    void ApplyChangeSet(string ChangeSet);
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
public interface XSynqPublisherChannel : XSynqPublisher, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
public class XSynqPublisherClient : System.ServiceModel.DuplexClientBase<XSynqPublisher>, XSynqPublisher
{
    
    public XSynqPublisherClient(System.ServiceModel.InstanceContext callbackInstance) : 
            base(callbackInstance)
    {
    }
    
    public XSynqPublisherClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
            base(callbackInstance, endpointConfigurationName)
    {
    }
    
    public XSynqPublisherClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
            base(callbackInstance, endpointConfigurationName, remoteAddress)
    {
    }
    
    public XSynqPublisherClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(callbackInstance, endpointConfigurationName, remoteAddress)
    {
    }
    
    public XSynqPublisherClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(callbackInstance, binding, remoteAddress)
    {
    }
    
    public string Subscribe(string DocumentName, string ClientName, string PublisherBranch, string SubscriberBranch, bool BranchOwner)
    {
        return Channel.Subscribe(DocumentName, ClientName, PublisherBranch, SubscriberBranch, BranchOwner);
    }
    
    public void Unsubscribe()
    {
        Channel.Unsubscribe();
    }
    
    public void PendigsNotify(int Count)
    {
        Channel.PendigsNotify(Count);
    }
    
    public void LockRequest(string BranchXPath)
    {
        Channel.LockRequest(BranchXPath);
    }
    
    public void UnlockRequest(string BranchXPath)
    {
        Channel.UnlockRequest(BranchXPath);
    }
}
