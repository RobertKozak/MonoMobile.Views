using System;
using System.Collections.Generic;
using MonoMobile.MVVM;
using System.Collections.ObjectModel;
using MonoTouch.Foundation;

namespace Samples
{
	[Preserve(AllMembers=true)]
	public class ThemeSampleView: View
	{
		[Bind("Themes", "Index")]
		public int Selected { get; set; }

		[Bind("Themes", "Theme")]
		public ObservableCollection<Theme> Themes { get; set; }	

		public ThemeSampleView()
		{
			DataContext = new ThemeSampleViewModel();
		}
	}
}

