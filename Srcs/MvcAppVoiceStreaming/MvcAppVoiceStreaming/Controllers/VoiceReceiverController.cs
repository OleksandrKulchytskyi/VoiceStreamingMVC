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
		private IContentManager _manager;

		public VoiceReceiverController(IContentManager manager)
		{
			_manager = manager;
		}

		[ActionName("Get")]
		[HttpGet()]
		public HttpResponseMessage Get()
		{
			Guid id;
			if(!Guid.TryParse(Request.Content.ReadAsStringAsync().Result,out id))
				throw new HttpResponseException(new HttpResponseMessage(  HttpStatusCode.NotFound));
			return new HttpResponseMessage() { Content = new StringContent(_manager.GetStatus(id).ToString()) }; 
		}

		[HttpPost]
		public void Receive()
		{
			//this.Request.Content.ReadAsByteArrayAsync().Result;
		}


		[HttpPost]
		public HttpResponseMessage Start()
		{
			string msg = this.Request.Content.ReadAsStringAsync().Result;
			if (msg.Equals("start", StringComparison.OrdinalIgnoreCase))
			{
				Guid id = Guid.NewGuid();
				_manager.Add(id, ContentStatus.Started);
				var retMsg = new HttpResponseMessage(HttpStatusCode.OK);
				retMsg.Content = new StringContent(id.ToString());
				return retMsg;
			}
			return new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
		}

		[ActionName("Stop")]
		[HttpGet]
		public void Stop()
		{
			string msg = this.Request.Content.ReadAsStringAsync().Result;
			Guid id;
			if (!Guid.TryParse(msg, out id))
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest));

			_manager.Change(id, ContentStatus.Stopped);
		}

		protected override void Dispose(bool disposing)
		{
			if (_manager != null)
				_manager = null;
			base.Dispose(disposing);
		}
	}
}
