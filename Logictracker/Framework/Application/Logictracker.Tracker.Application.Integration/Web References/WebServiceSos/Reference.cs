﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace Logictracker.Tracker.Application.Integration.WebServiceSos {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.81.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="ServiceSoap", Namespace="http://ws_derivacion_ivr.redsos.com.ar/")]
    public partial class Service : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback ObtenerServiciosDerivarOperationCompleted;
        
        private System.Threading.SendOrPostCallback ObtenerIntentosLlamadaOperationCompleted;
        
        private System.Threading.SendOrPostCallback ObtenerIntentosConfirmaSMSOperationCompleted;
        
        private System.Threading.SendOrPostCallback ObtenerSMSaConfirmarOperationCompleted;
        
        private System.Threading.SendOrPostCallback informarResultadosOperationCompleted;
        
        private System.Threading.SendOrPostCallback ActualizarSvcOperationCompleted;
        
        private System.Threading.SendOrPostCallback ObtenerFormularioOperationCompleted;
        
        private System.Threading.SendOrPostCallback ValidarAccesoOperationCompleted;
        
        private System.Threading.SendOrPostCallback _alertasRollbackOperationCompleted;
        
        private System.Threading.SendOrPostCallback ObtenerAlertasOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public Service() {
            this.Url = global::Logictracker.Tracker.Application.Integration.Properties.Settings.Default.Logictracker_Tracker_Application_Integration_WebServiceSos_Service;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event ObtenerServiciosDerivarCompletedEventHandler ObtenerServiciosDerivarCompleted;
        
        /// <remarks/>
        public event ObtenerIntentosLlamadaCompletedEventHandler ObtenerIntentosLlamadaCompleted;
        
        /// <remarks/>
        public event ObtenerIntentosConfirmaSMSCompletedEventHandler ObtenerIntentosConfirmaSMSCompleted;
        
        /// <remarks/>
        public event ObtenerSMSaConfirmarCompletedEventHandler ObtenerSMSaConfirmarCompleted;
        
        /// <remarks/>
        public event informarResultadosCompletedEventHandler informarResultadosCompleted;
        
        /// <remarks/>
        public event ActualizarSvcCompletedEventHandler ActualizarSvcCompleted;
        
        /// <remarks/>
        public event ObtenerFormularioCompletedEventHandler ObtenerFormularioCompleted;
        
        /// <remarks/>
        public event ValidarAccesoCompletedEventHandler ValidarAccesoCompleted;
        
        /// <remarks/>
        public event _alertasRollbackCompletedEventHandler _alertasRollbackCompleted;
        
        /// <remarks/>
        public event ObtenerAlertasCompletedEventHandler ObtenerAlertasCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://ws_derivacion_ivr.redsos.com.ar/ObtenerServiciosDerivar", RequestNamespace="http://ws_derivacion_ivr.redsos.com.ar/", ResponseNamespace="http://ws_derivacion_ivr.redsos.com.ar/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string ObtenerServiciosDerivar() {
            object[] results = this.Invoke("ObtenerServiciosDerivar", new object[0]);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void ObtenerServiciosDerivarAsync() {
            this.ObtenerServiciosDerivarAsync(null);
        }
        
        /// <remarks/>
        public void ObtenerServiciosDerivarAsync(object userState) {
            if ((this.ObtenerServiciosDerivarOperationCompleted == null)) {
                this.ObtenerServiciosDerivarOperationCompleted = new System.Threading.SendOrPostCallback(this.OnObtenerServiciosDerivarOperationCompleted);
            }
            this.InvokeAsync("ObtenerServiciosDerivar", new object[0], this.ObtenerServiciosDerivarOperationCompleted, userState);
        }
        
        private void OnObtenerServiciosDerivarOperationCompleted(object arg) {
            if ((this.ObtenerServiciosDerivarCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ObtenerServiciosDerivarCompleted(this, new ObtenerServiciosDerivarCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://ws_derivacion_ivr.redsos.com.ar/ObtenerIntentosLlamada", RequestNamespace="http://ws_derivacion_ivr.redsos.com.ar/", ResponseNamespace="http://ws_derivacion_ivr.redsos.com.ar/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string ObtenerIntentosLlamada() {
            object[] results = this.Invoke("ObtenerIntentosLlamada", new object[0]);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void ObtenerIntentosLlamadaAsync() {
            this.ObtenerIntentosLlamadaAsync(null);
        }
        
        /// <remarks/>
        public void ObtenerIntentosLlamadaAsync(object userState) {
            if ((this.ObtenerIntentosLlamadaOperationCompleted == null)) {
                this.ObtenerIntentosLlamadaOperationCompleted = new System.Threading.SendOrPostCallback(this.OnObtenerIntentosLlamadaOperationCompleted);
            }
            this.InvokeAsync("ObtenerIntentosLlamada", new object[0], this.ObtenerIntentosLlamadaOperationCompleted, userState);
        }
        
        private void OnObtenerIntentosLlamadaOperationCompleted(object arg) {
            if ((this.ObtenerIntentosLlamadaCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ObtenerIntentosLlamadaCompleted(this, new ObtenerIntentosLlamadaCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://ws_derivacion_ivr.redsos.com.ar/ObtenerIntentosConfirmaSMS", RequestNamespace="http://ws_derivacion_ivr.redsos.com.ar/", ResponseNamespace="http://ws_derivacion_ivr.redsos.com.ar/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string ObtenerIntentosConfirmaSMS() {
            object[] results = this.Invoke("ObtenerIntentosConfirmaSMS", new object[0]);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void ObtenerIntentosConfirmaSMSAsync() {
            this.ObtenerIntentosConfirmaSMSAsync(null);
        }
        
        /// <remarks/>
        public void ObtenerIntentosConfirmaSMSAsync(object userState) {
            if ((this.ObtenerIntentosConfirmaSMSOperationCompleted == null)) {
                this.ObtenerIntentosConfirmaSMSOperationCompleted = new System.Threading.SendOrPostCallback(this.OnObtenerIntentosConfirmaSMSOperationCompleted);
            }
            this.InvokeAsync("ObtenerIntentosConfirmaSMS", new object[0], this.ObtenerIntentosConfirmaSMSOperationCompleted, userState);
        }
        
        private void OnObtenerIntentosConfirmaSMSOperationCompleted(object arg) {
            if ((this.ObtenerIntentosConfirmaSMSCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ObtenerIntentosConfirmaSMSCompleted(this, new ObtenerIntentosConfirmaSMSCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://ws_derivacion_ivr.redsos.com.ar/ObtenerSMSaConfirmar", RequestNamespace="http://ws_derivacion_ivr.redsos.com.ar/", ResponseNamespace="http://ws_derivacion_ivr.redsos.com.ar/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string ObtenerSMSaConfirmar() {
            object[] results = this.Invoke("ObtenerSMSaConfirmar", new object[0]);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void ObtenerSMSaConfirmarAsync() {
            this.ObtenerSMSaConfirmarAsync(null);
        }
        
        /// <remarks/>
        public void ObtenerSMSaConfirmarAsync(object userState) {
            if ((this.ObtenerSMSaConfirmarOperationCompleted == null)) {
                this.ObtenerSMSaConfirmarOperationCompleted = new System.Threading.SendOrPostCallback(this.OnObtenerSMSaConfirmarOperationCompleted);
            }
            this.InvokeAsync("ObtenerSMSaConfirmar", new object[0], this.ObtenerSMSaConfirmarOperationCompleted, userState);
        }
        
        private void OnObtenerSMSaConfirmarOperationCompleted(object arg) {
            if ((this.ObtenerSMSaConfirmarCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ObtenerSMSaConfirmarCompleted(this, new ObtenerSMSaConfirmarCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://ws_derivacion_ivr.redsos.com.ar/informarResultados", RequestNamespace="http://ws_derivacion_ivr.redsos.com.ar/", ResponseNamespace="http://ws_derivacion_ivr.redsos.com.ar/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string informarResultados(string servicio, string idTelefono, int tipoResultado, int intentos, int resultado, int idRechazo, System.DateTime fecha, int ultimo) {
            object[] results = this.Invoke("informarResultados", new object[] {
                        servicio,
                        idTelefono,
                        tipoResultado,
                        intentos,
                        resultado,
                        idRechazo,
                        fecha,
                        ultimo});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void informarResultadosAsync(string servicio, string idTelefono, int tipoResultado, int intentos, int resultado, int idRechazo, System.DateTime fecha, int ultimo) {
            this.informarResultadosAsync(servicio, idTelefono, tipoResultado, intentos, resultado, idRechazo, fecha, ultimo, null);
        }
        
        /// <remarks/>
        public void informarResultadosAsync(string servicio, string idTelefono, int tipoResultado, int intentos, int resultado, int idRechazo, System.DateTime fecha, int ultimo, object userState) {
            if ((this.informarResultadosOperationCompleted == null)) {
                this.informarResultadosOperationCompleted = new System.Threading.SendOrPostCallback(this.OninformarResultadosOperationCompleted);
            }
            this.InvokeAsync("informarResultados", new object[] {
                        servicio,
                        idTelefono,
                        tipoResultado,
                        intentos,
                        resultado,
                        idRechazo,
                        fecha,
                        ultimo}, this.informarResultadosOperationCompleted, userState);
        }
        
        private void OninformarResultadosOperationCompleted(object arg) {
            if ((this.informarResultadosCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.informarResultadosCompleted(this, new informarResultadosCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://ws_derivacion_ivr.redsos.com.ar/ActualizarSvc", RequestNamespace="http://ws_derivacion_ivr.redsos.com.ar/", ResponseNamespace="http://ws_derivacion_ivr.redsos.com.ar/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public int ActualizarSvc(string _idMovil, string _servicio, int _estadoSvc, string _dato) {
            object[] results = this.Invoke("ActualizarSvc", new object[] {
                        _idMovil,
                        _servicio,
                        _estadoSvc,
                        _dato});
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public void ActualizarSvcAsync(string _idMovil, string _servicio, int _estadoSvc, string _dato) {
            this.ActualizarSvcAsync(_idMovil, _servicio, _estadoSvc, _dato, null);
        }
        
        /// <remarks/>
        public void ActualizarSvcAsync(string _idMovil, string _servicio, int _estadoSvc, string _dato, object userState) {
            if ((this.ActualizarSvcOperationCompleted == null)) {
                this.ActualizarSvcOperationCompleted = new System.Threading.SendOrPostCallback(this.OnActualizarSvcOperationCompleted);
            }
            this.InvokeAsync("ActualizarSvc", new object[] {
                        _idMovil,
                        _servicio,
                        _estadoSvc,
                        _dato}, this.ActualizarSvcOperationCompleted, userState);
        }
        
        private void OnActualizarSvcOperationCompleted(object arg) {
            if ((this.ActualizarSvcCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ActualizarSvcCompleted(this, new ActualizarSvcCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://ws_derivacion_ivr.redsos.com.ar/ObtenerFormulario", RequestNamespace="http://ws_derivacion_ivr.redsos.com.ar/", ResponseNamespace="http://ws_derivacion_ivr.redsos.com.ar/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string ObtenerFormulario(string _idMovil, string _pass) {
            object[] results = this.Invoke("ObtenerFormulario", new object[] {
                        _idMovil,
                        _pass});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void ObtenerFormularioAsync(string _idMovil, string _pass) {
            this.ObtenerFormularioAsync(_idMovil, _pass, null);
        }
        
        /// <remarks/>
        public void ObtenerFormularioAsync(string _idMovil, string _pass, object userState) {
            if ((this.ObtenerFormularioOperationCompleted == null)) {
                this.ObtenerFormularioOperationCompleted = new System.Threading.SendOrPostCallback(this.OnObtenerFormularioOperationCompleted);
            }
            this.InvokeAsync("ObtenerFormulario", new object[] {
                        _idMovil,
                        _pass}, this.ObtenerFormularioOperationCompleted, userState);
        }
        
        private void OnObtenerFormularioOperationCompleted(object arg) {
            if ((this.ObtenerFormularioCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ObtenerFormularioCompleted(this, new ObtenerFormularioCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://ws_derivacion_ivr.redsos.com.ar/ValidarAcceso", RequestNamespace="http://ws_derivacion_ivr.redsos.com.ar/", ResponseNamespace="http://ws_derivacion_ivr.redsos.com.ar/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public int ValidarAcceso(string _idMovil, string _pass) {
            object[] results = this.Invoke("ValidarAcceso", new object[] {
                        _idMovil,
                        _pass});
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public void ValidarAccesoAsync(string _idMovil, string _pass) {
            this.ValidarAccesoAsync(_idMovil, _pass, null);
        }
        
        /// <remarks/>
        public void ValidarAccesoAsync(string _idMovil, string _pass, object userState) {
            if ((this.ValidarAccesoOperationCompleted == null)) {
                this.ValidarAccesoOperationCompleted = new System.Threading.SendOrPostCallback(this.OnValidarAccesoOperationCompleted);
            }
            this.InvokeAsync("ValidarAcceso", new object[] {
                        _idMovil,
                        _pass}, this.ValidarAccesoOperationCompleted, userState);
        }
        
        private void OnValidarAccesoOperationCompleted(object arg) {
            if ((this.ValidarAccesoCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ValidarAccesoCompleted(this, new ValidarAccesoCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://ws_derivacion_ivr.redsos.com.ar/_alertasRollback", RequestNamespace="http://ws_derivacion_ivr.redsos.com.ar/", ResponseNamespace="http://ws_derivacion_ivr.redsos.com.ar/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string _alertasRollback(string nroSvc) {
            object[] results = this.Invoke("_alertasRollback", new object[] {
                        nroSvc});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void _alertasRollbackAsync(string nroSvc) {
            this._alertasRollbackAsync(nroSvc, null);
        }
        
        /// <remarks/>
        public void _alertasRollbackAsync(string nroSvc, object userState) {
            if ((this._alertasRollbackOperationCompleted == null)) {
                this._alertasRollbackOperationCompleted = new System.Threading.SendOrPostCallback(this.On_alertasRollbackOperationCompleted);
            }
            this.InvokeAsync("_alertasRollback", new object[] {
                        nroSvc}, this._alertasRollbackOperationCompleted, userState);
        }
        
        private void On_alertasRollbackOperationCompleted(object arg) {
            if ((this._alertasRollbackCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this._alertasRollbackCompleted(this, new _alertasRollbackCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://ws_derivacion_ivr.redsos.com.ar/ObtenerAlertas", RequestNamespace="http://ws_derivacion_ivr.redsos.com.ar/", ResponseNamespace="http://ws_derivacion_ivr.redsos.com.ar/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string ObtenerAlertas() {
            object[] results = this.Invoke("ObtenerAlertas", new object[0]);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void ObtenerAlertasAsync() {
            this.ObtenerAlertasAsync(null);
        }
        
        /// <remarks/>
        public void ObtenerAlertasAsync(object userState) {
            if ((this.ObtenerAlertasOperationCompleted == null)) {
                this.ObtenerAlertasOperationCompleted = new System.Threading.SendOrPostCallback(this.OnObtenerAlertasOperationCompleted);
            }
            this.InvokeAsync("ObtenerAlertas", new object[0], this.ObtenerAlertasOperationCompleted, userState);
        }
        
        private void OnObtenerAlertasOperationCompleted(object arg) {
            if ((this.ObtenerAlertasCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ObtenerAlertasCompleted(this, new ObtenerAlertasCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.81.0")]
    public delegate void ObtenerServiciosDerivarCompletedEventHandler(object sender, ObtenerServiciosDerivarCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.81.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ObtenerServiciosDerivarCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ObtenerServiciosDerivarCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.81.0")]
    public delegate void ObtenerIntentosLlamadaCompletedEventHandler(object sender, ObtenerIntentosLlamadaCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.81.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ObtenerIntentosLlamadaCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ObtenerIntentosLlamadaCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.81.0")]
    public delegate void ObtenerIntentosConfirmaSMSCompletedEventHandler(object sender, ObtenerIntentosConfirmaSMSCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.81.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ObtenerIntentosConfirmaSMSCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ObtenerIntentosConfirmaSMSCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.81.0")]
    public delegate void ObtenerSMSaConfirmarCompletedEventHandler(object sender, ObtenerSMSaConfirmarCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.81.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ObtenerSMSaConfirmarCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ObtenerSMSaConfirmarCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.81.0")]
    public delegate void informarResultadosCompletedEventHandler(object sender, informarResultadosCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.81.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class informarResultadosCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal informarResultadosCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.81.0")]
    public delegate void ActualizarSvcCompletedEventHandler(object sender, ActualizarSvcCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.81.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ActualizarSvcCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ActualizarSvcCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public int Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.81.0")]
    public delegate void ObtenerFormularioCompletedEventHandler(object sender, ObtenerFormularioCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.81.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ObtenerFormularioCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ObtenerFormularioCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.81.0")]
    public delegate void ValidarAccesoCompletedEventHandler(object sender, ValidarAccesoCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.81.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ValidarAccesoCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ValidarAccesoCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public int Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.81.0")]
    public delegate void _alertasRollbackCompletedEventHandler(object sender, _alertasRollbackCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.81.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class _alertasRollbackCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal _alertasRollbackCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.81.0")]
    public delegate void ObtenerAlertasCompletedEventHandler(object sender, ObtenerAlertasCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.81.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ObtenerAlertasCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ObtenerAlertasCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591