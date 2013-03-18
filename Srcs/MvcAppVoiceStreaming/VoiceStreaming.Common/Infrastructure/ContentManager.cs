using System;
using System.Collections.Concurrent;

namespace VoiceStreaming.Common.Infrastructure
{
	public sealed class ContentManager : IContentManager
	{
		private ConcurrentDictionary<Guid, ContentStatus> _holder;

		public ContentManager()
		{
			_holder = new ConcurrentDictionary<Guid, ContentStatus>();
			System.Diagnostics.Debug.WriteLine("ContentManager is created.");
		}

		public void Add(Guid id, ContentStatus status)
		{
			if (_holder.ContainsKey(id))
				throw new InvalidOperationException("Connection is already exists.");

			if (!_holder.TryAdd(id, status))
				throw new InvalidOperationException("Fail to add.");
		}

		public void Change(Guid id, ContentStatus status)
		{
			ContentStatus prevStatus;
			if (!_holder.TryGetValue(id, out prevStatus))
				throw new InvalidOperationException("Connection wasn't found.");

			if (!_holder.TryUpdate(id, status, prevStatus))
				throw new InvalidOperationException("Fail to update.");
		}

		public void Remove(Guid id)
		{
			ContentStatus status;
			if (!_holder.TryRemove(id, out status))
				throw new InvalidOperationException("Fail to remove.");
		}

		public ContentStatus GetStatus(Guid id)
		{
			ContentStatus status = ContentStatus.None;
			_holder.TryGetValue(id, out status);
			return status;
		}
	}
}