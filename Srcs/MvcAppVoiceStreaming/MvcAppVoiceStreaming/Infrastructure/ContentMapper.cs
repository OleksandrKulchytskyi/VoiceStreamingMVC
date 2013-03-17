using System;
using System.Collections.Concurrent;
using VoiceStreaming.Common;

namespace MvcAppVoiceStreaming.Infrastructure
{
	public class ContentMapper : IContentMapper
	{
		private ConcurrentDictionary<Guid, string> _mappings;

		public ContentMapper()
		{
			_mappings = new ConcurrentDictionary<Guid, string>();
			System.Diagnostics.Debug.WriteLine("ContentMapper is created.");
		}

		public void CreateMapping(Guid id, string path)
		{
			if (_mappings.ContainsKey(id))
				throw new InvalidOperationException("Maping is already exists.");
			if (!_mappings.TryAdd(id, path))
				throw new InvalidOperationException("Add operation was fail.");
		}

		public string GetFor(Guid id)
		{
			string result = string.Empty;
			_mappings.TryGetValue(id, out result);
			return result;
		}

		public bool Exist(Guid id)
		{
			return _mappings.ContainsKey(id);
		}

		public void Remove(Guid id)
		{
			string val;
			_mappings.TryRemove(id, out val);
		}
	}
}