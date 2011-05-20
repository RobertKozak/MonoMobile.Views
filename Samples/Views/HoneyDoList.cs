using System.Collections.Generic;
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
		[Bind("Caption", "Caption")]
		[Root(ViewType = typeof(StandardListView))]
		[Multiselect]
		public List<string> Items { get; private set; }

		
		public HoneyDoListView()
		{
			var viewModel = new HoneyDoListViewModel();
			DataContext = viewModel;

			Items = viewModel.Items;
			viewModel.Caption = "Things to do";
		}

		[Section]
		[Button]
		public void ChangeCaption()
		{
			((HoneyDoListViewModel)DataContext).Caption = "TODO";
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

