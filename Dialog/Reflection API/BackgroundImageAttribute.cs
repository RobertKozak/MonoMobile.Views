namespace MonoTouch.Dialog
{
	using System;

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public class BackgroundImageAttribute : Attribute
	{
		public BackgroundImageAttribute(string imageName)
		{
			ImageName = imageName;
		}

		public string ImageName { get; set; }
	}
}

