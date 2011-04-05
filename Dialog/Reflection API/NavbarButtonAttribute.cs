namespace MonoMobile.MVVM
{
	using System;
	using MonoTouch.UIKit;

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, Inherited = false)]
	public class NavbarButtonAttribute : Attribute
	{
		public NavbarButtonAttribute() : base()
		{
			Style = UIBarButtonItemStyle.Bordered;
			Location = BarButtonLocation.Right;
		}

		public NavbarButtonAttribute(UIBarButtonSystemItem buttonType) : this()
		{
			ButtonType = buttonType;
		}

		public NavbarButtonAttribute(string title) : this()
		{
			Title = title;
		}

		public UIBarButtonSystemItem ButtonType { get; set; }
		public UIBarButtonItemStyle Style { get; set; }
		public string Title { get; set; }
		public BarButtonLocation Location { get; set; }
	}
}

