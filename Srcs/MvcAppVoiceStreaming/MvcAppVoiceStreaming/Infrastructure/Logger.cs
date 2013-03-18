using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VoiceStreaming.Common;

namespace MvcAppVoiceStreaming.Infrastructure
{
	public sealed class Logger : VoiceStreaming.Common.ILog
	{
		private log4net.ILog _logger = null;

		public Logger()
		{
			_logger = LogManager.GetLogger(typeof(Logger).FullName);
			log4net.Config.XmlConfigurator.Configure();
			_logger.Info("Loger modle is initilaized.");
		}


		public void AddMessage(LogSeverity severity, string payload, string msg)
		{
			switch (severity)
			{
				case LogSeverity.Fatal:
					if (string.IsNullOrEmpty(payload))
						_logger.Fatal(msg);
					else
						_logger.FatalFormat("{0}{1}{2}", payload, Environment.NewLine, msg);
					break;

				case LogSeverity.Error:
					if (string.IsNullOrEmpty(payload))
						_logger.Error(msg);
					else
						_logger.ErrorFormat("{0}{1}{2}", payload, Environment.NewLine, msg);
					break;

				case LogSeverity.Info:
					if (string.IsNullOrEmpty(payload))
						_logger.Info(msg);
					else
						_logger.InfoFormat("{0}{1}{2}", payload, Environment.NewLine, msg);
					break;

				case LogSeverity.Warn:
					if (string.IsNullOrEmpty(payload))
						_logger.Warn(msg);
					else
						_logger.WarnFormat("{0}{1}{2}", payload, Environment.NewLine, msg);
					break;
				default:
					break;
			}
		}

		public void AddMessage(LogSeverity severity, string msg)
		{
			AddMessage(severity, string.Empty, msg);
		}


		public void AddMessage(LogSeverity severity, string payload, Exception ex)
		{
			switch (severity)
			{
				case LogSeverity.Fatal:
					_logger.Fatal(payload, ex);
					break;

				case LogSeverity.Error:
					_logger.Error(payload, ex);
					break;

				case LogSeverity.Info:
					_logger.Info(payload, ex);
					break;

				case LogSeverity.Warn:
					_logger.Warn(payload, ex);
					break;
				default:
					break;
			}
		}
	}
}