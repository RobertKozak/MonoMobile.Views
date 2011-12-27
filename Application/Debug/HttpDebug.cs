using System;
using System.Net;
using System.Threading;
using System.IO;
using System.Linq;
using MonoTouch.Foundation;

namespace MonoMobile.Views
{
	public class HttpDebug: NSObject
	{
		static HttpListener listener;
		static Thread handlerThread;
		
		protected override void Dispose (bool disposing)
		{
			if (disposing && listener != null)
			{
				listener.Close();
			}

			base.Dispose(disposing);
		}

		~HttpDebug()
		{
			Dispose(false);
		}

		static public void Start()
		{
			handlerThread = new Thread(HttpServer);
			handlerThread.Start();
		}
		
		static void HttpServer()
		{
			listener = new HttpListener();
			listener.Prefixes.Add("http://*:5000/");
			listener.Start();
			while (true)
			{
				var context = listener.GetContext();
				var request = context.Request;
				var response = context.Response;

				var tw = new StreamWriter(response.OutputStream);

				var path = request.Url.AbsolutePath;
				if (path.StartsWith("/type/"))
				{
					ShowInstancesOf(tw, path.Substring(6));
				}
				else
				{
					Summary(tw);
				}

				tw.Flush();
				response.OutputStream.Close();
			}
		}

		static void Header(TextWriter c, string title, string script = null)
		{
			c.WriteLine(
				"<html><head><title>{0}</title>\n" +
				"<head>\n" +
				"  <script src='https://ajax.googleapis.com/ajax/libs/jquery/1.5/jquery.min.js'></script>", title);
			if (script != null)
			{
				c.WriteLine("<script>$(document).ready(function () {{ {0} }});</script>", script);
			}
			c.WriteLine("</head><body>");
		}

		static void Summary(TextWriter c)
		{
			Header(c, "Summary", "$(\"a.type\").click (function (e) { $(this).append ('<div></div>'); $(this).children ().load ('/type/'); console.log ($(this).contents ()); e.preventDefault ();});");
			//Header (c, "Summary", "alert ('loaded');");
			var weakList = MonoTouch.ObjCRuntime.Runtime.GetSurfacedObjects();
			c.WriteLine("<div id='foo'></div>");
			c.WriteLine("<p>Total surfaced objects: {0}", weakList.Count);
			var groups = from weak in weakList
				let nso =  weak.Target
				where nso != null
				let typeName = nso.GetType().FullName
				orderby typeName
				group nso by typeName into g
				let gCount = g.ToList().Count
				orderby gCount descending
				select new { Type = g.Key, Instances = g };
			var list = groups.ToList();
			c.WriteLine("<p>Have {0} different types surfaced", list.Count);
			c.WriteLine("<ul>");
			foreach (var type in list)
			{
				c.WriteLine("<li>{1} <a href='' class='type'>{0}</a>", type.Type, type.Instances.ToList().Count);
			}
			c.WriteLine("</ul>");
			c.WriteLine("</body></html>");
		}

		static void ShowInstancesOf(TextWriter c, string type)
		{
			var weakList = MonoTouch.ObjCRuntime.Runtime.GetSurfacedObjects();
			var res = from weak in weakList
				let nso = weak.Target
				where nso != null
				let typeName = nso.GetType().FullName
				where typeName == type
				select nso;

			c.WriteLine("<ul>");
			foreach (NSObject nso in res)
			{
				c.WriteLine("<li>0x{0:x}</li>", nso.Handle);
			}
			c.WriteLine("</ul>");
		}

	}
}
