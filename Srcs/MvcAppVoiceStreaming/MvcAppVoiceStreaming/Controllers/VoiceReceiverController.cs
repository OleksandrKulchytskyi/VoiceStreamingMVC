using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VoiceStreaming.Common;

namespace MvcAppVoiceStreaming.Controllers
{
	public class VoiceReceiverController : ApiController, IVoiceReceiver
	{
		private IContentManager _manager = null;
		private IContentMapper _mapper = null;

		public VoiceReceiverController(IContentManager manager, IContentMapper mapper)
		{
			_manager = manager;
			_mapper = mapper;
		}

		[HttpPost]
		public HttpResponseMessage Start([FromUri] string flag)
		{
			if (flag.Equals("start", StringComparison.OrdinalIgnoreCase))
			{
				Guid id = Guid.NewGuid();
				string fullPath = string.Empty;
				try
				{
					_manager.Add(id, ContentStatus.Started);
					string root = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Audio");

					//string filename = string.Format("{0}_{1}.wav",id.ToString("N"), DateTime.Now.ToString("ddMMyyy_hh:mm:ss"));
					string filename = string.Format("{0}.wav", id.ToString("N"));
					fullPath = Path.Combine(root, filename);
					_mapper.CreateMapping(id, fullPath);
					System.Diagnostics.Debug.WriteLine(string.Format("Mapping was successfully created for: {0} {1} {2}",
																	id, Environment.NewLine, fullPath));
					if (File.Exists(fullPath))
						File.Delete(fullPath);

					using (FileStream fs = File.Create(fullPath))
					{
						System.Diagnostics.Debug.WriteLine(string.Format("File has been created: {0}", fullPath));
					}
				}
				catch (IOException ex)
				{
					System.Diagnostics.Debug.WriteLine(string.Format("Fail to create file: {0}{1}{2}", fullPath, Environment.NewLine, ex.Message));
				}

				var retMsg = new HttpResponseMessage(HttpStatusCode.OK);

				//send back to the client record id;
				retMsg.Content = new StringContent(id.ToString());
				return retMsg;
			}
			return new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
		}

		[NonAction]
		private string GetRecordId()
		{
			var value = Request.Headers.FirstOrDefault(x => x.Key.Equals("recordId", StringComparison.OrdinalIgnoreCase));
			if (value.Value != null)
			{
				string id = value.Value.First();
				return id;
			}
			return string.Empty;
		}

		[ActionName("Stop")]
		[HttpGet]
		public void Stop()
		{
			string recId = GetRecordId();
			Guid id;
			if (!Guid.TryParse(recId, out id))
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest));
			try
			{
				_manager.Change(id, ContentStatus.Stopped);
				_manager.Remove(id);
				_mapper.Remove(id);
			}
			catch (Exception ex)
			{
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.ExpectationFailed));
			}
		}

		[HttpPost]
		public void Receive()
		{
			string ID = GetRecordId();
			if (string.IsNullOrEmpty(ID))
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest));
			Guid id;
			if (!Guid.TryParse(ID, out id))
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.ExpectationFailed));

			try
			{
				System.Diagnostics.Debug.WriteLine("Write for Id: " + id.ToString());
				var task = Request.Content.ReadAsStreamAsync();
				task.Wait();
				Stream audioStream = task.Result;

				if (audioStream != null && audioStream.CanRead && _mapper.Exist(id))
				{
					using (FileStream fs = File.Open(_mapper.GetFor(id), FileMode.Append))
					{
						if (fs.CanSeek)
							fs.Seek(0, SeekOrigin.End);
						audioStream.CopyTo(fs);
						fs.Flush();
					}
					audioStream.Close();
				}
			}
			catch (IOException ioEx) { }
			catch (Exception ex) { }
		}

		[ActionName("Get")]
		[HttpGet()]
		public HttpResponseMessage Get()
		{
			Guid id;
			if (!Guid.TryParse(Request.Content.ReadAsStringAsync().Result, out id))
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
			return new HttpResponseMessage() { Content = new StringContent(_manager.GetStatus(id).ToString()) };
		}

		protected override void Dispose(bool disposing)
		{
			if (_manager != null) _manager = null;
			if (_mapper != null) _mapper = null;

			base.Dispose(disposing);
		}
	}
}