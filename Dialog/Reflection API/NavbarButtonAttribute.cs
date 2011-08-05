namespace MonoMobile.Views
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

		public NavbarButtonAttribute(Type viewType) : this()
		{
			ViewType = viewType;
		}

		public NavbarButtonAttribute(string canExecutePropertyName) : this()
		{
			CanExecutePropertyName = canExecutePropertyName;
		}

		public string CanExecutePropertyName;
		public CommandOption CommandOption;
		public UIBarButtonSystemItem? ButtonType { get; set; }
		public UIBarButtonItemStyle Style { get; set; }
		public BarButtonLocation Location { get; set; }
		public Type ViewType { get; set; }
	}
}

