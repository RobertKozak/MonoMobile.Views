using System.Linq;
namespace DealerCenter
{
	using System;
	using System.Globalization;
	using MonoMobile.MVVM;
	using MonoTouch.Foundation;

	[Preserve(AllMembers = true)]
	public class Last6VINConverter : IValueConverter
	{
		public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((string)value).Substring(11, 6);
		}

		public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}

	}
}

