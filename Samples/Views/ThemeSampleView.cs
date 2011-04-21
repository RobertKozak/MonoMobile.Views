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


//	public class ThemeConverter : IValueConverter
//	{
//		public object Convert(object value, Type targetType, object parameter, Globalization.CultureInfo culture)
//		{
//		}
//
//		public object ConvertBack(object value, Type targetType, object parameter, Globalization.CultureInfo culture)
//		{
//			throw new NotImplementedException ();
//		}
//	}
}

