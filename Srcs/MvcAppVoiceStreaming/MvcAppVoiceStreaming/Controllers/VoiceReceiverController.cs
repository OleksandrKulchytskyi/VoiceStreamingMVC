using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VoiceStreaming.Common;

namespace MvcAppVoiceStreaming.Controllers
{
	public class VoiceReceiverController : ApiController, IVoiceReceiver
	{
		[HttpPost]
		public void Receive()
		{
			//this.Request.Content.ReadAsByteArrayAsync().Result;
		}

		[HttpGet]
		public void Start()
		{
			throw new NotImplementedException();
		}
		[HttpGet]
		public void Stop()
		{
			throw new NotImplementedException();
		}
	}
}
