using System.Collections.Generic;
namespace Samples
{
	using System;
	using MonoMobile.Views;
	using MonoTouch.Foundation;
	using System.IO;
	using MonoTouch.UIKit;
	using System.ComponentModel;

	[Theme(typeof(HoneyDoTheme))]
	public class HoneyDoListView: View
	{
		[List(SelectionAction = SelectionAction.Multiselection)]
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

