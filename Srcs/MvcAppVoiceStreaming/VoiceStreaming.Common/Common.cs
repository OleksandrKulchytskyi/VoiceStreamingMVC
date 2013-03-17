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
		HttpResponseMessage Start(string flag);

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

	public interface IContentMapper
	{
		void CreateMapping(Guid id, string path);
		string GetFor(Guid id);
		bool Exist(Guid id);
		void Remove(Guid id);
	}
}