namespace MonoMobile.MVVM
{
	using System;
	using System.Reflection;
	using MonoTouch.UIKit;

	public class ViewBinding
	{		
		public object DataContext { get; set; }
		public DataContextCode DataContextCode { get; set; }
		public MemberInfo MemberInfo { get; set; }
		public Type ViewType { get; set; }
		
		public UIView View { get; set; }
		public UIView CurrentView
		{
			get 
			{
				var value = GetValue(MemberInfo, DataContext) as UIView;
				if (value != null)
					return value;

				return View; 
			}
		}

		public ViewBinding()
		{
		}

		public static object GetValue(MemberInfo mi, object o)
		{
			var fi = mi as FieldInfo;
			if (fi != null)
			{
				return fi.GetValue(o);
			}

			var pi = mi as PropertyInfo;
			if (pi != null)
			{
				object value = pi.GetValue(o,new object[] {}); 
				return value;
			}

			return null;
		}
	}
}

