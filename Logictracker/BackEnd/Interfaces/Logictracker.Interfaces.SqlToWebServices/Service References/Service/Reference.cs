﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Logictracker.Interfaces.SqlToWebServices.Service {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="RespuestaOfBoolean", Namespace="http://plataforma.logictracker.com/")]
    [System.SerializableAttribute()]
    public partial class RespuestaOfBoolean : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private bool ResultadoField;
        
        private bool RespuestaOkField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MensajeField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public bool Resultado {
            get {
                return this.ResultadoField;
            }
            set {
                if ((this.ResultadoField.Equals(value) != true)) {
                    this.ResultadoField = value;
                    this.RaisePropertyChanged("Resultado");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=1)]
        public bool RespuestaOk {
            get {
                return this.RespuestaOkField;
            }
            set {
                if ((this.RespuestaOkField.Equals(value) != true)) {
                    this.RespuestaOkField = value;
                    this.RaisePropertyChanged("RespuestaOk");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string Mensaje {
            get {
                return this.MensajeField;
            }
            set {
                if ((object.ReferenceEquals(this.MensajeField, value) != true)) {
                    this.MensajeField = value;
                    this.RaisePropertyChanged("Mensaje");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="RespuestaOfString", Namespace="http://plataforma.logictracker.com/")]
    [System.SerializableAttribute()]
    public partial class RespuestaOfString : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ResultadoField;
        
        private bool RespuestaOkField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MensajeField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public string Resultado {
            get {
                return this.ResultadoField;
            }
            set {
                if ((object.ReferenceEquals(this.ResultadoField, value) != true)) {
                    this.ResultadoField = value;
                    this.RaisePropertyChanged("Resultado");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=1)]
        public bool RespuestaOk {
            get {
                return this.RespuestaOkField;
            }
            set {
                if ((this.RespuestaOkField.Equals(value) != true)) {
                    this.RespuestaOkField = value;
                    this.RaisePropertyChanged("RespuestaOk");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string Mensaje {
            get {
                return this.MensajeField;
            }
            set {
                if ((object.ReferenceEquals(this.MensajeField, value) != true)) {
                    this.MensajeField = value;
                    this.RaisePropertyChanged("Mensaje");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://plataforma.logictracker.com/", ConfigurationName="Service.TicketsSoap")]
    public interface TicketsSoap {
        
        // CODEGEN: Generating message contract since element name sessionId from namespace http://plataforma.logictracker.com/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://plataforma.logictracker.com/AssignAndInit", ReplyAction="*")]
        Logictracker.Interfaces.SqlToWebServices.Service.AssignAndInitResponse AssignAndInit(Logictracker.Interfaces.SqlToWebServices.Service.AssignAndInitRequest request);
        
        // CODEGEN: Generating message contract since element name username from namespace http://plataforma.logictracker.com/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://plataforma.logictracker.com/Login", ReplyAction="*")]
        Logictracker.Interfaces.SqlToWebServices.Service.LoginResponse Login(Logictracker.Interfaces.SqlToWebServices.Service.LoginRequest request);
        
        // CODEGEN: Generating message contract since element name sessionId from namespace http://plataforma.logictracker.com/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://plataforma.logictracker.com/IsActive", ReplyAction="*")]
        Logictracker.Interfaces.SqlToWebServices.Service.IsActiveResponse IsActive(Logictracker.Interfaces.SqlToWebServices.Service.IsActiveRequest request);
        
        // CODEGEN: Generating message contract since element name sessionId from namespace http://plataforma.logictracker.com/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://plataforma.logictracker.com/Logout", ReplyAction="*")]
        Logictracker.Interfaces.SqlToWebServices.Service.LogoutResponse Logout(Logictracker.Interfaces.SqlToWebServices.Service.LogoutRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class AssignAndInitRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="AssignAndInit", Namespace="http://plataforma.logictracker.com/", Order=0)]
        public Logictracker.Interfaces.SqlToWebServices.Service.AssignAndInitRequestBody Body;
        
        public AssignAndInitRequest() {
        }
        
        public AssignAndInitRequest(Logictracker.Interfaces.SqlToWebServices.Service.AssignAndInitRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://plataforma.logictracker.com/")]
    public partial class AssignAndInitRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string sessionId;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string company;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string branch;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=3)]
        public System.DateTime utcDate;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string clientCode;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=5)]
        public string deliveryPointCode;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=6)]
        public int tripNo;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=7)]
        public string vehicle;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=8)]
        public string driver;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=9)]
        public string load;
        
        public AssignAndInitRequestBody() {
        }
        
        public AssignAndInitRequestBody(string sessionId, string company, string branch, System.DateTime utcDate, string clientCode, string deliveryPointCode, int tripNo, string vehicle, string driver, string load) {
            this.sessionId = sessionId;
            this.company = company;
            this.branch = branch;
            this.utcDate = utcDate;
            this.clientCode = clientCode;
            this.deliveryPointCode = deliveryPointCode;
            this.tripNo = tripNo;
            this.vehicle = vehicle;
            this.driver = driver;
            this.load = load;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class AssignAndInitResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="AssignAndInitResponse", Namespace="http://plataforma.logictracker.com/", Order=0)]
        public Logictracker.Interfaces.SqlToWebServices.Service.AssignAndInitResponseBody Body;
        
        public AssignAndInitResponse() {
        }
        
        public AssignAndInitResponse(Logictracker.Interfaces.SqlToWebServices.Service.AssignAndInitResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://plataforma.logictracker.com/")]
    public partial class AssignAndInitResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public Logictracker.Interfaces.SqlToWebServices.Service.RespuestaOfBoolean AssignAndInitResult;
        
        public AssignAndInitResponseBody() {
        }
        
        public AssignAndInitResponseBody(Logictracker.Interfaces.SqlToWebServices.Service.RespuestaOfBoolean AssignAndInitResult) {
            this.AssignAndInitResult = AssignAndInitResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class LoginRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="Login", Namespace="http://plataforma.logictracker.com/", Order=0)]
        public Logictracker.Interfaces.SqlToWebServices.Service.LoginRequestBody Body;
        
        public LoginRequest() {
        }
        
        public LoginRequest(Logictracker.Interfaces.SqlToWebServices.Service.LoginRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://plataforma.logictracker.com/")]
    public partial class LoginRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string username;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string password;
        
        public LoginRequestBody() {
        }
        
        public LoginRequestBody(string username, string password) {
            this.username = username;
            this.password = password;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class LoginResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="LoginResponse", Namespace="http://plataforma.logictracker.com/", Order=0)]
        public Logictracker.Interfaces.SqlToWebServices.Service.LoginResponseBody Body;
        
        public LoginResponse() {
        }
        
        public LoginResponse(Logictracker.Interfaces.SqlToWebServices.Service.LoginResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://plataforma.logictracker.com/")]
    public partial class LoginResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public Logictracker.Interfaces.SqlToWebServices.Service.RespuestaOfString LoginResult;
        
        public LoginResponseBody() {
        }
        
        public LoginResponseBody(Logictracker.Interfaces.SqlToWebServices.Service.RespuestaOfString LoginResult) {
            this.LoginResult = LoginResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class IsActiveRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="IsActive", Namespace="http://plataforma.logictracker.com/", Order=0)]
        public Logictracker.Interfaces.SqlToWebServices.Service.IsActiveRequestBody Body;
        
        public IsActiveRequest() {
        }
        
        public IsActiveRequest(Logictracker.Interfaces.SqlToWebServices.Service.IsActiveRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://plataforma.logictracker.com/")]
    public partial class IsActiveRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string sessionId;
        
        public IsActiveRequestBody() {
        }
        
        public IsActiveRequestBody(string sessionId) {
            this.sessionId = sessionId;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class IsActiveResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="IsActiveResponse", Namespace="http://plataforma.logictracker.com/", Order=0)]
        public Logictracker.Interfaces.SqlToWebServices.Service.IsActiveResponseBody Body;
        
        public IsActiveResponse() {
        }
        
        public IsActiveResponse(Logictracker.Interfaces.SqlToWebServices.Service.IsActiveResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://plataforma.logictracker.com/")]
    public partial class IsActiveResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public Logictracker.Interfaces.SqlToWebServices.Service.RespuestaOfBoolean IsActiveResult;
        
        public IsActiveResponseBody() {
        }
        
        public IsActiveResponseBody(Logictracker.Interfaces.SqlToWebServices.Service.RespuestaOfBoolean IsActiveResult) {
            this.IsActiveResult = IsActiveResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class LogoutRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="Logout", Namespace="http://plataforma.logictracker.com/", Order=0)]
        public Logictracker.Interfaces.SqlToWebServices.Service.LogoutRequestBody Body;
        
        public LogoutRequest() {
        }
        
        public LogoutRequest(Logictracker.Interfaces.SqlToWebServices.Service.LogoutRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://plataforma.logictracker.com/")]
    public partial class LogoutRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string sessionId;
        
        public LogoutRequestBody() {
        }
        
        public LogoutRequestBody(string sessionId) {
            this.sessionId = sessionId;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class LogoutResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="LogoutResponse", Namespace="http://plataforma.logictracker.com/", Order=0)]
        public Logictracker.Interfaces.SqlToWebServices.Service.LogoutResponseBody Body;
        
        public LogoutResponse() {
        }
        
        public LogoutResponse(Logictracker.Interfaces.SqlToWebServices.Service.LogoutResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://plataforma.logictracker.com/")]
    public partial class LogoutResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public Logictracker.Interfaces.SqlToWebServices.Service.RespuestaOfBoolean LogoutResult;
        
        public LogoutResponseBody() {
        }
        
        public LogoutResponseBody(Logictracker.Interfaces.SqlToWebServices.Service.RespuestaOfBoolean LogoutResult) {
            this.LogoutResult = LogoutResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface TicketsSoapChannel : Logictracker.Interfaces.SqlToWebServices.Service.TicketsSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class TicketsSoapClient : System.ServiceModel.ClientBase<Logictracker.Interfaces.SqlToWebServices.Service.TicketsSoap>, Logictracker.Interfaces.SqlToWebServices.Service.TicketsSoap {
        
        public TicketsSoapClient() {
        }
        
        public TicketsSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public TicketsSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TicketsSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TicketsSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Logictracker.Interfaces.SqlToWebServices.Service.AssignAndInitResponse Logictracker.Interfaces.SqlToWebServices.Service.TicketsSoap.AssignAndInit(Logictracker.Interfaces.SqlToWebServices.Service.AssignAndInitRequest request) {
            return base.Channel.AssignAndInit(request);
        }
        
        public Logictracker.Interfaces.SqlToWebServices.Service.RespuestaOfBoolean AssignAndInit(string sessionId, string company, string branch, System.DateTime utcDate, string clientCode, string deliveryPointCode, int tripNo, string vehicle, string driver, string load) {
            Logictracker.Interfaces.SqlToWebServices.Service.AssignAndInitRequest inValue = new Logictracker.Interfaces.SqlToWebServices.Service.AssignAndInitRequest();
            inValue.Body = new Logictracker.Interfaces.SqlToWebServices.Service.AssignAndInitRequestBody();
            inValue.Body.sessionId = sessionId;
            inValue.Body.company = company;
            inValue.Body.branch = branch;
            inValue.Body.utcDate = utcDate;
            inValue.Body.clientCode = clientCode;
            inValue.Body.deliveryPointCode = deliveryPointCode;
            inValue.Body.tripNo = tripNo;
            inValue.Body.vehicle = vehicle;
            inValue.Body.driver = driver;
            inValue.Body.load = load;
            Logictracker.Interfaces.SqlToWebServices.Service.AssignAndInitResponse retVal = ((Logictracker.Interfaces.SqlToWebServices.Service.TicketsSoap)(this)).AssignAndInit(inValue);
            return retVal.Body.AssignAndInitResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Logictracker.Interfaces.SqlToWebServices.Service.LoginResponse Logictracker.Interfaces.SqlToWebServices.Service.TicketsSoap.Login(Logictracker.Interfaces.SqlToWebServices.Service.LoginRequest request) {
            return base.Channel.Login(request);
        }
        
        public Logictracker.Interfaces.SqlToWebServices.Service.RespuestaOfString Login(string username, string password) {
            Logictracker.Interfaces.SqlToWebServices.Service.LoginRequest inValue = new Logictracker.Interfaces.SqlToWebServices.Service.LoginRequest();
            inValue.Body = new Logictracker.Interfaces.SqlToWebServices.Service.LoginRequestBody();
            inValue.Body.username = username;
            inValue.Body.password = password;
            Logictracker.Interfaces.SqlToWebServices.Service.LoginResponse retVal = ((Logictracker.Interfaces.SqlToWebServices.Service.TicketsSoap)(this)).Login(inValue);
            return retVal.Body.LoginResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Logictracker.Interfaces.SqlToWebServices.Service.IsActiveResponse Logictracker.Interfaces.SqlToWebServices.Service.TicketsSoap.IsActive(Logictracker.Interfaces.SqlToWebServices.Service.IsActiveRequest request) {
            return base.Channel.IsActive(request);
        }
        
        public Logictracker.Interfaces.SqlToWebServices.Service.RespuestaOfBoolean IsActive(string sessionId) {
            Logictracker.Interfaces.SqlToWebServices.Service.IsActiveRequest inValue = new Logictracker.Interfaces.SqlToWebServices.Service.IsActiveRequest();
            inValue.Body = new Logictracker.Interfaces.SqlToWebServices.Service.IsActiveRequestBody();
            inValue.Body.sessionId = sessionId;
            Logictracker.Interfaces.SqlToWebServices.Service.IsActiveResponse retVal = ((Logictracker.Interfaces.SqlToWebServices.Service.TicketsSoap)(this)).IsActive(inValue);
            return retVal.Body.IsActiveResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Logictracker.Interfaces.SqlToWebServices.Service.LogoutResponse Logictracker.Interfaces.SqlToWebServices.Service.TicketsSoap.Logout(Logictracker.Interfaces.SqlToWebServices.Service.LogoutRequest request) {
            return base.Channel.Logout(request);
        }
        
        public Logictracker.Interfaces.SqlToWebServices.Service.RespuestaOfBoolean Logout(string sessionId) {
            Logictracker.Interfaces.SqlToWebServices.Service.LogoutRequest inValue = new Logictracker.Interfaces.SqlToWebServices.Service.LogoutRequest();
            inValue.Body = new Logictracker.Interfaces.SqlToWebServices.Service.LogoutRequestBody();
            inValue.Body.sessionId = sessionId;
            Logictracker.Interfaces.SqlToWebServices.Service.LogoutResponse retVal = ((Logictracker.Interfaces.SqlToWebServices.Service.TicketsSoap)(this)).Logout(inValue);
            return retVal.Body.LogoutResult;
        }
    }
}
