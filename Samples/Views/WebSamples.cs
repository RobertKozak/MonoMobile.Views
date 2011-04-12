using System;
using MonoMobile.MVVM;

namespace Samples
{
	public class WebSamples : View
	{
		public Uri Google { get; set; }

		[Html]
		public string Nowcom { get; set; }

		public WebSamples()
		{
			Google = new Uri("Http://www.google.com");
			Nowcom = "Http://www.nowcom.com";
		}

	}
}

