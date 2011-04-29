namespace Samples
{
	using System;
	using MonoMobile.MVVM;
	using MonoTouch.Foundation;
	using System.IO;
	using MonoTouch.UIKit;
	using System.ComponentModel;

	[Theme(typeof(HoneyDoTheme))]
	[Theme(typeof(FrostedTheme))]
	public class HoneyDoList
	{
		[Checkbox]
		[DefaultValue(true)]
		public bool SubmitApp
		{
			get;
			set;
		}
		[Checkbox]
		public bool OrderNewSupplies
		{
			get;
			set;
		}
		[Checkbox]
		public bool BuyTickets
		{
			get;
			set;
		}
		[Checkbox]
		[DefaultValue(true)]
		public bool PickUpBooks
		{
			get;
			set;
		}
		public HoneyDoList()
		{
			
		}
	}

	public class HoneyDoTheme : Theme
	{
		[Preserve]
		public HoneyDoTheme()
		{
			BackgroundUri = new Uri("file://" + Path.GetFullPath("Images/honeycomb.jpg"));
			this.BarTintColor = UIColor.Orange;
			this.BarTranslucent = true;
		}
	}
}

