using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceStreaming.Common
{
	public enum LogSeverity
	{
		Info = 0,
		Warn,
		Error,
		Fatal
	}

	public interface ILog
	{
		void AddMessage(LogSeverity severity, string payload, string msg);
		void AddMessage(LogSeverity severity, string msg);
		void AddMessage(LogSeverity severity, string payload, Exception ex);
	}
}
