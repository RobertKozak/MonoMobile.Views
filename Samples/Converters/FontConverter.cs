namespace Samples
{
	using System;
	using MonoTouch.Dialog;
	using System.Globalization;
	using MonoTouch.MVVM;
	using MonoTouch.UIKit;

	public class FontConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return null;

			var font = UIFont.BoldSystemFontOfSize((float)value);

			return font;
		}

		public object ConvertBack(object value, Type targteType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

