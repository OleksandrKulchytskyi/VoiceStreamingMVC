using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using VoiceStreaming.Common;

namespace MvcAppVoiceStreaming.Controllers
{
	public class VoiceReceiverController : ApiController, IVoiceReceiver
	{
		private readonly IContentManager _manager = null;
		private readonly IContentMapper _mapper = null;
		private readonly ILog _logger = null;

		public VoiceReceiverController(IContentManager manager, IContentMapper mapper, ILog logger)
		{
			_manager = manager;
			_mapper = mapper;
			_logger = logger;
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
					string filename = string.Format("{0}.wav", id.ToString("N"));
					fullPath = Path.Combine(root, filename);

					_mapper.CreateMapping(id, fullPath);
					_logger.AddMessage(LogSeverity.Info, string.Format("Created mapping for {0}, {1}", id, fullPath));
					System.Diagnostics.Debug.WriteLine(string.Format("Mapping was successfully created for: {0} {1} {2}",
																	id, Environment.NewLine, fullPath));
					if (File.Exists(fullPath))
					{
						_logger.AddMessage(LogSeverity.Info, string.Format("File is exists {0}", fullPath));
						File.Delete(fullPath);
					}

					using (FileStream fs = File.Create(fullPath))
					{
						System.Diagnostics.Debug.WriteLine(string.Format("File has been created: {0}", fullPath));
						_logger.AddMessage(LogSeverity.Info, string.Format("File was successfully created, {0}", fullPath));
					}
				}
				catch (IOException ex)
				{
					System.Diagnostics.Debug.WriteLine(string.Format("Fail to create file: {0}{1}{2}", fullPath, Environment.NewLine, ex.Message));
					_logger.AddMessage(LogSeverity.Error, "IOException", ex);
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
			_logger.AddMessage(LogSeverity.Info, "Stop", recId);
			Guid id;
			if (!Guid.TryParse(recId, out id))
			{
				_logger.AddMessage(LogSeverity.Error, "GUID parsing was fail.");
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest));
			}

			try
			{
				_manager.Change(id, ContentStatus.Stopped);
				_manager.Remove(id);
				_mapper.Remove(id);
			}
			catch (Exception ex)
			{
				_logger.AddMessage(LogSeverity.Error, ex.Message);
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.ExpectationFailed));
			}
		}

		[HttpPost]
		public void Receive()
		{
			string ID = GetRecordId();
			_logger.AddMessage(LogSeverity.Info, ID);
			if (string.IsNullOrEmpty(ID))
			{
				_logger.AddMessage(LogSeverity.Info,"RecordID value was missing in headers.");
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest));
			}
			Guid id;
			if (!Guid.TryParse(ID, out id))
			{
				_logger.AddMessage(LogSeverity.Error, "GUID parsing was fail.");
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.ExpectationFailed));
			}

			try
			{
				System.Diagnostics.Debug.WriteLine("Write for Id: " + id.ToString());
				var task = Request.Content.ReadAsStreamAsync();
				task.Wait();
				Stream audioStream = task.Result;

				if (audioStream != null && audioStream.CanRead && _mapper.Exist(id))
				{
					using (FileStream fs = File.Open(_mapper.GetFor(id), FileMode.Append, FileAccess.Write))
					using (BinaryWriter bw = new BinaryWriter(fs))
					{
						if (bw.BaseStream.CanSeek)
							bw.Seek(0, SeekOrigin.End);
						_logger.AddMessage(LogSeverity.Info, string.Format("Stream position is {0}", fs.Position));
						audioStream.CopyTo(fs);
						fs.Flush();
					}
				}
			}
			catch (IOException iEx) { _logger.AddMessage(LogSeverity.Error, "IOException", iEx); }
			catch (Exception ex) { _logger.AddMessage(LogSeverity.Error, "Exception", ex); }
		}

		[ActionName("getRecord")]
		[HttpGet]
		public HttpResponseMessage GetRecord([FromUri] string record)
		{
			if (string.IsNullOrEmpty(record))
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest));

			string root = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Audio");
			string fullPath = Path.Combine(root, Path.ChangeExtension(record.IndexOf('-') != -1 ? record.Replace("-", string.Empty) : record,
																		"wav"));
			if (File.Exists(fullPath))
			{
				HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
				using (var fs = new FileStream(fullPath, FileMode.Open))
				{
					result.Content = new StreamContent(fs);
					//result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
					//result.Content.Headers.ContentDisposition.FileName = Path.GetFileName(fullPath);
					result.Content.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");
				}
				return result;
			}
			else
				return Request.CreateResponse(HttpStatusCode.Gone);
		}

		[ActionName("GetStatus")]
		[HttpGet()]
		public HttpResponseMessage GetStatus()
		{
			Guid id;
			if (!Guid.TryParse(Request.Content.ReadAsStringAsync().Result, out id))
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
			return new HttpResponseMessage() { Content = new StringContent(_manager.GetStatus(id).ToString()) };
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
	}
}