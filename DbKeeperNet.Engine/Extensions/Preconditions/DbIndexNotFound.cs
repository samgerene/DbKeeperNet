using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace DbKeeperNet.Engine.Extensions.Preconditions
{
    /// <summary>
    /// Condition verifies that index with given name doesn't exist.
    /// Condition reference name is <value>DbIndexNotFound</value>.
    /// It has one parameter which should contain tested database
    /// index or primary key name.
    /// <code>
    /// <![CDATA[
    /// <Precondition FriendlyName="Index UQ_test not found" Precondition="DbIndexNotFound">
    ///   <Param>UQ_test</Param>
    ///   <Param>table_test_index</Param>
    /// </Precondition>
    /// ]]>
    /// </code>
    /// </summary>
    public sealed class DbIndexNotFound: IPrecondition
    {
        #region IPrecondition Members

        public string Name
        {
            get { return @"DbIndexNotFound"; }
        }

        public bool CheckPrecondition(IUpdateContext context, PreconditionParamType[] param)
        {
            if (context == null)
                throw new ArgumentNullException(@"context");
            if ((param == null) || (param.Length < 2) || (String.IsNullOrEmpty(param[0].Value)) || (String.IsNullOrEmpty(param[1].Value)))
                throw new ArgumentNullException(@"param", String.Format("Index name and table for condition {0} must be specified", Name));

            bool result = !context.DatabaseService.IndexExists(param[0].Value, param[1].Value);

            return result;
        }

        #endregion
    }
}
