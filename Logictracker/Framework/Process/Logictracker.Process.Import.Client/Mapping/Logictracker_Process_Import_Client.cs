﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.8000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=2.0.50727.3038.
// 
namespace Logictracker.Process.Import.Client.Mapping {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:Logictracker.Process.Import.Client")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:Logictracker.Process.Import.Client", IsNullable=false)]
    public partial class Configuration : object, System.ComponentModel.INotifyPropertyChanged {
        
        private Import[] importField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Import")]
        public Import[] Import {
            get {
                return this.importField;
            }
            set {
                this.importField = value;
                this.RaisePropertyChanged("Import");
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:Logictracker.Process.Import.Client")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:Logictracker.Process.Import.Client", IsNullable=false)]
    public partial class Import : object, System.ComponentModel.INotifyPropertyChanged {
        
        private DataSource dataSourceField;
        
        private Entity[] entityField;
        
        /// <remarks/>
        public DataSource DataSource {
            get {
                return this.dataSourceField;
            }
            set {
                this.dataSourceField = value;
                this.RaisePropertyChanged("DataSource");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Entity")]
        public Entity[] Entity {
            get {
                return this.entityField;
            }
            set {
                this.entityField = value;
                this.RaisePropertyChanged("Entity");
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:Logictracker.Process.Import.Client")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:Logictracker.Process.Import.Client", IsNullable=false)]
    public partial class DataSource : object, System.ComponentModel.INotifyPropertyChanged {
        
        private Parameter[] parameterField;
        
        private string typeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Parameter")]
        public Parameter[] Parameter {
            get {
                return this.parameterField;
            }
            set {
                this.parameterField = value;
                this.RaisePropertyChanged("Parameter");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
                this.RaisePropertyChanged("Type");
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:Logictracker.Process.Import.Client")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:Logictracker.Process.Import.Client", IsNullable=false)]
    public partial class Parameter : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string nameField;
        
        private string valueField;
        
        /// <remarks/>
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
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
                this.RaisePropertyChanged("Value");
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:Logictracker.Process.Import.Client")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:Logictracker.Process.Import.Client", IsNullable=false)]
    public partial class Entity : object, System.ComponentModel.INotifyPropertyChanged {
        
        private Operation operationField;
        
        private Property[] propertyField;
        
        private string typeField;
        
        /// <remarks/>
        public Operation Operation {
            get {
                return this.operationField;
            }
            set {
                this.operationField = value;
                this.RaisePropertyChanged("Operation");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Property")]
        public Property[] Property {
            get {
                return this.propertyField;
            }
            set {
                this.propertyField = value;
                this.RaisePropertyChanged("Property");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
                this.RaisePropertyChanged("Type");
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:Logictracker.Process.Import.Client")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:Logictracker.Process.Import.Client", IsNullable=false)]
    public partial class Operation : object, System.ComponentModel.INotifyPropertyChanged {
        
        private OperationProperty operationPropertyField;
        
        private OperationValue[] operationValueField;
        
        private OperationType typeField;
        
        private bool typeFieldSpecified;
        
        /// <remarks/>
        public OperationProperty OperationProperty {
            get {
                return this.operationPropertyField;
            }
            set {
                this.operationPropertyField = value;
                this.RaisePropertyChanged("OperationProperty");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("OperationValue")]
        public OperationValue[] OperationValue {
            get {
                return this.operationValueField;
            }
            set {
                this.operationValueField = value;
                this.RaisePropertyChanged("OperationValue");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public OperationType Type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
                this.RaisePropertyChanged("Type");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TypeSpecified {
            get {
                return this.typeFieldSpecified;
            }
            set {
                this.typeFieldSpecified = value;
                this.RaisePropertyChanged("TypeSpecified");
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:Logictracker.Process.Import.Client")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:Logictracker.Process.Import.Client", IsNullable=false)]
    public partial class OperationProperty : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string columnNameField;
        
        private int columnIndexField;
        
        private bool columnIndexFieldSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ColumnName {
            get {
                return this.columnNameField;
            }
            set {
                this.columnNameField = value;
                this.RaisePropertyChanged("ColumnName");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int ColumnIndex {
            get {
                return this.columnIndexField;
            }
            set {
                this.columnIndexField = value;
                this.RaisePropertyChanged("ColumnIndex");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ColumnIndexSpecified {
            get {
                return this.columnIndexFieldSpecified;
            }
            set {
                this.columnIndexFieldSpecified = value;
                this.RaisePropertyChanged("ColumnIndexSpecified");
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:Logictracker.Process.Import.Client")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:Logictracker.Process.Import.Client", IsNullable=false)]
    public partial class OperationValue : object, System.ComponentModel.INotifyPropertyChanged {
        
        private OperationType operationField;
        
        private string valueField;
        
        private bool caseSensitiveField;
        
        private bool caseSensitiveFieldSpecified;
        
        private bool defaultField;
        
        private bool defaultFieldSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public OperationType Operation {
            get {
                return this.operationField;
            }
            set {
                this.operationField = value;
                this.RaisePropertyChanged("Operation");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
                this.RaisePropertyChanged("Value");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool CaseSensitive {
            get {
                return this.caseSensitiveField;
            }
            set {
                this.caseSensitiveField = value;
                this.RaisePropertyChanged("CaseSensitive");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CaseSensitiveSpecified {
            get {
                return this.caseSensitiveFieldSpecified;
            }
            set {
                this.caseSensitiveFieldSpecified = value;
                this.RaisePropertyChanged("CaseSensitiveSpecified");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool Default {
            get {
                return this.defaultField;
            }
            set {
                this.defaultField = value;
                this.RaisePropertyChanged("Default");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DefaultSpecified {
            get {
                return this.defaultFieldSpecified;
            }
            set {
                this.defaultFieldSpecified = value;
                this.RaisePropertyChanged("DefaultSpecified");
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:Logictracker.Process.Import.Client")]
    public enum OperationType {
        
        /// <remarks/>
        AddOrModify,
        
        /// <remarks/>
        Add,
        
        /// <remarks/>
        Modify,
        
        /// <remarks/>
        Delete,
        
        /// <remarks/>
        Custom,
        
        /// <remarks/>
        None,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:Logictracker.Process.Import.Client")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:Logictracker.Process.Import.Client", IsNullable=false)]
    public partial class Property : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string nameField;
        
        private string columnNameField;
        
        private int columnIndexField;
        
        private bool columnIndexFieldSpecified;
        
        private string defaultField;
        
        /// <remarks/>
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
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ColumnName {
            get {
                return this.columnNameField;
            }
            set {
                this.columnNameField = value;
                this.RaisePropertyChanged("ColumnName");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int ColumnIndex {
            get {
                return this.columnIndexField;
            }
            set {
                this.columnIndexField = value;
                this.RaisePropertyChanged("ColumnIndex");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ColumnIndexSpecified {
            get {
                return this.columnIndexFieldSpecified;
            }
            set {
                this.columnIndexFieldSpecified = value;
                this.RaisePropertyChanged("ColumnIndexSpecified");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Default {
            get {
                return this.defaultField;
            }
            set {
                this.defaultField = value;
                this.RaisePropertyChanged("Default");
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
