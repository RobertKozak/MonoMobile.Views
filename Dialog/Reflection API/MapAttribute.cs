using System;
namespace MonoMobile.MVVM
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
	public class MapAttribute : Attribute
	{
		public MapAttribute(string caption)
		{
			Caption = caption;
		}

		public string Caption { get;set; }
	}
}

