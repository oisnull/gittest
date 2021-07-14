using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using System;
using System.IO;

namespace HWL_IM_Core
{
    public class LogHelper
    {
        private static ILoggerRepository repository { get; set; }
        private static string defaultRepository = "default";

        //public static string GetLogDirectory()
        //{
        //    var rootAppender = ((Hierarchy)LogManager.GetRepository(defaultRepository))
        //                                    .Root.Appenders.OfType<FileAppender>()
        //                                    .FirstOrDefault();

        //    return Path.GetDirectoryName(rootAppender.File);
        //}

        private static ILog GetLogger(Type sourceType)
        {
            ILog logger;
            if (null == sourceType)
            {
                logger = LogManager.GetLogger(defaultRepository, "");
            }
            else
            {
                logger = LogManager.GetLogger(defaultRepository, sourceType);
            }

            return logger;
        }

        public static void InitConfigure(string logDir = null, string repositoryName = null)
        {
            if (!string.IsNullOrEmpty(repositoryName?.Trim()))
            {
                defaultRepository = repositoryName?.Trim();
            }

            if (string.IsNullOrEmpty(logDir))
            {
                //logDir = Path.GetPathRoot(AppContext.BaseDirectory);
                logDir = AppContext.BaseDirectory;
            }

            if (!Path.IsPathRooted(logDir))
            {
                logDir = Path.Combine(Path.GetPathRoot(AppContext.BaseDirectory), logDir);
            }

            GlobalContext.Properties["LogFileName"] = Path.Combine(logDir, $"{defaultRepository}-");

            string configFilePath = Path.Combine(AppContext.BaseDirectory, "log4net.config");
            repository = LogManager.CreateRepository(defaultRepository);
            XmlConfigurator.Configure(repository, new Uri(configFilePath));

            //var appenders = repository.GetAppenders();
            //var targetApder = appenders.FirstOrDefault(p => p.Name == "RollingFileAppender") as FileAppender;
        }

        public static void Debug(string message, Type sourceType = null)
        {
            //if (!LogConfigManager.EnableDebug) return;

            var logger = GetLogger(sourceType);

            if (logger.IsDebugEnabled)
            {
                logger.Debug(message);
            }
        }

        public static void Debug(string message, Exception ex, Type sourceType = null)
        {
            //if (!LogConfigManager.EnableDebug) return;

            var logger = GetLogger(sourceType);

            if (logger.IsDebugEnabled)
            {
                logger.Debug(message, ex);
            }
        }

        public static void Info(string message, Type sourceType = null)
        {
            //if (!LogConfigManager.EnableInfo) return;

            var logger = GetLogger(sourceType);

            if (logger.IsInfoEnabled)
            {
                logger.Info(message);
            }
        }

        public static void Info(string message, Exception ex, Type sourceType = null)
        {
            //if (!LogConfigManager.EnableInfo) return;

            var logger = GetLogger(sourceType);

            if (logger.IsInfoEnabled)
            {
                logger.Info(message, ex);
            }
        }

        public static void Warn(string message, Type sourceType = null)
        {
            //if (!LogConfigManager.EnableWarn) return;

            var logger = GetLogger(sourceType);

            if (logger.IsWarnEnabled)
            {
                logger.Warn(message);
            }
        }

        public static void Warn(string message, Exception ex, Type sourceType = null)
        {
            //if (!LogConfigManager.EnableWarn) return;

            var logger = GetLogger(sourceType);

            if (logger.IsWarnEnabled)
            {
                logger.Warn(message, ex);
            }
        }

        public static void Error(string message, Type sourceType = null)
        {
            //if (!LogConfigManager.EnableError) return;

            var logger = GetLogger(sourceType);

            if (logger.IsErrorEnabled)
            {
                logger.Error(message);
            }
        }

        public static void Error(string message, Exception ex, Type sourceType = null)
        {
            //if (!LogConfigManager.EnableError) return;

            var logger = GetLogger(sourceType);

            if (logger.IsErrorEnabled)
            {
                logger.Error(message, ex);
            }
        }

        public static void Fatal(string message, Type sourceType = null)
        {
            //if (!LogConfigManager.EnableFatal) return;

            var logger = GetLogger(sourceType);

            if (logger.IsFatalEnabled)
            {
                logger.Fatal(message);
            }
        }

        public static void Fatal(string message, Exception ex, Type sourceType = null)
        {
            //if (!LogConfigManager.EnableFatal) return;

            var logger = GetLogger(sourceType);

            if (logger.IsFatalEnabled)
            {
                logger.Fatal(message, ex);
            }
        }
    }
}
