using System;
using MonoMobile.MVVM;
using System.Collections.Generic;
namespace Samples
{
	public class ThemeSampleViewModel : ViewModel
	{		
		private List<Theme> _Themes = new List<Theme>()
		{
			new AutumnTheme(),
			new BrickedTheme(),
			new CorkTheme(),
			new DenimTheme(),
			new FrostedTheme()
		};
		
		public List<Theme> Themes { get; set; }
		
		public ThemeSampleViewModel()
		{
		}
	}
}

