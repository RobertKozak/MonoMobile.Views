namespace MonoTouch.MVVM
{
	using System;
	using MonoTouch.UIKit;

	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public class EditButtonAttribute : Attribute
	{
		public EditButtonAttribute(UIBarButtonSystemItem buttonType)
		{
			ButtonType = buttonType;
		}

		public UIBarButtonSystemItem ButtonType { get; set; }
	}
}

