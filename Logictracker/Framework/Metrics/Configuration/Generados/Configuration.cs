﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión del motor en tiempo de ejecución:2.0.50727.8009
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=2.0.50727.3038.
// 
namespace Logictracker.Metrics {
    using System.Xml.Serialization;
    
    
    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:Logictracker.Metrics.Configuration")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:Logictracker.Metrics.Configuration", IsNullable=false)]
    public partial class Configuration : object, System.ComponentModel.INotifyPropertyChanged {
        
        private MetricConfiguration[] metricConfigurationField;
        
        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute("MetricConfiguration")]
        public MetricConfiguration[] MetricConfiguration {
            get {
                return this.metricConfigurationField;
            }
            set {
                this.metricConfigurationField = value;
                this.RaisePropertyChanged("MetricConfiguration");
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
    
    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:Logictracker.Metrics.Configuration")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:Logictracker.Metrics.Configuration", IsNullable=false)]
    public partial class MetricConfiguration : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string classField;
        
        private string nameField;
        
        private bool enabledField;
        
        private System.DateTime startDateField;
        
        private bool startDateFieldSpecified;
        
        private System.DateTime endDateField;
        
        private bool endDateFieldSpecified;
        
        private System.DateTime intervalField;
        
        private bool publishCounterField;
        
        public MetricConfiguration() {
            this.enabledField = true;
            this.intervalField = new System.DateTime(6000000000);
            this.publishCounterField = false;
        }
        
        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Class {
            get {
                return this.classField;
            }
            set {
                this.classField = value;
                this.RaisePropertyChanged("Class");
            }
        }
        
        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
                this.RaisePropertyChanged("Name");
            }
        }
        
        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool Enabled {
            get {
                return this.enabledField;
            }
            set {
                this.enabledField = value;
                this.RaisePropertyChanged("Enabled");
            }
        }
        
        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime StartDate {
            get {
                return this.startDateField;
            }
            set {
                this.startDateField = value;
                this.RaisePropertyChanged("StartDate");
            }
        }
        
        /// <comentarios/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool StartDateSpecified {
            get {
                return this.startDateFieldSpecified;
            }
            set {
                this.startDateFieldSpecified = value;
                this.RaisePropertyChanged("StartDateSpecified");
            }
        }
        
        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime EndDate {
            get {
                return this.endDateField;
            }
            set {
                this.endDateField = value;
                this.RaisePropertyChanged("EndDate");
            }
        }
        
        /// <comentarios/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool EndDateSpecified {
            get {
                return this.endDateFieldSpecified;
            }
            set {
                this.endDateFieldSpecified = value;
                this.RaisePropertyChanged("EndDateSpecified");
            }
        }
        
        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="time")]
        [System.ComponentModel.DefaultValueAttribute(typeof(System.DateTime), "0001-01-01T00:10:00")]
        public System.DateTime Interval {
            get {
                return this.intervalField;
            }
            set {
                this.intervalField = value;
                this.RaisePropertyChanged("Interval");
            }
        }
        
        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool PublishCounter {
            get {
                return this.publishCounterField;
            }
            set {
                this.publishCounterField = value;
                this.RaisePropertyChanged("PublishCounter");
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
}
