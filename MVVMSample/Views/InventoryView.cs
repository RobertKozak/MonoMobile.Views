using MonoTouch.UIKit;
namespace MVVMSample
{
	using System;
	using MonoTouch.Foundation;
	using MonoMobile.MVVM;
	using Nowcom.VersionedLocalDB.DMS.Inventory;

	[Theme(typeof(GroupedTheme))]
	public class InventoryView: View, IDataContext
	{		
		[Skip]
		public InventoryItemViewModel DataContext { get { return (InventoryItemViewModel)base.DataContext; } set { base.DataContext = value; } }

	[Section]
		[Entry]
		public string StockNumber { get; set; }

		[Entry]
		public int Year { get; set; }

		[Entry]
		public string Make { get; set; }

		[Entry]
		public string Model { get; set; }

		[Entry]
		public string Trim { get; set; }

		[Entry]
		public string Vin { get; set; }

		[ToolbarButton("Kelley")]
		public void KelleyBlueBook()
		{
			DataContext.KelleyMethod();
		}

		[ToolbarButton("NADA")]
		public void NADA()
		{
			var binding = new BindingContext(MonoMobileApplication.MainView, MonoMobileApplication.Title);
			var controller = new DialogViewController(UITableViewStyle.Grouped, binding, true) { Autorotate = true };
		
			controller.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
controller.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
			Application.NavigationController.PresentModalViewController(controller, true);
		}

		[ToolbarButton("BlackBook")]
		public void BlackBook()
		{
		}

		[ToolbarButton("Manheim")]
		public void Manheim()
		{
		}

		[ToolbarButton("AutoCheck")]
		public void AutoCheck()
		{
		}

		public InventoryView()
		{
		}

		public override string ToString()
		{
			return string.Format ("{1} {2} {3}",Year, Make, Model);
		}
	}
}

