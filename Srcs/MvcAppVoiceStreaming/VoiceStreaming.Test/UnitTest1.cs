using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VoiceStreaming.Test
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			Guid id = Guid.NewGuid();

			string fullPath = string.Format("~/App_Data/Audio/{0}_{1}.wav",
														id.ToString("N"), DateTime.Now.ToString("ddMMyyy_hh:mm:ss"));
			Assert.IsNotNull(fullPath);
		}
	}
}
