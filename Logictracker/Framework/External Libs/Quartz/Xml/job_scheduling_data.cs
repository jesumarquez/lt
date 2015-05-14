namespace Quartz.Xml {
    /// <remarks/>
    [System.Xml.Serialization.XmlType(Namespace="http://quartznet.sourceforge.net/JobSchedulingData")]
    [System.Xml.Serialization.XmlRoot(Namespace="http://quartznet.sourceforge.net/JobSchedulingData", IsNullable=false)]
    public class quartz {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElement("job-listener")]
        public joblistenerType[] joblistener;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElement("calendar")]
        public calendarType[] calendar;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElement("job")]
        public jobType[] job;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string version;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute("overwrite-existing-jobs")]
        [System.ComponentModel.DefaultValue(true)]
        public bool overwriteexistingjobs = true;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlType(TypeName="job-listenerType", Namespace="http://quartznet.sourceforge.net/JobSchedulingData")]
    public class joblistenerType {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string type;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        [System.ComponentModel.DefaultValue("required")]
        public string name = "required";
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlType(Namespace="http://quartznet.sourceforge.net/JobSchedulingData")]
    [System.Xml.Serialization.XmlInclude(typeof(cronType))]
    [System.Xml.Serialization.XmlInclude(typeof(simpleType))]
    public abstract class abstractTriggerType {
        
        /// <remarks/>
        public string name;
        
        /// <remarks/>
        public string group;
        
        /// <remarks/>
        public string description;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElement("misfire-instruction")]
        public string misfireinstruction;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElement("calendar-name")]
        public string calendarname;
        
        /// <remarks/>
        [System.ComponentModel.DefaultValue(false)]
        public bool @volatile;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnore]
        public bool volatileSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElement("job-data-map")]
        public jobdatamapType jobdatamap;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlType(TypeName="job-data-mapType", Namespace="http://quartznet.sourceforge.net/JobSchedulingData")]
    public class jobdatamapType {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElement("entry")]
        public entryType[] entry;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlType(Namespace="http://quartznet.sourceforge.net/JobSchedulingData")]
    public class entryType {
        
        /// <remarks/>
        public string key;
        
        /// <remarks/>
        public string value;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlType(Namespace="http://quartznet.sourceforge.net/JobSchedulingData")]
    public class cronType : abstractTriggerType {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElement("job-name")]
        public string jobname;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElement("job-group")]
        public string jobgroup;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElement("start-time")]
        public System.DateTime starttime;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElement("end-time")]
        public System.DateTime endtime;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElement("cron-expression")]
        public string cronexpression;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElement("time-zone")]
        public string timezone;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlType(Namespace="http://quartznet.sourceforge.net/JobSchedulingData")]
    public class simpleType : abstractTriggerType {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElement("job-name")]
        public string jobname;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElement("job-group")]
        public string jobgroup;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElement("start-time")]
        public System.DateTime starttime;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElement("end-time")]
        public System.DateTime endtime;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnore]
        public bool endtimeSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElement("repeat-count", DataType="integer")]
        public string repeatcount;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElement("repeat-interval", DataType="nonNegativeInteger")]
        public string repeatinterval;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlType(Namespace="http://quartznet.sourceforge.net/JobSchedulingData")]
    public class triggerType {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElement("cron", typeof(cronType))]
        [System.Xml.Serialization.XmlElement("simple", typeof(simpleType))]
        public abstractTriggerType Item;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlType(TypeName="job-detailType", Namespace="http://quartznet.sourceforge.net/JobSchedulingData")]
    public class jobdetailType {
        
        /// <remarks/>
        public string name;
        
        /// <remarks/>
        public string group;
        
        /// <remarks/>
        public string description;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElement("job-type")]
        public string jobtype;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElement("job-listener-ref")]
        public string joblistenerref;
        
        /// <remarks/>
        [System.ComponentModel.DefaultValue(false)]
        public bool @volatile;
        
        /// <remarks/>
        [System.ComponentModel.DefaultValue(false)]
        public bool durable;
        
        /// <remarks/>
        [System.ComponentModel.DefaultValue(false)]
        public bool recover;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElement("job-data-map")]
        public jobdatamapType jobdatamap;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlType(Namespace="http://quartznet.sourceforge.net/JobSchedulingData")]
    public class jobType {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElement("job-detail")]
        public jobdetailType jobdetail;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElement("trigger")]
        public triggerType[] trigger;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlType(Namespace="http://quartznet.sourceforge.net/JobSchedulingData")]
    public class calendarType {
        
        /// <remarks/>
        public string name;
        
        /// <remarks/>
        public string description;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElement("base-calendar")]
        public calendarType basecalendar;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        public string type;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute]
        [System.ComponentModel.DefaultValue(false)]
        public bool replace;
    }
}
