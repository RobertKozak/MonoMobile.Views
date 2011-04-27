using System;
using System.Collections.Generic;
using MonoMobile.MVVM;
namespace Samples
{
	public class ThemeSampleView: View
	{
		
		[Bind("Themes", "Theme")]
		public List<Theme> Themes { get; set; }	
	}
}

