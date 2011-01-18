using System;
namespace MonoTouch.MVVM
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
	public class MapAttribute : Attribute
	{
		public MapAttribute(string caption, string value)
		{
			Caption = caption;
			Value = value;
		}

		public string Caption {get;set;}
		public string Value {get;set;}
	}
}

