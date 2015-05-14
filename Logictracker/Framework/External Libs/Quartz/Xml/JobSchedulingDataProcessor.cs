#region Usings

#if NET_20
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Common.Logging;
using Quartz.Util;
using NullableDateTime = System.Nullable<System.DateTime>;
#else
using Nullables;
#endif

#if NET_35
using TimeZone = System.TimeZoneInfo;
#endif

#endregion

namespace Quartz.Xml
{
	/// <summary> 
	/// Parses an XML file that declares Jobs and their schedules (Triggers).
	/// </summary>
	/// <remarks>
	/// <p>
	/// The xml document must conform to the format defined in
	/// "job_scheduling_data.xsd"
	/// </p>
	/// 
	/// <p>
	/// After creating an instance of this class, you should call one of the <see cref="ProcessFile()" />
	/// functions, after which you may call the <see cref="ScheduledJobs()" />
	/// function to get a handle to the defined Jobs and Triggers, which can then be
	/// scheduled with the <see cref="IScheduler" />. Alternatively, you could call
	/// the <see cref="ProcessFileAndScheduleJobs(IScheduler,bool)" /> function to do all of this
	/// in one step.
	/// </p>
	/// 
	/// <p>
	/// The same instance can be used again and again, with the list of defined Jobs
	/// being cleared each time you call a <see cref="ProcessFile()" /> method,
	/// however a single instance is not thread-safe.
	/// </p>
    /// </remarks>
	/// <author><a href="mailto:bonhamcm@thirdeyeconsulting.com">Chris Bonham</a></author>
	/// <author>James House</author>
	/// <author>Marko Lahma (.NET)</author>
	public class JobSchedulingDataProcessor
	{
		private readonly ILog log;
	    private readonly bool validateXml;
	    private readonly bool validateSchema;

		public const string PropertyQuartzSystemIdDir = "quartz.system.id.dir";
		public const string QuartzXmlFileName = "quartz_jobs.xml";
		public const string QuartzSchema = "http://quartznet.sourceforge.net/xml/job_scheduling_data.xsd";
		public const string QuartzXsdResourceName = "Quartz.Quartz.Xml.job_scheduling_data.xsd";
		
		protected const string ThreadLocalKeyScheduler = "quartz_scheduler";
		
		/// <summary> 
		/// XML Schema dateTime datatype format.
		/// <p>
		/// See <a href="http://www.w3.org/TR/2001/REC-xmlschema-2-20010502/#dateTime">
		/// http://www.w3.org/TR/2001/REC-xmlschema-2-20010502/#dateTime</a>
		/// </p>
		/// </summary>
		protected const string XsdDateFormat = "yyyy-MM-dd'T'hh:mm:ss";

		protected IDictionary scheduledJobs = new Hashtable();
		protected IList jobsToSchedule = new ArrayList();
		protected IList calsToSchedule = new ArrayList();
		protected IList listenersToSchedule = new ArrayList();

		protected ArrayList validationExceptions = new ArrayList();

		private bool overwriteExistingJobs = true;

		
		/// <summary> 
		/// Gets or sets whether to overwrite existing jobs.
		/// </summary>
		public virtual bool OverwriteExistingJobs
		{
			get { return overwriteExistingJobs; }
			set { overwriteExistingJobs = value; }
		}


        /// <summary>
        /// Gets the log.
        /// </summary>
        /// <value>The log.</value>
	    protected internal ILog Log
	    {
	        get { return log; }
	    }

	    /// <summary> 
        /// Returns a <see cref="IDictionary" /> of scheduled jobs.
		/// <p>
		/// The key is the job name and the value is a <see cref="JobSchedulingBundle" />
		/// containing the <see cref="JobDetail" /> and <see cref="Trigger" />.
		/// </p>
		/// </summary>
        /// <returns> a <see cref="IDictionary" /> of scheduled jobs.
		/// </returns>
		public virtual IDictionary ScheduledJobs
		{
			get { return scheduledJobs; }
		}


		/// <summary>
		/// Constructor for JobSchedulingDataProcessor.
		/// </summary>
		public JobSchedulingDataProcessor() : this(true, true)
		{
		}

		/// <summary>
		/// Constructor for JobSchedulingDataProcessor.
		/// </summary>
		/// <param name="validateXml">whether or not to validate XML.</param>
		/// <param name="validateSchema">whether or not to validate XML schema.</param>
		public JobSchedulingDataProcessor(bool validateXml, bool validateSchema)
		{
		    this.validateXml = validateXml;
		    this.validateSchema = validateSchema;
		    log = LogManager.GetLogger(GetType());
		}


		/// <summary> 
		/// Process the xml file in the default location (a file named
		/// "quartz_jobs.xml" in the current working directory).
		/// </summary>
		public virtual void ProcessFile()
		{
			ProcessFile(QuartzXmlFileName);
		}

		/// <summary>
		/// Process the xml file named <see param="fileName" />.
		/// </summary>
		/// <param name="fileName">meta data file name.</param>
		public virtual void ProcessFile(string fileName)
		{
			ProcessFile(fileName, fileName);
		}

		/// <summary>
		/// Process the xmlfile named <see param="fileName" /> with the given system
		/// ID.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <param name="systemId">The system id.</param>
		public virtual void ProcessFile(string fileName, string systemId)
		{
			Log.Info(string.Format(CultureInfo.InvariantCulture, "Parsing XML file: {0} with systemId: {1} validating: {2} validating schema: {3}", fileName, systemId, validateXml, validateSchema));
            using (var sr = new StreamReader(fileName))
            {
                ProcessInternal(sr.ReadToEnd());
            }
		}

		/// <summary>
		/// Process the xmlfile named <see param="fileName" /> with the given system
		/// ID.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="systemId">The system id.</param>
		public virtual void ProcessStream(Stream stream, string systemId)
		{
			Log.Info(string.Format(CultureInfo.InvariantCulture, "Parsing XML from stream with systemId: {0} validating: {1} validating schema: {2}", systemId, validateXml, validateSchema));
            using (var sr = new StreamReader(stream))
            {
                ProcessInternal(sr.ReadToEnd());
            }
		}

        protected internal virtual void ProcessInternal(string xml)
        {
            ClearValidationExceptions();

            scheduledJobs.Clear();
            jobsToSchedule.Clear();
            calsToSchedule.Clear();

            ValidateXmlIfNeeded(xml);
            
            // deserialize as object model
            var xs = new XmlSerializer(typeof(quartz));
            var data = (quartz) xs.Deserialize(new StringReader(xml));

            // process data
            
            // add calendars
            if (data.calendar != null)
            {
                foreach (var ct in data.calendar)
                {
                    var c = CreateCalendarFromXmlObject(ct);
                    AddCalendarToSchedule(c);
                }
            }

            // add job scheduling bundles
            ProcessJobs(data);

            if (data.joblistener != null)
            {
                // go through listeners
                foreach (var jt in data.joblistener)
                {
                    var listenerType = Type.GetType(jt.type);
                    if (listenerType == null)
                    {
                        throw new SchedulerConfigException("Unknown job listener type " + jt.type);
                    }
                    var listener = (IJobListener) ObjectUtils.InstantiateType(listenerType);
                    // set name of trigger with reflection, this might throw errors
                    var properties = new NameValueCollection();
                    properties.Add("Name", jt.name);

                    try
                    {
                        ObjectUtils.SetObjectProperties(listener, properties);
                    }
                    catch (Exception)
                    {
                        throw new SchedulerConfigException(string.Format("Could not set name for job listener of type '{0}', do you have public set method defined for property 'Name'?", jt.type));
                    }
                    AddListenerToSchedule(listener);
                }
            }
            MaybeThrowValidationException();
        }

	    private void ProcessJobs(quartz data)
	    {
            if (data.job == null)
            {
                // no jobs to process, file is empty
                return;
            }

	        foreach (var jt in data.job)
	        {
	            var jsb = new JobSchedulingBundle();
	            var j = jt.jobdetail;
	            var jobType = Type.GetType(j.jobtype);
                if (jobType == null)
                {
                    throw new SchedulerConfigException("Unknown job type " + j.jobtype);
                }

	            var jd = new JobDetail(j.name, j.group, jobType, j.@volatile, j.durable, j.recover);
	            jd.Description = j.description;

                if (j.joblistenerref != null && j.joblistenerref.Trim().Length > 0)
                {
                    jd.AddJobListener(j.joblistenerref);
                }
                
                jsb.JobDetail = jd;

                // read job data map
                if (j.jobdatamap != null && j.jobdatamap.entry != null)
                {
                    foreach (var entry in j.jobdatamap.entry)
                    {
                        jd.JobDataMap.Put(entry.key, entry.value);
                    }
                }

	            var tArr = jt.trigger;
	            foreach (var t in tArr)
	            {
	                Trigger trigger;
	                if (t.Item is cronType)
	                {
	                    var c = (cronType) t.Item;

                        var startTime = (c.starttime == DateTime.MinValue ? DateTime.UtcNow : c.starttime);
                        var endTime = (c.endtime == DateTime.MinValue ? null : (NullableDateTime)c.endtime);

	                    var jobName = c.jobname != null ? c.jobname : j.name;
	                    var jobGroup = c.jobgroup != null ? c.jobgroup : j.group;

                        var ct = new CronTrigger(
                            c.name,
                            c.group,
                            jobName,
                            jobGroup,
                            startTime,
                            endTime,
                            c.cronexpression);

	                    if (c.timezone != null && c.timezone.Trim().Length > 0)
	                    {
#if NET_35
                            ct.TimeZone = TimeZoneInfo.FindSystemTimeZoneById(c.timezone);
#else
	                        throw new ArgumentException(
	                            "Specifying time zone for cron trigger is only supported in .NET 3.5 builds");
#endif
	                    }
	                    trigger = ct;
	                }
	                else if (t.Item is simpleType)
	                {
	                    var s = (simpleType) t.Item;
	                    
	                    var startTime = (s.starttime == DateTime.MinValue ? DateTime.UtcNow : s.starttime);
                        var endTime = (s.endtime == DateTime.MinValue ? null : (NullableDateTime)s.endtime);

                        var jobName = s.jobname != null ? s.jobname : j.name;
                        var jobGroup = s.jobgroup != null ? s.jobgroup : j.group;

                        var st = new SimpleTrigger(
                            s.name, 
                            s.group, 
                            jobName, 
                            jobGroup,
                            startTime, 
                            endTime, 
                            ParseSimpleTriggerRepeatCount(s.repeatcount), 
                            TimeSpan.FromMilliseconds(Convert.ToInt64(s.repeatinterval, CultureInfo.InvariantCulture)));

	                    trigger = st;
	                }
	                else
	                {
	                    throw new ArgumentException("Unknown trigger type in XML");
	                }
	                trigger.CalendarName = t.Item.calendarname;
                    if (t.Item.misfireinstruction != null)
                    {
                        trigger.MisfireInstruction = ReadMisfireInstructionFromString(t.Item.misfireinstruction);
                    }
                    if (t.Item.jobdatamap != null && t.Item.jobdatamap.entry != null)
                    {
                        foreach (var entry in t.Item.jobdatamap.entry)
                        {
                            if (trigger.JobDataMap.Contains(entry.key))
                            {
                                Log.Warn("Overriding key '" + entry.key + "' with another value in same trigger job data map");
                            }
                            trigger.JobDataMap[entry.key] = entry.value;
                        }
                    }
	                jsb.Triggers.Add(trigger);
	            }

	            AddJobToSchedule(jsb);
	        }
	    }

	    private static int ParseSimpleTriggerRepeatCount(string repeatcount)
	    {
	        int value;
	        if (repeatcount == "RepeatIndefinitely")
	        {
	            value = SimpleTrigger.RepeatIndefinitely;
	        }
            else
	        {
                value = Convert.ToInt32(repeatcount, CultureInfo.InvariantCulture);
	        }

            return value;
	    }

	    private static int ReadMisfireInstructionFromString(string misfireinstruction)
	    {
	       var c = new Constants(typeof(MisfireInstruction), typeof(MisfireInstruction.CronTrigger), typeof(MisfireInstruction.SimpleTrigger));
	       return c.AsNumber(misfireinstruction);
	    }

	    private static CalendarBundle CreateCalendarFromXmlObject(calendarType ct)
	    {
            var c = new CalendarBundle(); 
            
            // set type name first as it creates the actual inner instance
	        c.TypeName = ct.type;
            c.Description = ct.description;
            c.CalendarName = ct.name;
            c.Replace = ct.replace;

            if (ct.basecalendar != null)
            {
                c.CalendarBase = CreateCalendarFromXmlObject(ct.basecalendar);
            }
            return c;
	    }

	    private void ValidateXmlIfNeeded(string xml)
	    {
            if (validateXml)
            {
                // stream to validate
                using (var stringReader = new StringReader(xml))
                {
                    var xmlr = new XmlTextReader(stringReader);
                    var xmlvread = new XmlValidatingReader(xmlr);

                    // Set the validation event handler
                    xmlvread.ValidationEventHandler += XmlValidationCallBack;

                    // Read XML data
                    while (xmlvread.Read()) { }

                    //Close the reader.
                    xmlvread.Close();
                }
            }
	    }

	    private void XmlValidationCallBack(object sender, ValidationEventArgs e)
	    {
	        validationExceptions.Add(e.Exception);
	    }


	    /// <summary> 
		/// Process the xml file in the default location, and schedule all of the
		/// jobs defined within it.
		/// </summary>
        public virtual void ProcessFileAndScheduleJobs(IScheduler sched, bool overwriteExistingJobs)
		{
            ProcessFileAndScheduleJobs(QuartzXmlFileName, sched, overwriteExistingJobs);
		}

		/// <summary>
		/// Process the xml file in the given location, and schedule all of the
		/// jobs defined within it.
		/// </summary>
		/// <param name="fileName">meta data file name.</param>
		/// <param name="sched">The scheduler.</param>
		/// <param name="overwriteExistingJobs">if set to <c>true</c> overwrite existing jobs.</param>
		public virtual void ProcessFileAndScheduleJobs(string fileName, IScheduler sched, bool overwriteExistingJobs)
		{
			ProcessFileAndScheduleJobs(fileName, fileName, sched, overwriteExistingJobs);
		}

		/// <summary>
		/// Process the xml file in the given location, and schedule all of the
		/// jobs defined within it.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <param name="systemId">The system id.</param>
		/// <param name="sched">The sched.</param>
        /// <param name="overwriteExistingJobs">if set to <c>true</c> [over write existing jobs].</param>
		public virtual void ProcessFileAndScheduleJobs(string fileName, string systemId, IScheduler sched,
                                                       bool overwriteExistingJobs)
		{
			LogicalThreadContext.SetData(ThreadLocalKeyScheduler, sched);
			try
			{
				ProcessFile(fileName, systemId);
                ScheduleJobs(ScheduledJobs, sched, overwriteExistingJobs);
			}
			finally
			{
				LogicalThreadContext.FreeNamedDataSlot(ThreadLocalKeyScheduler);
			}
		}

		/// <summary>
		/// Add the Jobs and Triggers defined in the given map of <see cref="JobSchedulingBundle" />
		/// s to the given scheduler.
		/// </summary>
		/// <param name="jobBundles">The job bundles.</param>
		/// <param name="sched">The sched.</param>
		/// <param name="overwriteExistingJobs">if set to <c>true</c> [over write existing jobs].</param>
		public virtual void ScheduleJobs(IDictionary jobBundles, IScheduler sched, bool overwriteExistingJobs)
		{
			Log.Info(string.Format(CultureInfo.InvariantCulture, "Scheduling {0} parsed jobs.", jobsToSchedule.Count));

			foreach (CalendarBundle bndle in calsToSchedule)
			{
				AddCalendar(sched, bndle);
			}

			foreach (JobSchedulingBundle bndle in jobsToSchedule)
			{
				ScheduleJob(bndle, sched, overwriteExistingJobs);
			}

			foreach (IJobListener listener in listenersToSchedule)
			{
				Log.Info(string.Format(CultureInfo.InvariantCulture, "adding listener {0} of type {1}", listener.Name, listener.GetType().FullName));
				sched.AddJobListener(listener);
			}
			Log.Info(string.Format(CultureInfo.InvariantCulture, "{0} scheduled jobs.", jobBundles.Count));
		}

		/// <summary>
		/// Returns a <see cref="JobSchedulingBundle" /> for the job name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>
		/// a <see cref="JobSchedulingBundle" /> for the job name.
		/// </returns>
		public virtual JobSchedulingBundle GetScheduledJob(string name)
		{
			return (JobSchedulingBundle) ScheduledJobs[name];
		}

		/// <summary>
        /// Returns an <see cref="Stream" /> from the fileName as a resource.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <returns>
        /// an <see cref="Stream" /> from the fileName as a resource.
		/// </returns>
		protected virtual Stream GetInputStream(string fileName)
		{
			return new StreamReader(fileName).BaseStream;
		}

		/// <summary>
		/// Schedules a given job and trigger (both wrapped by a <see cref="JobSchedulingBundle" />).
		/// </summary>
		/// <param name="job">job wrapper.</param>
		/// <exception cref="SchedulerException">
		/// if the Job or Trigger cannot be added to the Scheduler, or
		/// there is an internal Scheduler error.
		/// </exception>
		public virtual void ScheduleJob(JobSchedulingBundle job)
		{
			ScheduleJob(job, (IScheduler) LogicalThreadContext.GetData(ThreadLocalKeyScheduler), OverwriteExistingJobs);
		}


		public virtual void AddJobToSchedule(JobSchedulingBundle job)
		{
			jobsToSchedule.Add(job);
		}

		public virtual void AddCalendarToSchedule(CalendarBundle cal)
		{
			calsToSchedule.Add(cal);
		}

		public virtual void AddListenerToSchedule(IJobListener listener)
		{
			listenersToSchedule.Add(listener);
		}

		/// <summary>
		/// Schedules a given job and trigger (both wrapped by a <see cref="JobSchedulingBundle" />).
		/// </summary>
		/// <param name="job">The job.</param>
		/// <param name="sched">The sched.</param>
		/// <param name="localOverWriteExistingJobs">if set to <c>true</c> [local over write existing jobs].</param>
		/// <exception cref="SchedulerException"> 
		/// if the Job or Trigger cannot be added to the Scheduler, or
		/// there is an internal Scheduler error.
		/// </exception>
		public virtual void ScheduleJob(JobSchedulingBundle job, IScheduler sched, bool localOverWriteExistingJobs)
		{
			if ((job != null) && job.Valid)
			{
				var detail = job.JobDetail;

				var dupeJ = sched.GetJobDetail(detail.Name, detail.Group);

				if ((dupeJ != null) && !localOverWriteExistingJobs)
				{
					Log.Info("Not overwriting existing job: " + dupeJ.FullName);
					return;
				}

				if (dupeJ != null)
				{
					Log.Info(string.Format(CultureInfo.InvariantCulture, "Replacing job: {0}", detail.FullName));
				}
				else
				{
					Log.Info(string.Format(CultureInfo.InvariantCulture, "Adding job: {0}", detail.FullName));
				}

				if (job.Triggers.Count == 0 && !job.JobDetail.Durable)
				{
					throw new SchedulerException("A Job defined without any triggers must be durable");
				}
				
				sched.AddJob(detail, true);

					
				foreach(Trigger trigger in job.Triggers)
				{
					var dupeT = sched.GetTrigger(trigger.Name, trigger.Group);

					trigger.JobName = detail.Name;
					trigger.JobGroup = detail.Group;

					if (trigger.StartTimeUtc == DateTime.MinValue)
					{
						trigger.StartTimeUtc = DateTime.UtcNow;
					}

					if (dupeT != null)
					{
						Log.Debug(string.Format(CultureInfo.InvariantCulture, "Rescheduling job: {0} with updated trigger: {1}", detail.FullName, trigger.FullName));
						if (!dupeT.JobGroup.Equals(trigger.JobGroup) || !dupeT.JobName.Equals(trigger.JobName))
						{
							Log.Warn("Possibly duplicately named triggers in jobs xml file!");
						}
						sched.RescheduleJob(trigger.Name, trigger.Group, trigger);
					}
					else
					{
						Log.Debug(string.Format(CultureInfo.InvariantCulture, "Scheduling job: {0} with trigger: {1}", detail.FullName, trigger.FullName));
						sched.ScheduleJob(trigger);
					}
				}

				AddScheduledJob(job);
			}
		}

		/// <summary>
		/// Adds a scheduled job.
		/// </summary>
		/// <param name="job">The job.</param>
		protected virtual void AddScheduledJob(JobSchedulingBundle job)
		{
			scheduledJobs[job.FullName] = job;
		}

		/// <summary>
		/// Adds a calendar.
		/// </summary>
		/// <param name="sched">The sched.</param>
		/// <param name="calendarBundle">calendar bundle.</param>
		/// <throws>  SchedulerException if the Calendar cannot be added to the Scheduler, or </throws>
		public virtual void AddCalendar(IScheduler sched, CalendarBundle calendarBundle)
		{
			sched.AddCalendar(calendarBundle.CalendarName, calendarBundle.Calendar, calendarBundle.Replace, true);
		}




		/// <summary>
		/// Adds a detected validation exception.
		/// </summary>
		/// <param name="e">The exception.</param>
		protected virtual void AddValidationException(XmlException e)
		{
			validationExceptions.Add(e);
		}

		/// <summary>
		/// Resets the the number of detected validation exceptions.
		/// </summary>
		protected virtual void ClearValidationExceptions()
		{
			validationExceptions.Clear();
		}

		/// <summary>
		/// Throws a ValidationException if the number of validationExceptions
		/// detected is greater than zero.
		/// </summary>
		/// <exception cref="ValidationException"> 
		/// DTD validation exception.
		/// </exception>
		protected virtual void MaybeThrowValidationException()
		{
			if (validationExceptions.Count > 0)
			{
				throw new ValidationException(validationExceptions);
			}
		}


	}

    /// <summary>
    /// Helper class to map constant names to their values.
    /// </summary>
    internal class Constants
    {
        private readonly Type[] types;

        public Constants(params Type[] reflectedTypes)
        {
            types = reflectedTypes;
        }

        public int AsNumber(string field)
        {
            foreach (var type in types)
            {
                var fi = type.GetField(field);
                if (fi != null)
                {
                    return Convert.ToInt32(fi.GetValue(null), CultureInfo.InvariantCulture);
                }
            }

            // not found
            throw new Exception(string.Format(CultureInfo.InvariantCulture, "Unknown field '{0}'", field));
        }
    }
}
