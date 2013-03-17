using System;
using System.Net.Http;

namespace VoiceStreaming.Common
{
	public interface IRunnable
	{
		void Run();
	}

	public interface IVoiceReceiver
	{
		HttpResponseMessage Start();

		void Receive();

		void Stop();
	}

	public enum ContentStatus
	{
		None = 0,
		Started,
		Receiving,
		Stopped
	}

	public interface IContentManager
	{
		void Add(Guid id, ContentStatus status);

		void Change(Guid id, ContentStatus status);

		void Remove(Guid id);

		ContentStatus GetStatus(Guid id);
	}
}