using System.Collections.Generic;
using System.ComponentModel;
namespace DealerCenter
{
	using System;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using MonoMobile.MVVM;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	[Theme(typeof(DealerCenterTheme))]
	[Theme(typeof(PlainTheme))]
	[Searchbar(IncrementalSearch = true)]
	public class InventoryListView : View, IDataContext
	{	
		protected new InventoryViewModel DataContext { get { return (InventoryViewModel)GetDataContext(); } }

		[List(ViewType = typeof(InventoryItemView), ElementType = typeof(InventoryElement))]
		[Bind("CurrentInventoryList")]
		[CellEditingStyle(UITableViewCellEditingStyle.Delete)]
		public ObservableCollection<InventoryItemViewModel> Inventory { get { return DataContext.CurrentInventoryList; } }
		
		
		[NavbarButton(UIBarButtonSystemItem.Edit)]
		public void Edit()
		{
			var tableView  = Application.CurrentDialogViewController.TableView as DialogViewTable;

			tableView.SetEditing(true, true);

			var oldButton = tableView.Controller.NavigationItem.RightBarButtonItem;

			tableView.Controller.NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate 
			{
				tableView.SetEditing(false, true);
				tableView.Controller.NavigationItem.RightBarButtonItem = oldButton;
			});
		}

		[NavbarButton("Logout", Location = BarButtonLocation.Left)]
		[Bind("LoginCaption", "Caption")]
		public void Login()
		{
			Application.PresentModelView(new ReloginView());
		}

		[ToolbarButton(UIBarButtonSystemItem.Search, Style = UIBarButtonItemStyle.Plain, Location = BarButtonLocation.Left)]
		public void Search()
		{
			Application.ToggleSearchbar();
		}

		[ToolbarButton(typeof(InventoryListSelector))]
		public void Selector(int segmentIndex)
		{
			if (segmentIndex == 0)
				DataContext.CurrentInventoryList = DataContext.DCInventory;
			if (segmentIndex == 1)
				DataContext.CurrentInventoryList = DataContext.FavoriteInventory;
			if (segmentIndex == 2)
				DataContext.CurrentInventoryList = DataContext.LocalInventory;

			Application.CurrentDialogViewController.Root.DataContext = Inventory;
			((RootElement)Application.CurrentDialogViewController.Root).Reload(Application.CurrentDialogViewController.Root.Sections[0], UITableViewRowAnimation.None);
		}

		[ToolbarButton(UIBarButtonSystemItem.Compose, Style = UIBarButtonItemStyle.Plain, Location = BarButtonLocation.Right)]
		public void Compose()
		{
			Inventory.Add(new InventoryItemViewModel());
		}

		public InventoryListView()
		{
			var login = new LoginView();
			login.Initialize();

			Application.PresentModelView(login);

			var model = new InventoryModel();
			model.Load();
			
			var viewModel = new InventoryViewModel(model);

			base.DataContext = viewModel;
		}
		
		[PullToRefresh]
		public void Refresh()
		{
			//DataContext.Refresh();
			Inventory.Add(new InventoryItemViewModel()
	      	{
				Id = 4,
				Year = 2007,
				Make = "BMW",
				Model = "325i", 
				Trim = "4D Sedan", 
				StockNumber = "S2344", 
				Vin = "S024GPATN836DF834"
			});

		}
	}
}



