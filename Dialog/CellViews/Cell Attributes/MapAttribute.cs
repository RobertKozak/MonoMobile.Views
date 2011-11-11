using System;
namespace MonoMobile.Views
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
	public class MapAttribute : BaseControlAttribute
	{
		public MapAttribute(string caption)
		{
			Caption = caption;
		}

		public string Caption { get;set; }
	}
}

