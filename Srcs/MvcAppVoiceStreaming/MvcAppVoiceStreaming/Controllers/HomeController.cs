using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace MvcAppVoiceStreaming.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

			return View();
		}

		public ActionResult About()
		{
			ViewBag.Message = "HTML5 Voice Streaming Proof-Of-Concept.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Voicer has been written by Oleksandr Kulchytskyi.";

			return View();
		}

		public ActionResult Download([FromUri]string record)
		{
			string root = Server.MapPath("~/App_Data/Audio");
			string fullPath = System.IO.Path.Combine(root, System.IO.Path.ChangeExtension(record.IndexOf('-') != -1 ? record.Replace("-", string.Empty) : record,
																		"wav"));
			if (System.IO.File.Exists(fullPath))
			{
				string contentType = "audio/wav";
				return File(fullPath, contentType, System.IO.Path.GetFileName(fullPath));
			}
			else
				return View();
		}
	}
}
