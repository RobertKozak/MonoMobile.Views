using MonoTouch.UIKit;
namespace MVVMSample
{
	using System;
	using MonoMobile.MVVM;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;

	[Theme(typeof(SampleTheme))]
	public class MainView : View, IDataContext
	{	
		private InventoryViewModel DataContext { get { return (InventoryViewModel)base.DataContext; } }

		[List(ViewType = typeof(InventoryView), ElementType = typeof(InventoryElement))]
		[Theme(typeof(PlainStyle))]
		public ObservableCollection<InventoryItemViewModel> Inventory { get { return DataContext.Inventory; } }

		public MainView()
		{
			var model = new InventoryModel();
			model.Load();
			
			var viewModel = new InventoryViewModel(model);

			base.DataContext = viewModel;
		}
		
		[PullToRefresh]
		public void Refresh()
		{
			DataContext.Refresh();
		}
	}
}

