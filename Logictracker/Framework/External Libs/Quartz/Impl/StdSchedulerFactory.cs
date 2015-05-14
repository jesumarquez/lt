#region Usings

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using Common.Logging;
using Quartz.Collection;
using Quartz.Core;
using Quartz.Impl.AdoJobStore;
using Quartz.Impl.AdoJobStore.Common;
using Quartz.Simpl;
using Quartz.Spi;
using Quartz.Util;

#endregion

namespace Quartz.Impl
{
    /// <summary>
    /// An implementation of <see cref="ISchedulerFactory" /> that
    /// does all of it's work of creating a <see cref="QuartzScheduler" /> instance
    /// based on the contenents of a properties file.
    /// </summary>
    /// <remarks>
    /// <p>
    /// By default a properties are loaded from App.config's quartz section. 
    /// If that fails, then the "quartz.properties"
    /// file located (as a embedded resource) in Quartz.dll is loaded. If you
    /// wish to use a file other than these defaults, you must define the system
    /// property 'quartz.properties' to point to the file you want.
    /// </p>
    /// 
    /// <p>
    /// See the sample properties that are distributed with Quartz for
    /// information about the various settings available within the file.
    /// </p>
    /// 
    /// <p>
    /// Alternativly, you can explicitly Initialize the factory by calling one of
    /// the <see cref="Initialize()" /> methods before calling <see cref="GetScheduler()" />.
    /// </p>
    /// 
    /// <p>
    /// Instances of the specified <see cref="IJobStore" />,
    /// <see cref="IThreadPool" />, classes will be created
    /// by name, and then any additional properties specified for them in the config
    /// file will be set on the instance by calling an equivalent 'set' method. For
    /// example if the properties file contains the property 'quartz.jobStore.
    /// myProp = 10' then after the JobStore class has been instantiated, the property
    /// 'MyProp' will be set with the value. Type conversion to primitive CLR types
    /// (int, long, float, double, boolean, enum and string) are performed before calling
    /// the property's setter method.
    /// </p>
    /// </remarks>
    /// <author>James House</author>
    /// <author>Anthony Eden</author>
    /// <author>Mohammad Rezaei</author>
    /// <author>Marko Lahma (.NET)</author>
    public class StdSchedulerFactory : ISchedulerFactory
    {
        public const string PropertiesFile = "quartz.config";
        public const string PropertySchedulerInstanceName = "quartz.scheduler.instanceName";
        public const string PropertySchedulerInstanceId = "quartz.scheduler.instanceId";
        public const string PropertySchedulerInstanceIdGeneratorPrefix = "quartz.scheduler.instanceIdGenerator";
        public const string PropertySchedulerInstanceIdGeneratorType = PropertySchedulerInstanceIdGeneratorPrefix + ".type";
        public const string PropertySchedulerThreadName = "quartz.scheduler.threadName";
        public const string PropertySchedulerExporterPrefix = "quartz.scheduler.exporter";
        public const string PropertySchedulerExporterType = PropertySchedulerExporterPrefix + ".type";
        public const string PropertySchedulerProxy = "quartz.scheduler.proxy";
        public const string PropertySchedulerProxyAddress = "quartz.scheduler.proxy.address";
        public const string PropertySchedulerIdleWaitTime = "quartz.scheduler.idleWaitTime";
        public const string PropertySchedulerDbFailureRetryInterval = "quartz.scheduler.dbFailureRetryInterval";
        public const string PropertySchedulerMakeSchedulerThreadDaemon = "quartz.scheduler.makeSchedulerThreadDaemon";
        public const string PropertySchedulerTypeLoadHelperType = "quartz.scheduler.typeLoadHelper.type";
        public const string PropertySchedulerJobFactoryType = "quartz.scheduler.jobFactory.type";
        public const string PropertySchedulerJobFactoryPrefix = "quartz.scheduler.jobFactory";
        public const string PropertySchedulerContextPrefix = "quartz.context.key";
        public const string PropertyThreadPoolPrefix = "quartz.threadPool";
        public const string PropertyThreadPoolType = "quartz.threadPool.type";
        public const string PropertyJobStorePrefix = "quartz.jobStore";
        public const string PropertyJobStoreLockHandlerPrefix = PropertyJobStorePrefix + ".lockHandler";
        public const string PropertyJobStoreLockHandlerType = PropertyJobStoreLockHandlerPrefix + ".type";
        public const string PropertyTablePrefix = "tablePrefix";
        public const string PropertyJobStoreType = "quartz.jobStore.type";
        public const string PropertyJobStoreUseProperties = "quartz.jobStore.useProperties";
        public const string PropertyDataSourcePrefix = "quartz.dataSource";
        public const string PropertyDbProviderType = "connectionProvider.type";
        public const string PropertyDataSourceProvider = "provider";
        public const string PropertyDataSourceConnectionString = "connectionString";
        public const string PropertyDataSourceConnectionStringName = "connectionStringName";
        public const string PropertyDataSourceValidationQuery = "validationQuery";
        public const string PropertyPluginPrefix = "quartz.plugin";
        public const string PropertyPluginType = "type";
        public const string PropertyJobListenerPrefix = "quartz.jobListener";
        public const string PropertyTriggerListenerPrefix = "quartz.triggerListener";
        public const string PropertyListenerType = "type";
        public const string DefaultInstanceId = "NON_CLUSTERED";
        public const string AutoGenerateInstanceId = "AUTO";
        private SchedulerException initException;

        private PropertiesParser cfg;

        private static readonly ILog Log = LogManager.GetLogger(typeof (StdSchedulerFactory));

        private string SchedulerName
        {
            get { return cfg.GetStringProperty(PropertySchedulerInstanceName, "QuartzScheduler"); }
        }

        private string SchedulerInstId
        {
            get { return cfg.GetStringProperty(PropertySchedulerInstanceId, DefaultInstanceId); }
        }

        /// <summary>
        /// Returns a handle to the default Scheduler, creating it if it does not
        /// yet exist.
        /// </summary>
        /// <seealso cref="Initialize()">
        /// </seealso>
        public static IScheduler DefaultScheduler
        {
            get
            {
                var fact = new StdSchedulerFactory();

                return fact.GetScheduler();
            }
        }

        /// <summary> <p>
        /// Returns a handle to all known Schedulers (made by any
        /// StdSchedulerFactory instance.).
        /// </p>
        /// </summary>
        public virtual ICollection AllSchedulers
        {
            get { return SchedulerRepository.Instance.LookupAll(); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StdSchedulerFactory"/> class.
        /// </summary>
        public StdSchedulerFactory()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StdSchedulerFactory"/> class.
        /// </summary>
        /// <param name="props">The props.</param>
        public StdSchedulerFactory(NameValueCollection props)
        {
            Initialize(props);
        }

        /// <summary>
        /// Initialize the <see cref="ISchedulerFactory" /> with
        /// the contenents of a Properties file.
        /// 
        /// <p>
        /// By default a properties file named "quartz.properties" is loaded from
        /// the 'current working directory'. If that fails, then the
        /// "quartz.properties" file located (as a resource) in the org/quartz
        /// package is loaded. If you wish to use a file other than these defaults,
        /// you must define the system property 'quartz.properties' to point to
        /// the file you want.
        /// </p>
        /// </summary>
        public void Initialize()
        {
            // short-circuit if already initialized
            if (cfg != null)
            {
                return;
            }
            if (initException != null)
            {
                throw initException;
            }

#if NET_20
            var props = (NameValueCollection) ConfigurationManager.GetSection("quartz");
#else
			NameValueCollection props = (NameValueCollection) ConfigurationSettings.GetConfig("quartz");
#endif
            var requestedFile = Environment.GetEnvironmentVariable(PropertiesFile);
            var propFileName = requestedFile != null && requestedFile.Trim().Length > 0 ? requestedFile : "~/quartz.config";

            // check for specials
            propFileName = FileUtil.ResolveFile(propFileName);

            if (props == null && File.Exists(propFileName))
            {
                // file system
                try
                {
                    var pp = PropertiesParser.ReadFromFileResource(propFileName);
                    props = pp.UnderlyingProperties;
                    Log.Info(string.Format("Quartz.NET properties loaded from configuration file '{0}'", propFileName));
                }
                catch (Exception ex)
                {
                    Log.Error(
                        string.Format(CultureInfo.InvariantCulture, 
                        "Could not load properties for Quartz from file {0}: {1}",
                        propFileName,              
                        ex.Message), ex);
                }

            }
            if (props == null)
            {
                // read from assembly
                try
                {
                    var pp = PropertiesParser.ReadFromEmbeddedAssemblyResource("Quartz.quartz.config");
                    props = pp.UnderlyingProperties;
                    Log.Info("Default Quartz.NET properties loaded from embedded resource file");
                }
                catch (Exception ex)
                {
                    Log.Error(
                        string.Format(CultureInfo.InvariantCulture, "Could not load default properties for Quartz from Quartz assembly: {0}",
                                      ex.Message), ex);
                }
            }
            if (props == null)
            {
                throw new SchedulerConfigException(
                    @"Could not find <quartz> configuration section from your application config or load default configuration from assembly.
Please add configuration to your application config file to correctly initialize Quartz.");
            }
            Initialize(OverrideWithSysProps(props));
        }

        /// <summary>
        /// Creates a new name value collection and overrides its values
        /// with system values (environment variables).
        /// </summary>
        /// <param name="props">The base properties to override.</param>
        /// <returns>A new NameValueCollection instance.</returns>
        private static NameValueCollection OverrideWithSysProps(NameValueCollection props)
        {
            var retValue = new NameValueCollection(props);
            var keys = Environment.GetEnvironmentVariables().Keys;

            foreach (string key in keys)
            {
                retValue.Set(key, props[key]);
            }
            return retValue;
        }


        /// <summary> 
        /// Initialize the <see cref="ISchedulerFactory" /> with
        /// the contenents of the given Properties object.
        /// </summary>
        public virtual void Initialize(NameValueCollection props)
        {
            cfg = new PropertiesParser(props);
        }

        /// <summary>  </summary>
        private IScheduler Instantiate()
        {
            if (cfg == null)
            {
                Initialize();
            }

            if (initException != null)
            {
                throw initException;
            }

            ISchedulerExporter exporter = null;
            IJobStore js;
            IThreadPool tp;
            QuartzScheduler qs;
            SchedulingContext schedCtxt;
            DBConnectionManager dbMgr = null;
            string instanceIdGeneratorType = null;
            NameValueCollection tProps;
            var autoId = false;
            var idleWaitTime = TimeSpan.Zero;
            var dbFailureRetry = TimeSpan.Zero;
            string typeLoadHelperType;
            string jobFactoryType;

            var schedRep = SchedulerRepository.Instance;

            // Get Scheduler Properties
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            var schedName = cfg.GetStringProperty(PropertySchedulerInstanceName, "QuartzScheduler");
            var threadName =
                cfg.GetStringProperty(PropertySchedulerThreadName, string.Format(CultureInfo.InvariantCulture, "{0}_QuartzSchedulerThread", schedName));
            var schedInstId = cfg.GetStringProperty(PropertySchedulerInstanceId, DefaultInstanceId);

            if (schedInstId.Equals(AutoGenerateInstanceId))
            {
                autoId = true;
                instanceIdGeneratorType =
                    cfg.GetStringProperty(PropertySchedulerInstanceIdGeneratorType,
                                          "Quartz.Simpl.SimpleInstanceIdGenerator, Quartz");
            }


            typeLoadHelperType =
                cfg.GetStringProperty(PropertySchedulerTypeLoadHelperType,
                                      "Quartz.Simpl.CascadingClassLoadHelper, Quartz");
            jobFactoryType = cfg.GetStringProperty(PropertySchedulerJobFactoryType, null);

            idleWaitTime = cfg.GetTimeSpanProperty(PropertySchedulerIdleWaitTime, idleWaitTime);
            dbFailureRetry = cfg.GetTimeSpanProperty(PropertySchedulerDbFailureRetryInterval, dbFailureRetry);
            var makeSchedulerThreadDaemon = cfg.GetBooleanProperty(PropertySchedulerMakeSchedulerThreadDaemon);

            var schedCtxtProps = cfg.GetPropertyGroup(PropertySchedulerContextPrefix, true);

            var proxyScheduler = cfg.GetBooleanProperty(PropertySchedulerProxy, false);
            // If Proxying to remote scheduler, short-circuit here...
            // ~~~~~~~~~~~~~~~~~~
            if (proxyScheduler)
            {
                if (autoId)
                {
                    schedInstId = DefaultInstanceId;
                }

                schedCtxt = new SchedulingContext();
                schedCtxt.InstanceId = schedInstId;

                var uid = QuartzSchedulerResources.GetUniqueIdentifier(schedName, schedInstId);

                var remoteScheduler = new RemoteScheduler(schedCtxt, uid);
                var remoteSchedulerAddress = cfg.GetStringProperty(PropertySchedulerProxyAddress);
                remoteScheduler.RemoteSchedulerAddress = remoteSchedulerAddress;
                schedRep.Bind(remoteScheduler);

                return remoteScheduler;
            }

            // Create type load helper
            ITypeLoadHelper loadHelper;
            try
            {
                loadHelper = (ITypeLoadHelper)ObjectUtils.InstantiateType(LoadType(typeLoadHelperType));
            }
            catch (Exception e)
            {
                throw new SchedulerConfigException(
                    string.Format(CultureInfo.InvariantCulture, "Unable to instantiate type load helper: {0}", e.Message), e);
            }
            loadHelper.Initialize();

            IJobFactory jobFactory = null;
            if (jobFactoryType != null)
            {
                try
                {
                    jobFactory = (IJobFactory) ObjectUtils.InstantiateType(loadHelper.LoadType(jobFactoryType));
                }
                catch (Exception e)
                {
                    throw new SchedulerConfigException(
                        string.Format(CultureInfo.InvariantCulture, "Unable to Instantiate JobFactory: {0}", e.Message), e);
                }

                tProps = cfg.GetPropertyGroup(PropertySchedulerJobFactoryPrefix, true);
                try
                {
                    ObjectUtils.SetObjectProperties(jobFactory, tProps);
                }
                catch (Exception e)
                {
                    initException =
                        new SchedulerException(
                            string.Format(CultureInfo.InvariantCulture, "JobFactory of type '{0}' props could not be configured.", jobFactoryType), e);
                    initException.ErrorCode = SchedulerException.ErrorBadConfiguration;
                    throw initException;
                }
            }

            IInstanceIdGenerator instanceIdGenerator = null;
            if (instanceIdGeneratorType != null)
            {
                try
                {
                    instanceIdGenerator =
                        (IInstanceIdGenerator)
                        ObjectUtils.InstantiateType(loadHelper.LoadType(instanceIdGeneratorType));
                }
                catch (Exception e)
                {
                    throw new SchedulerConfigException(
                        string.Format(CultureInfo.InvariantCulture, "Unable to Instantiate InstanceIdGenerator: {0}", e.Message), e);
                }
                tProps = cfg.GetPropertyGroup(PropertySchedulerInstanceIdGeneratorPrefix, true);
                try
                {
                    ObjectUtils.SetObjectProperties(instanceIdGenerator, tProps);
                }
                catch (Exception e)
                {
                    initException =
                        new SchedulerException(
                            string.Format(CultureInfo.InvariantCulture, "InstanceIdGenerator of type '{0}' props could not be configured.",
                                          instanceIdGeneratorType), e);
                    initException.ErrorCode = SchedulerException.ErrorBadConfiguration;
                    throw initException;
                }
            }

            // Get ThreadPool Properties
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            var tpType = cfg.GetStringProperty(PropertyThreadPoolType, typeof(SimpleThreadPool).AssemblyQualifiedName);

            if (tpType == null)
            {
                initException =
                    new SchedulerException("ThreadPool type not specified. ", SchedulerException.ErrorBadConfiguration);
                throw initException;
            }

            try
            {
                tp = (IThreadPool) ObjectUtils.InstantiateType(loadHelper.LoadType(tpType));
            }
            catch (Exception e)
            {
                initException =
                    new SchedulerException(string.Format(CultureInfo.InvariantCulture, "ThreadPool type '{0}' could not be instantiated.", tpType),
                                           e);
                initException.ErrorCode = SchedulerException.ErrorBadConfiguration;
                throw initException;
            }
            tProps = cfg.GetPropertyGroup(PropertyThreadPoolPrefix, true);
            try
            {
                ObjectUtils.SetObjectProperties(tp, tProps);
            }
            catch (Exception e)
            {
                initException =
                    new SchedulerException(
                        string.Format(CultureInfo.InvariantCulture, "ThreadPool type '{0}' props could not be configured.", tpType), e);
                initException.ErrorCode = SchedulerException.ErrorBadConfiguration;
                throw initException;
            }

                        // Set up any DataSources
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            var dsNames = cfg.GetPropertyGroups(PropertyDataSourcePrefix);
            for (var i = 0; i < dsNames.Length; i++)
            {
                var pp =
                    new PropertiesParser(
                        cfg.GetPropertyGroup(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", PropertyDataSourcePrefix, dsNames[i]), true));

                var cpType = pp.GetStringProperty(PropertyDbProviderType, null);

                // custom connectionProvider...
                if (cpType != null)
                {
                    IDbProvider cp;
                    try
                    {
                        cp = (IDbProvider) ObjectUtils.InstantiateType(loadHelper.LoadType(cpType));
                    }
                    catch (Exception e)
                    {
                        initException =
                            new SchedulerException(
                                string.Format(CultureInfo.InvariantCulture, "ConnectionProvider of type '{0}' could not be instantiated.", cpType), e);
                        initException.ErrorCode = SchedulerException.ErrorBadConfiguration;
                        throw initException;
                    }

                    try
                    {
                        // remove the type name, so it isn't attempted to be set
                        pp.UnderlyingProperties.Remove(PropertyDbProviderType);

                        ObjectUtils.SetObjectProperties(cp, pp.UnderlyingProperties);
                    }
                    catch (Exception e)
                    {
                        initException =
                            new SchedulerException(
                                string.Format(CultureInfo.InvariantCulture, "ConnectionProvider type '{0}' props could not be configured.", cpType),
                                e);
                        initException.ErrorCode = SchedulerException.ErrorBadConfiguration;
                        throw initException;
                    }

                    dbMgr = DBConnectionManager.Instance;
                    dbMgr.AddConnectionProvider(dsNames[i], cp);
                }
                else
                {
                    var dsProvider = pp.GetStringProperty(PropertyDataSourceProvider, null);
                    var dsConnectionString = pp.GetStringProperty(PropertyDataSourceConnectionString, null);
                    var dsConnectionStringName = pp.GetStringProperty(PropertyDataSourceConnectionStringName, null);

                    if (dsConnectionString == null && dsConnectionStringName != null && dsConnectionStringName.Length > 0)
                    {
#if NET_20
                        var connectionStringSettings = ConfigurationManager.ConnectionStrings[dsConnectionStringName];
                        if (connectionStringSettings == null)
                        {
                            initException =
                                new SchedulerException(string.Format(CultureInfo.InvariantCulture, "Named connection string '{0}' not found for DataSource: {1}", dsConnectionStringName,  dsNames[i]));
                            throw initException;
                        }
                        dsConnectionString = connectionStringSettings.ConnectionString;
#endif
                    }

                    if (dsProvider == null)
                    {
                        initException =
                            new SchedulerException(string.Format(CultureInfo.InvariantCulture, "Provider not specified for DataSource: {0}", dsNames[i]));
                        throw initException;
                    }
                    if (dsConnectionString == null)
                    {
                        initException =
                            new SchedulerException(string.Format(CultureInfo.InvariantCulture, "Connection string not specified for DataSource: {0}", dsNames[i]));
                        throw initException;
                    }
                    try
                    {
                        var dbp = new DbProvider(dsProvider, dsConnectionString);
						
                        dbMgr = DBConnectionManager.Instance;
                        dbMgr.AddConnectionProvider(dsNames[i], dbp);
                    }
                    catch (Exception exception)
                    {
                        initException =
                            new SchedulerException(string.Format(CultureInfo.InvariantCulture, "Could not Initialize DataSource: {0}", dsNames[i]),
                                                   exception);
                        throw initException;
                    }
                }
            }

            // Get JobStore Properties
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            var jsType = cfg.GetStringProperty(PropertyJobStoreType, typeof (RAMJobStore).FullName);

            if (jsType == null)
            {
                initException =
                    new SchedulerException("JobStore type not specified. ", SchedulerException.ErrorBadConfiguration);
                throw initException;
            }

            try
            {
                js = (IJobStore) ObjectUtils.InstantiateType(loadHelper.LoadType(jsType));
            }
            catch (Exception e)
            {
                initException =
                    new SchedulerException(string.Format(CultureInfo.InvariantCulture, "JobStore of type '{0}' could not be instantiated.", jsType), e);
                initException.ErrorCode = SchedulerException.ErrorBadConfiguration;
                throw initException;
            }
            
            tProps =
                cfg.GetPropertyGroup(PropertyJobStorePrefix, true, new[] {PropertyJobStoreLockHandlerPrefix});
            
            try
            {
                ObjectUtils.SetObjectProperties(js, tProps);
            }
            catch (Exception e)
            {
                initException =
                    new SchedulerException(
                        string.Format(CultureInfo.InvariantCulture, "JobStore type '{0}' props could not be configured.", jsType), e);
                initException.ErrorCode = SchedulerException.ErrorBadConfiguration;
                throw initException;
            }


            if (js is JobStoreSupport)
            {
                ((JobStoreSupport) js).InstanceId = schedInstId;
                ((JobStoreSupport) js).InstanceName = schedName;

                // Install custom lock handler (Semaphore)
                var lockHandlerTypeName = cfg.GetStringProperty(PropertyJobStoreLockHandlerType);
                if (lockHandlerTypeName != null)
                {
                    try
                    {
                        var lockHandlerType = loadHelper.LoadType(lockHandlerTypeName);
                        ISemaphore lockHandler;
                        var cWithDbProvider =
                            lockHandlerType.GetConstructor(new[] {typeof (DbProvider)});

                        if (cWithDbProvider != null)
                        {
                            // takes db provider
                            var dbProvider = DBConnectionManager.Instance.GetDbProvider(((JobStoreSupport) js).DataSource);
                            lockHandler = (ISemaphore) cWithDbProvider.Invoke(new object[] { dbProvider });
                        }
                        else
                        {
                            lockHandler = (ISemaphore)ObjectUtils.InstantiateType(lockHandlerType);
                        }

                        tProps = cfg.GetPropertyGroup(PropertyJobStoreLockHandlerPrefix, true);

                        // If this lock handler requires the table prefix, add it to its properties.
                        if (lockHandler is ITablePrefixAware)
                        {
                            tProps[PropertyTablePrefix] = ((JobStoreSupport) js).TablePrefix;
                        }

                        try
                        {
                            ObjectUtils.SetObjectProperties(lockHandler, tProps);
                        }
                        catch (Exception e)
                        {
                            initException = new SchedulerException(string.Format(CultureInfo.InvariantCulture, "JobStore LockHandler type '{0}' props could not be configured.", lockHandlerTypeName), e);
                            initException.ErrorCode = SchedulerException.ErrorBadConfiguration;
                            throw initException;
                        }

                        ((JobStoreSupport) js).LockHandler = lockHandler;
                        Log.Info("Using custom data access locking (synchronization): " + lockHandlerType);
                    }
                    catch (Exception e)
                    {
                        initException = new SchedulerException(string.Format(CultureInfo.InvariantCulture, "JobStore LockHandler type '{0}' could not be instantiated.", lockHandlerTypeName), e);
                        initException.ErrorCode = SchedulerException.ErrorBadConfiguration;
                        throw initException;
                    }
                }
            }

            // Set up any SchedulerPlugins
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            var pluginNames = cfg.GetPropertyGroups(PropertyPluginPrefix);
            var plugins = new ISchedulerPlugin[pluginNames.Length];
            for (var i = 0; i < pluginNames.Length; i++)
            {
                var pp =
                    cfg.GetPropertyGroup(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", PropertyPluginPrefix, pluginNames[i]), true);

                var plugInType = pp[PropertyPluginType] == null ? null : pp[PropertyPluginType];

                if (plugInType == null)
                {
                    initException =
                        new SchedulerException(
                            string.Format(CultureInfo.InvariantCulture, "SchedulerPlugin type not specified for plugin '{0}'", pluginNames[i]),
                            SchedulerException.ErrorBadConfiguration);
                    throw initException;
                }
                ISchedulerPlugin plugin;
                try
                {
                    plugin = (ISchedulerPlugin) ObjectUtils.InstantiateType(LoadType(plugInType));
                }
                catch (Exception e)
                {
                    initException =
                        new SchedulerException(
                            string.Format(CultureInfo.InvariantCulture, "SchedulerPlugin of type '{0}' could not be instantiated.", plugInType), e);
                    initException.ErrorCode = SchedulerException.ErrorBadConfiguration;
                    throw initException;
                }
                try
                {
                    ObjectUtils.SetObjectProperties(plugin, pp);
                }
                catch (Exception e)
                {
                    initException =
                        new SchedulerException(
                            string.Format(CultureInfo.InvariantCulture, "JobStore SchedulerPlugin '{0}' props could not be configured.", plugInType),
                            e);
                    initException.ErrorCode = SchedulerException.ErrorBadConfiguration;
                    throw initException;
                }
                plugins[i] = plugin;
            }

            // Set up any JobListeners
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            var strArg = new[] {typeof (string)};
            var jobListenerNames = cfg.GetPropertyGroups(PropertyJobListenerPrefix);
            var jobListeners = new IJobListener[jobListenerNames.Length];
            for (var i = 0; i < jobListenerNames.Length; i++)
            {
                var lp =
                    cfg.GetPropertyGroup(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", PropertyJobListenerPrefix, jobListenerNames[i]), true);

                var listenerType = lp[PropertyListenerType] == null ? null : lp[PropertyListenerType];

                if (listenerType == null)
                {
                    initException =
                        new SchedulerException(
                            string.Format(CultureInfo.InvariantCulture, "JobListener type not specified for listener '{0}'", jobListenerNames[i]),
                            SchedulerException.ErrorBadConfiguration);
                    throw initException;
                }
                IJobListener listener;
                try
                {
                    listener = (IJobListener) ObjectUtils.InstantiateType(loadHelper.LoadType(listenerType));
                }
                catch (Exception e)
                {
                    initException =
                        new SchedulerException(
                            string.Format(CultureInfo.InvariantCulture, "JobListener of type '{0}' could not be instantiated.", listenerType), e);
                    initException.ErrorCode = SchedulerException.ErrorBadConfiguration;
                    throw initException;
                }
                try
                {
                    var nameSetter =
                        listener.GetType().GetMethod("setName", (strArg == null) ? new Type[0] : strArg);
                    if (nameSetter != null)
                    {
                        nameSetter.Invoke(listener, new object[] {jobListenerNames[i]});
                    }
                    ObjectUtils.SetObjectProperties(listener, lp);
                }
                catch (Exception e)
                {
                    initException =
                        new SchedulerException(
                            string.Format(CultureInfo.InvariantCulture, "JobListener '{0}' props could not be configured.", listenerType), e);
                    initException.ErrorCode = SchedulerException.ErrorBadConfiguration;
                    throw initException;
                }
                jobListeners[i] = listener;
            }

            // Set up any TriggerListeners
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            var triggerListenerNames = cfg.GetPropertyGroups(PropertyTriggerListenerPrefix);
            var triggerListeners = new ITriggerListener[triggerListenerNames.Length];
            for (var i = 0; i < triggerListenerNames.Length; i++)
            {
                var lp =
                    cfg.GetPropertyGroup(
                        string.Format(CultureInfo.InvariantCulture, "{0}.{1}", PropertyTriggerListenerPrefix, triggerListenerNames[i]), true);

                var listenerType = lp[PropertyListenerType] == null ? null : lp[PropertyListenerType];

                if (listenerType == null)
                {
                    initException =
                        new SchedulerException(
                            string.Format(CultureInfo.InvariantCulture, "TriggerListener type not specified for listener '{0}'",
                                          triggerListenerNames[i]),
                            SchedulerException.ErrorBadConfiguration);
                    throw initException;
                }
                ITriggerListener listener;
                try
                {
                    listener = (ITriggerListener) ObjectUtils.InstantiateType(loadHelper.LoadType(listenerType));
                }
                catch (Exception e)
                {
                    initException =
                        new SchedulerException(
                            string.Format(CultureInfo.InvariantCulture, "TriggerListener of type '{0}' could not be instantiated.", listenerType), e);
                    initException.ErrorCode = SchedulerException.ErrorBadConfiguration;
                    throw initException;
                }
                try
                {
                    var nameSetter =
                        listener.GetType().GetMethod("setName", (strArg == null) ? new Type[0] : strArg);
                    if (nameSetter != null)
                    {
                        nameSetter.Invoke(listener, (new object[] {triggerListenerNames[i]}));
                    }
                    ObjectUtils.SetObjectProperties(listener, lp);
                }
                catch (Exception e)
                {
                    initException =
                        new SchedulerException(
                            string.Format(CultureInfo.InvariantCulture, "TriggerListener '{0}' props could not be configured.", listenerType), e);
                    initException.ErrorCode = SchedulerException.ErrorBadConfiguration;
                    throw initException;
                }
                triggerListeners[i] = listener;
            }

            // Get exporter
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            var exporterType = cfg.GetStringProperty(PropertySchedulerExporterType, null);

            if (exporterType != null)
            {
                try
                {
                    exporter = (ISchedulerExporter)ObjectUtils.InstantiateType(loadHelper.LoadType(exporterType));
                }
                catch (Exception e)
                {
                    initException =
                        new SchedulerException(
                            string.Format(CultureInfo.InvariantCulture, "Scheduler exporter of type '{0}' could not be instantiated.", exporterType), e);
                    initException.ErrorCode = SchedulerException.ErrorBadConfiguration;
                    throw initException;
                }

                tProps = cfg.GetPropertyGroup(PropertySchedulerExporterPrefix, true);

                try
                {
                    ObjectUtils.SetObjectProperties(exporter, tProps);
                }
                catch (Exception e)
                {
                    initException =
                        new SchedulerException(
                            string.Format(CultureInfo.InvariantCulture, "Scheduler exporter type '{0}' props could not be configured.", exporterType), e);
                    initException.ErrorCode = SchedulerException.ErrorBadConfiguration;
                    throw initException;
                }
            }

            // Fire everything up
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            IJobRunShellFactory jrsf = new StdJobRunShellFactory();
            
            if (autoId)
            {
                try
                {
                    schedInstId = DefaultInstanceId;
                    
                    if (js is JobStoreSupport)
					{
						if (((JobStoreSupport) js).Clustered)
						{
							schedInstId = instanceIdGenerator.GenerateInstanceId();
						}
					}
                   
                }
                catch (Exception e)
                {
                    Log.Error("Couldn't generate instance Id!", e);
                    throw new SystemException("Cannot run without an instance id.");
                }
            }

            
			if (js is JobStoreSupport)
			{
				var jjs = (JobStoreSupport) js;
				jjs.InstanceId = schedInstId;
                jjs.DbRetryInterval = dbFailureRetry;
			}

            var rsrcs = new QuartzSchedulerResources();
            rsrcs.Name = schedName;
            rsrcs.ThreadName = threadName;
            rsrcs.InstanceId = schedInstId;
            rsrcs.JobRunShellFactory = jrsf;
            rsrcs.MakeSchedulerThreadDaemon = makeSchedulerThreadDaemon;
            rsrcs.ThreadPool = tp;
            rsrcs.SchedulerExporter = exporter;

            if (tp is SimpleThreadPool)
            {
                ((SimpleThreadPool) tp).ThreadNamePrefix = schedName + "_Worker";
            }
            tp.Initialize();

            rsrcs.JobStore = js;

            // add plugins
            for (var i = 0; i < plugins.Length; i++)
            {
                rsrcs.AddSchedulerPlugin(plugins[i]);
            }

            schedCtxt = new SchedulingContext();
            schedCtxt.InstanceId = rsrcs.InstanceId;

            qs = new QuartzScheduler(rsrcs, schedCtxt, idleWaitTime, dbFailureRetry);

            // Create Scheduler ref...
            var sched = Instantiate(rsrcs, qs);

            // set job factory if specified
            if (jobFactory != null)
            {
                qs.JobFactory = jobFactory;
            }

            // Initialize plugins now that we have a Scheduler instance.
            for (var i = 0; i < plugins.Length; i++)
            {
                plugins[i].Initialize(pluginNames[i], sched);
            }

            // add listeners
            for (var i = 0; i < jobListeners.Length; i++)
            {
                qs.AddGlobalJobListener(jobListeners[i]);
            }
            for (var i = 0; i < triggerListeners.Length; i++)
            {
                qs.AddGlobalTriggerListener(triggerListeners[i]);
            }

            // set scheduler context data...
            var itr = new HashSet(schedCtxtProps).GetEnumerator();
            while (itr.MoveNext())
            {
                var key = (String) itr.Current;
                var val = schedCtxtProps.Get(key);

                sched.Context.Put(key, val);
            }

            // fire up job store, and runshell factory

            js.Initialize(loadHelper, qs.SchedulerSignaler);

            jrsf.Initialize(sched, schedCtxt);

            Log.Info(string.Format(CultureInfo.InvariantCulture, "Quartz scheduler '{0}' initialized", sched.SchedulerName));

            Log.Info(string.Format(CultureInfo.InvariantCulture, "Quartz scheduler version: {0}", qs.Version));

            // prevents the repository from being garbage collected
            qs.AddNoGCObject(schedRep);
            // prevents the db manager from being garbage collected
            if (dbMgr != null)
            {
                qs.AddNoGCObject(dbMgr);
            }

            schedRep.Bind(sched);

            return sched;
        }

        protected internal virtual IScheduler Instantiate(QuartzSchedulerResources rsrcs, QuartzScheduler qs)
        {
            var schedCtxt = new SchedulingContext();
            schedCtxt.InstanceId = rsrcs.InstanceId;

            IScheduler sched = new StdScheduler(qs, schedCtxt);
            return sched;
        }


        protected virtual Type LoadType(string typeName)
        {
            return Type.GetType(typeName);
        }

        /// <summary>
        /// Returns a handle to the Scheduler produced by this factory.
        /// </summary>
        /// 
        /// <remarks>
        /// If one of the <see cref="Initialize()" /> methods has not be previously
        /// called, then the default (no-arg) <see cref="Initialize()" /> method
        /// will be called by this method.
        /// </remarks>
        public virtual IScheduler GetScheduler()
        {
            if (cfg == null)
            {
                Initialize();
            }

            var schedRep = SchedulerRepository.Instance;

            var sched = schedRep.Lookup(SchedulerName);

            if (sched != null)
            {
                if (sched.IsShutdown)
                {
                    schedRep.Remove(SchedulerName);
                }
                else
                {
                    return sched;
                }
            }

            sched = Instantiate();

            return sched;
        }

        /// <summary> <p>
        /// Returns a handle to the Scheduler with the given name, if it exists (if
        /// it has already been instantiated).
        /// </p>
        /// </summary>
        public virtual IScheduler GetScheduler(string schedName)
        {
            return SchedulerRepository.Instance.Lookup(schedName);
        }
    }
}