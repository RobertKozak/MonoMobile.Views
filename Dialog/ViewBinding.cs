namespace MonoMobile.Views
{
	using System;
	using System.Reflection;
	using MonoTouch.UIKit;

	public class ViewBinding
	{		
		public object DataContext { get; set; }
		public DataContextCode DataContextCode { get; set; }
		public MemberInfo MemberInfo { get; set; }

		public Type ElementType { get; set; }

		public Type ViewType { get; set; }
		
		public UIView View { get; set; }
		public UIView CurrentView
		{
			get 
			{
				if (View != null)
					return View;

				if (DataContext != null)
				{
					var value = MemberInfo.TryGetValue(DataContext) as UIView;
					if (value != null)
						return value;
				}
				
				return null; 
			}
		}

		public ViewBinding()
		{
		}
	}
}

