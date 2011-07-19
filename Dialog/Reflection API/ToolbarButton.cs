namespace MonoMobile.MVVM
{
	using System;
	using MonoTouch.UIKit;

	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public class ToolbarButtonAttribute: Attribute
	{
		public ToolbarButtonAttribute()
		{
			Style = UIBarButtonItemStyle.Bordered;
			Location = BarButtonLocation.Center;
		}
		
		public ToolbarButtonAttribute(UIBarButtonSystemItem buttonType): this()
		{
			ButtonType = buttonType;
		}
		
		public ToolbarButtonAttribute(Type viewType) : this()
		{
			ViewType = viewType;
		}

		public ToolbarButtonAttribute(string canExecutePropertyName) : this()
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

