namespace MonoTouch.MVVM
{
	using System;
	using MonoTouch.UIKit;

	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public class ToolbarButtonAttribute: Attribute
	{
		public ToolbarButtonAttribute()
		{
			Style = UIBarButtonItemStyle.Bordered;
		}
		
		public ToolbarButtonAttribute(UIBarButtonSystemItem buttonType): this()
		{
			ButtonType = buttonType;
		}

		public UIBarButtonSystemItem ButtonType { get; set; }
		public UIBarButtonItemStyle Style { get; set; }
	}
}

