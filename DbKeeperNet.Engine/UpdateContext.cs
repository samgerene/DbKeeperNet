using System;
using System.Collections.Generic;
using DbKeeperNet.Engine.Resources;
using System.Globalization;
using System.IO;

namespace DbKeeperNet.Engine
{
    /// <summary>
    /// Default IUpdateContext interface implementation.
    /// </summary>
    public class UpdateContext : IUpdateContext
    {
        IDatabaseService _databaseService;
        ILoggingService _loggingService;
        DbKeeperNetConfigurationSection _configurationSection;

        Dictionary<string, ILoggingService> _loggingServices = new Dictionary<string, ILoggingService>();
        Dictionary<string, IPrecondition> _preconditions = new Dictionary<string, IPrecondition>();
        Dictionary<string, IDatabaseService> _databaseServices = new Dictionary<string, IDatabaseService>();
        Dictionary<string, IScriptProviderService> _scriptProviderServices = new Dictionary<string, IScriptProviderService>();

        string _friendlyName;
        string _currentAssemblyName;
        string _currentVersion;
        int _currentStep;
        PreconditionType[] _defaultPreconditions = { };

        public UpdateContext()
            : this(DbKeeperNetConfigurationSection.Current)
        {
        }

        public UpdateContext(DbKeeperNetConfigurationSection section)
        {
            if (section == null)
                throw new ArgumentNullException("section");

             _configurationSection = section;
        }

        #region IUpdateContext Members
        public ILoggingService Logger
        {
            get
            {
                if (_loggingService == null)
                    InitializeLoggingService(_configurationSection.LoggingService);

                if (_loggingService == null)
                    throw new InvalidOperationException(UpdateContextMessages.CanNotInitializeLoggingService);

                return _loggingService;
            }
        }
        public IDatabaseService DatabaseService
        {
            get
            {
                if (_databaseService == null)
                    throw new InvalidOperationException(UpdateContextMessages.DatabaseServiceNotInitialized);

                return _databaseService;
            }
        }

        public void RegisterPrecondition(IPrecondition precondition)
        {
            if (precondition == null)
                throw new ArgumentNullException(@"precondition");
            if (String.IsNullOrEmpty(precondition.Name))
                throw new InvalidOperationException(UpdateContextMessages.PreconditionNameNull);

            if (_preconditions.ContainsKey(precondition.Name))
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, UpdateContextMessages.PreconditionAlreadyRegistered, precondition.Name));

            _preconditions[precondition.Name] = precondition;
        }

        public bool CheckPrecondition(string name, PreconditionParamType[] parameters)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            IPrecondition precondition = _preconditions[name];

            return precondition.CheckPrecondition(this, parameters);
        }

        public void RegisterDatabaseService(IDatabaseService service)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            if (String.IsNullOrEmpty(service.Name))
                throw new InvalidOperationException(UpdateContextMessages.DatabaseServiceNameNull);

            if (_databaseServices.ContainsKey(service.Name))
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, UpdateContextMessages.DatabaseServiceAlreadyRegistered, service.Name));

            _databaseServices[service.Name] = service;
        }

        public void RegisterLoggingService(ILoggingService service)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            if (String.IsNullOrEmpty(service.Name))
                throw new InvalidOperationException(UpdateContextMessages.LoggingServiceNameNull);

            if (_loggingServices.ContainsKey(service.Name))
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, UpdateContextMessages.LoggingServiceAlreadyRegistered, service.Name));

            _loggingServices[service.Name] = service;
        }

        public void RegisterScriptProviderService(IScriptProviderService provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (String.IsNullOrEmpty(provider.Name))
                throw new InvalidCastException(UpdateContextMessages.ScriptExecutionServiceNameNull);

            if (_loggingServices.ContainsKey(provider.Name))
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, UpdateContextMessages.LoggingServiceAlreadyRegistered, provider.Name));

            _scriptProviderServices[provider.Name] = provider;
        }

        public string CurrentVersion
        {
            get
            {
                return _currentVersion;
            }
            set
            {
                _currentVersion = value;
            }
        }

        public int CurrentStep
        {
            get
            {
                return _currentStep;
            }
            set
            {
                _currentStep = value;
            }
        }

        public string CurrentAssemblyName
        {
            get
            {
                return _currentAssemblyName;
            }
            set
            {
                _currentAssemblyName = value;
            }
        }

        public string FriendlyName
        {
            get
            {
                return _friendlyName;
            }
            set
            {
                _friendlyName = value;
            }
        }
        public PreconditionType[] DefaultPreconditions
        {
            get { return _defaultPreconditions; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _defaultPreconditions = value;
            }
        }

        public void InitializeLoggingService(string serviceName)
        {
            if (String.IsNullOrEmpty(serviceName))
                throw new ArgumentNullException("serviceName");

            if (!_loggingServices.ContainsKey(serviceName))
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, UpdateContextMessages.LoggingServiceNotRegistered, serviceName));

            _loggingService = _loggingServices[serviceName];
        }

        public void InitializeDatabaseService(string connectionString)
        {
            if (String.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");

            string databaseServiceName = null;

            Logger.TraceInformation(UpdateContextMessages.SearchingDatabaseService, connectionString);

            foreach (DatabaseServiceMappingConfigurationElement e in ConfigurationSection.DatabaseServiceMappings)
            {
                if (e.ConnectString == connectionString)
                {
                    databaseServiceName = e.DatabaseService;
                    break;
                }
            }

            if (databaseServiceName == null)
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, UpdateContextMessages.DatabaseServiceMappingNotFound, connectionString));

            if (!_databaseServices.ContainsKey(databaseServiceName))
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, UpdateContextMessages.DatabaseServiceNotRegistered, databaseServiceName));

            Logger.TraceInformation(String.Format(CultureInfo.CurrentCulture, UpdateContextMessages.DatabaseServiceMappingFound, connectionString, databaseServiceName));

            _databaseService = _databaseServices[databaseServiceName].CloneForConnectionString(connectionString);

            Logger.TraceInformation(UpdateContextMessages.DatabaseServiceInitialized, connectionString);
        }

        public void LoadExtensions()
        {
            ExtensionLoader.LoadExtensions(this);
        }

        public DbKeeperNetConfigurationSection ConfigurationSection
        {
            get
            {
                return _configurationSection;
            }
        }

        public Stream GetScriptFromStreamLocation(string provider, string location)
        {
            if (String.IsNullOrEmpty(provider))
                throw new ArgumentNullException("provider");
            if (String.IsNullOrEmpty(location))
                throw new ArgumentNullException("location");

            IScriptProviderService service = _scriptProviderServices[provider];

            return service.GetScriptStreamFromLocation(location);
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_databaseService != null)
                {
                    _databaseService.Dispose();
                    _databaseService = null;
                }
            }
        }
        #endregion
    }
}
