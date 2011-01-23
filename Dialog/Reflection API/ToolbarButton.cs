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

		public ToolbarButtonAttribute(string title) : this()
		{
			Title = title;
		}

		public UIBarButtonSystemItem ButtonType { get; set; }
		public UIBarButtonItemStyle Style { get; set; }
		public string Title { get; set; }
		public BarButtonLocation Location { get; set; }
	}
}

