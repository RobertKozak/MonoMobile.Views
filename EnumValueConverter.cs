namespace MonoTouch.Dialog
{
	using System;
	using MonoTouch.MVVM;
	using System.Globalization;

	public class EnumValueConverter: IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Enum.Parse(targetType, (string)value);
		}
	}
}

