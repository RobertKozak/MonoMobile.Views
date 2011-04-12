namespace Samples
{
	using System;
	using MonoMobile.MVVM;
	using System.Globalization;
	using MonoTouch.UIKit;
	using System.Collections.Generic;
	
	public class CurrentMovieConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var movieList = parameter as List<MovieView>;
			
			int index = System.Convert.ToInt32(value);

			return movieList[index];
		}

		public object ConvertBack(object value, Type targteType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

