using System;
using System.Collections.Generic;
using System.Text;

namespace DbKeeperNet.Engine
{
    /// <summary>
    /// Extension bootstrap and marking interface.
    /// 
    /// When an extension assembly is loaded, this interfaces
    /// allows registration of all provided services.
    /// 
    /// For examples look on built-in services.
    /// </summary>
    public interface IExtension
    {
        /// <summary>
        /// Method invoked for extension initialization. Register
        /// all provided services and new preconditions here.
        /// </summary>
        /// <param name="context">Current update context</param>
        /// <see cref="IUpdateContext.RegisterPrecondition"/>
        /// <see cref="IUpdateContext.RegisterDatabaseService"/>
        /// <see cref="IUpdateContext.RegisterLoggingService"/>
        void Initialize(IUpdateContext context);
    }
}
