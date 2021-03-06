﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Logictracker.Interfaces.PumpControl.PumpControlService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://pss/ho/fleet/wservices/gpsfleetws/transaction/", ConfigurationName="PumpControlService.GpsFleetService")]
    public interface GpsFleetService {
        
        // CODEGEN: Generating message contract since the operation getFirstPendingTransaction is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="http://pss/ho/fleet/wservices/gpsfleetws/getFirstPendingTransaction", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        Logictracker.Interfaces.PumpControl.PumpControlService.getFirstPendingTransactionResponse1 getFirstPendingTransaction(Logictracker.Interfaces.PumpControl.PumpControlService.getFirstPendingTransactionRequest request);
        
        // CODEGEN: Generating message contract since the operation setLastInformedTransaction is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="http://pss/ho/fleet/wservices/gpsfleetws/setLastInformedTransaction", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        Logictracker.Interfaces.PumpControl.PumpControlService.setLastInformedTransactionResponse1 setLastInformedTransaction(Logictracker.Interfaces.PumpControl.PumpControlService.setLastInformedTransactionRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34230")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://pss/ho/fleet/wservices/gpsfleetws/transaction/")]
    public partial class getFirstPendingTransaction : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string userField;
        
        private string passwordField;
        
        private string companyField;
        
        private string last_idField;
        
        private string car_plateField;
        
        private string date_fromField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string user {
            get {
                return this.userField;
            }
            set {
                this.userField = value;
                this.RaisePropertyChanged("user");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string password {
            get {
                return this.passwordField;
            }
            set {
                this.passwordField = value;
                this.RaisePropertyChanged("password");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string company {
            get {
                return this.companyField;
            }
            set {
                this.companyField = value;
                this.RaisePropertyChanged("company");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public string last_id {
            get {
                return this.last_idField;
            }
            set {
                this.last_idField = value;
                this.RaisePropertyChanged("last_id");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=4)]
        public string car_plate {
            get {
                return this.car_plateField;
            }
            set {
                this.car_plateField = value;
                this.RaisePropertyChanged("car_plate");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=5)]
        public string date_from {
            get {
                return this.date_fromField;
            }
            set {
                this.date_fromField = value;
                this.RaisePropertyChanged("date_from");
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
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34230")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://pss/ho/fleet/wservices/gpsfleetws/transaction/")]
    public partial class getFirstPendingTransactionResponse : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string car_plateField;
        
        private string trx_idField;
        
        private string trx_dateField;
        
        private string car_kilometerField;
        
        private string trx_volumeField;
        
        private string storeField;
        
        private string pumpField;
        
        private string hoseField;
        
        private string prod_idField;
        
        private string tag_idField;
        
        private string vehicle_idField;
        
        private string ppuField;
        
        private string amountField;
        
        private string lastinformed_error_msgField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string car_plate {
            get {
                return this.car_plateField;
            }
            set {
                this.car_plateField = value;
                this.RaisePropertyChanged("car_plate");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string trx_id {
            get {
                return this.trx_idField;
            }
            set {
                this.trx_idField = value;
                this.RaisePropertyChanged("trx_id");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string trx_date {
            get {
                return this.trx_dateField;
            }
            set {
                this.trx_dateField = value;
                this.RaisePropertyChanged("trx_date");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public string car_kilometer {
            get {
                return this.car_kilometerField;
            }
            set {
                this.car_kilometerField = value;
                this.RaisePropertyChanged("car_kilometer");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=4)]
        public string trx_volume {
            get {
                return this.trx_volumeField;
            }
            set {
                this.trx_volumeField = value;
                this.RaisePropertyChanged("trx_volume");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=5)]
        public string store {
            get {
                return this.storeField;
            }
            set {
                this.storeField = value;
                this.RaisePropertyChanged("store");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=6)]
        public string pump {
            get {
                return this.pumpField;
            }
            set {
                this.pumpField = value;
                this.RaisePropertyChanged("pump");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=7)]
        public string hose {
            get {
                return this.hoseField;
            }
            set {
                this.hoseField = value;
                this.RaisePropertyChanged("hose");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=8)]
        public string prod_id {
            get {
                return this.prod_idField;
            }
            set {
                this.prod_idField = value;
                this.RaisePropertyChanged("prod_id");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=9)]
        public string tag_id {
            get {
                return this.tag_idField;
            }
            set {
                this.tag_idField = value;
                this.RaisePropertyChanged("tag_id");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=10)]
        public string vehicle_id {
            get {
                return this.vehicle_idField;
            }
            set {
                this.vehicle_idField = value;
                this.RaisePropertyChanged("vehicle_id");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=11)]
        public string ppu {
            get {
                return this.ppuField;
            }
            set {
                this.ppuField = value;
                this.RaisePropertyChanged("ppu");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=12)]
        public string amount {
            get {
                return this.amountField;
            }
            set {
                this.amountField = value;
                this.RaisePropertyChanged("amount");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=13)]
        public string lastinformed_error_msg {
            get {
                return this.lastinformed_error_msgField;
            }
            set {
                this.lastinformed_error_msgField = value;
                this.RaisePropertyChanged("lastinformed_error_msg");
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class getFirstPendingTransactionRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://pss/ho/fleet/wservices/gpsfleetws/transaction/", Order=0)]
        public Logictracker.Interfaces.PumpControl.PumpControlService.getFirstPendingTransaction getFirstPendingTransaction;
        
        public getFirstPendingTransactionRequest() {
        }
        
        public getFirstPendingTransactionRequest(Logictracker.Interfaces.PumpControl.PumpControlService.getFirstPendingTransaction getFirstPendingTransaction) {
            this.getFirstPendingTransaction = getFirstPendingTransaction;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class getFirstPendingTransactionResponse1 {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://pss/ho/fleet/wservices/gpsfleetws/transaction/", Order=0)]
        public Logictracker.Interfaces.PumpControl.PumpControlService.getFirstPendingTransactionResponse getFirstPendingTransactionResponse;
        
        public getFirstPendingTransactionResponse1() {
        }
        
        public getFirstPendingTransactionResponse1(Logictracker.Interfaces.PumpControl.PumpControlService.getFirstPendingTransactionResponse getFirstPendingTransactionResponse) {
            this.getFirstPendingTransactionResponse = getFirstPendingTransactionResponse;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34230")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://pss/ho/fleet/wservices/gpsfleetws/transaction/")]
    public partial class setLastInformedTransaction : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string userField;
        
        private string passwordField;
        
        private string companyField;
        
        private string trx_idField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string user {
            get {
                return this.userField;
            }
            set {
                this.userField = value;
                this.RaisePropertyChanged("user");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string password {
            get {
                return this.passwordField;
            }
            set {
                this.passwordField = value;
                this.RaisePropertyChanged("password");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string company {
            get {
                return this.companyField;
            }
            set {
                this.companyField = value;
                this.RaisePropertyChanged("company");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public string trx_id {
            get {
                return this.trx_idField;
            }
            set {
                this.trx_idField = value;
                this.RaisePropertyChanged("trx_id");
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
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34230")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://pss/ho/fleet/wservices/gpsfleetws/transaction/")]
    public partial class setLastInformedTransactionResponse : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string lastinformed_error_msgField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string lastinformed_error_msg {
            get {
                return this.lastinformed_error_msgField;
            }
            set {
                this.lastinformed_error_msgField = value;
                this.RaisePropertyChanged("lastinformed_error_msg");
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class setLastInformedTransactionRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://pss/ho/fleet/wservices/gpsfleetws/transaction/", Order=0)]
        public Logictracker.Interfaces.PumpControl.PumpControlService.setLastInformedTransaction setLastInformedTransaction;
        
        public setLastInformedTransactionRequest() {
        }
        
        public setLastInformedTransactionRequest(Logictracker.Interfaces.PumpControl.PumpControlService.setLastInformedTransaction setLastInformedTransaction) {
            this.setLastInformedTransaction = setLastInformedTransaction;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class setLastInformedTransactionResponse1 {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://pss/ho/fleet/wservices/gpsfleetws/transaction/", Order=0)]
        public Logictracker.Interfaces.PumpControl.PumpControlService.setLastInformedTransactionResponse setLastInformedTransactionResponse;
        
        public setLastInformedTransactionResponse1() {
        }
        
        public setLastInformedTransactionResponse1(Logictracker.Interfaces.PumpControl.PumpControlService.setLastInformedTransactionResponse setLastInformedTransactionResponse) {
            this.setLastInformedTransactionResponse = setLastInformedTransactionResponse;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface GpsFleetServiceChannel : Logictracker.Interfaces.PumpControl.PumpControlService.GpsFleetService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GpsFleetServiceClient : System.ServiceModel.ClientBase<Logictracker.Interfaces.PumpControl.PumpControlService.GpsFleetService>, Logictracker.Interfaces.PumpControl.PumpControlService.GpsFleetService {
        
        public GpsFleetServiceClient() {
        }
        
        public GpsFleetServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public GpsFleetServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GpsFleetServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GpsFleetServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Logictracker.Interfaces.PumpControl.PumpControlService.getFirstPendingTransactionResponse1 Logictracker.Interfaces.PumpControl.PumpControlService.GpsFleetService.getFirstPendingTransaction(Logictracker.Interfaces.PumpControl.PumpControlService.getFirstPendingTransactionRequest request) {
            return base.Channel.getFirstPendingTransaction(request);
        }
        
        public Logictracker.Interfaces.PumpControl.PumpControlService.getFirstPendingTransactionResponse getFirstPendingTransaction(Logictracker.Interfaces.PumpControl.PumpControlService.getFirstPendingTransaction getFirstPendingTransaction1) {
            Logictracker.Interfaces.PumpControl.PumpControlService.getFirstPendingTransactionRequest inValue = new Logictracker.Interfaces.PumpControl.PumpControlService.getFirstPendingTransactionRequest();
            inValue.getFirstPendingTransaction = getFirstPendingTransaction1;
            Logictracker.Interfaces.PumpControl.PumpControlService.getFirstPendingTransactionResponse1 retVal = ((Logictracker.Interfaces.PumpControl.PumpControlService.GpsFleetService)(this)).getFirstPendingTransaction(inValue);
            return retVal.getFirstPendingTransactionResponse;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Logictracker.Interfaces.PumpControl.PumpControlService.setLastInformedTransactionResponse1 Logictracker.Interfaces.PumpControl.PumpControlService.GpsFleetService.setLastInformedTransaction(Logictracker.Interfaces.PumpControl.PumpControlService.setLastInformedTransactionRequest request) {
            return base.Channel.setLastInformedTransaction(request);
        }
        
        public Logictracker.Interfaces.PumpControl.PumpControlService.setLastInformedTransactionResponse setLastInformedTransaction(Logictracker.Interfaces.PumpControl.PumpControlService.setLastInformedTransaction setLastInformedTransaction1) {
            Logictracker.Interfaces.PumpControl.PumpControlService.setLastInformedTransactionRequest inValue = new Logictracker.Interfaces.PumpControl.PumpControlService.setLastInformedTransactionRequest();
            inValue.setLastInformedTransaction = setLastInformedTransaction1;
            Logictracker.Interfaces.PumpControl.PumpControlService.setLastInformedTransactionResponse1 retVal = ((Logictracker.Interfaces.PumpControl.PumpControlService.GpsFleetService)(this)).setLastInformedTransaction(inValue);
            return retVal.setLastInformedTransactionResponse;
        }
    }
}
