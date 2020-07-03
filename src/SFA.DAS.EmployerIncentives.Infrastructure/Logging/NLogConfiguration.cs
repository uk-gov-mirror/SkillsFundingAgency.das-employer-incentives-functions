﻿using NLog;
using NLog.Common;
using NLog.Config;
using NLog.Targets;
using SFA.DAS.NLog.Targets.Redis.DotNetCore;
using System;
using System.IO;

namespace SFA.DAS.EmployerIncentives.Infrastructure.Logging
{
    public class NLogConfiguration
    {
        public void ConfigureNLog()
        {
            var appName = Environment.GetEnvironmentVariable("AppName");
            var env = Environment.GetEnvironmentVariable("EnvironmentName");
            var config = new LoggingConfiguration();

            if (string.IsNullOrEmpty(env) || env.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
            {
                AddLocalTarget(config, appName);
            }
            else
            {
                AddRedisTarget(config, appName);
            }

            LogManager.Configuration = config;
        }

        private static void AddLocalTarget(LoggingConfiguration config, string appName)
        {
            InternalLogger.LogFile = Path.Combine(Directory.GetCurrentDirectory(), $"{appName}\\nlog-internal.{appName}.log");
            var fileTarget = new FileTarget("Disk")
            {
                FileName = Path.Combine(Directory.GetCurrentDirectory(), $"{appName}\\{appName}.${{shortdate}}.log"),
                Layout = "${longdate} [${uppercase:${level}}] [${logger}] - ${message} ${onexception:${exception:format=tostring}}"
            };
            config.AddTarget(fileTarget);

            config.AddRule(GetMinLogLevel(), LogLevel.Fatal, "Disk");
        }

        private static void AddRedisTarget(LoggingConfiguration config, string appName)
        {
            var target = new RedisTarget
            {
                Name = "RedisLog",
                AppName = appName,
                EnvironmentKeyName = "EnvironmentName",
                ConnectionStringName = "LoggingRedisConnectionString",
                IncludeAllProperties = true,
                Layout = "${message}"
            };

            config.AddTarget(target);
            config.AddRule(GetMinLogLevel(), LogLevel.Fatal, "RedisLog");
        }

        private static LogLevel GetMinLogLevel() => LogLevel.FromString("Info");
    }
}