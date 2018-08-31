﻿using DbKeeperNet.Engine.Configuration;
using DbKeeperNet.Engine.Tests.Checkers;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DbKeeperNet.Extensions.Mysql.Tests.Checkers
{
    [TestFixture]
    public class MySqlDatabaseServiceViewCheckerTest : ViewCheckerTestBase
    {
        protected override void Configure(IDbKeeperNetBuilder configurationBuilder)
        {
            configurationBuilder
                .UseMysql(ConnectionStrings.TestDatabase)
                ;

            configurationBuilder.Services.AddLogging();
        }

        protected override void CreateView(string viewName)
        {
            ExecuteSqlAndIgnoreException(@"create view {0} as select 1 as version", viewName);
        }

        protected override void DropView(string viewName)
        {
            ExecuteSqlAndIgnoreException(@"drop view {0}", viewName);
        }

    }
}