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
	public class HoneyDoListView: View
	{
		[List]
		public MultiselectCollection<string> Items { get; private set; }
		public HoneyDoListView()
		{
			var viewModel = new HoneyDoListViewModel();
			Items = viewModel.Items;
			DataContext = viewModel;
		}
	}

	public class HoneyDoTheme : Theme
	{
		[Preserve]
		public HoneyDoTheme()
		{
			BackgroundUri = new Uri("file://" + Path.GetFullPath("Images/honeycomb.jpg"));
			this.BarTintColor = UIColor.Orange;
		//	this.BarTranslucent = true;
		}
	}
}

