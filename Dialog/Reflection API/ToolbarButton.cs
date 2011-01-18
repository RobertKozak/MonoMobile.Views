namespace MonoTouch.MVVM
{
	using System;
	using MonoTouch.UIKit;

	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public class ToolbarButtonAttribute: Attribute
	{
		public ToolbarButtonAttribute(UIBarButtonSystemItem buttonType)
		{
			ButtonType = buttonType;
		}

		public UIBarButtonSystemItem ButtonType { get; set; }
	}
}

