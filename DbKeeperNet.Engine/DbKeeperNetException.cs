using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace DbKeeperNet.Engine
{
    /// <summary>
    /// All exceptions thrown within library during update process should be
    /// exposed only thru this class.
    /// Optional details may be available as an inner exception.
    /// </summary>
    [Serializable]
    public class DbKeeperNetException: Exception
    {
        public DbKeeperNetException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public DbKeeperNetException(string message)
            : base(message)
        {
        }

        public DbKeeperNetException()
        {
        }

        protected DbKeeperNetException(SerializationInfo info, StreamingContext context)
            :base(info, context)
        {
        }
    }
}
