using System;
using MonoMobile.MVVM;
using MonoTouch.Foundation;
using System.Threading;

namespace DealerCenter
{
	[Theme(typeof(GroupedTheme))]
	[Preserve(AllMembers = true)]
	public class KelleyBookView: View
	{
		protected new KelleyViewModel DataContext { get { return (KelleyViewModel)GetDataContext(); } }
		
	[Section]
		public string Period;

	[Section("Wholesale")]
		[Caption("Base")]
		[Theme(typeof(RowHeightTheme))]
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public string WholesaleBase;

		[Caption("Package")]
		[Theme(typeof(RowHeightTheme))]
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public string WholesalePackage;

		[Caption("Total")]
		[Theme(typeof(RowHeightTheme))]
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public string WholesaleTotal;

	[Section("Retail")]
		[Caption("Base")]
		[Theme(typeof(RowHeightTheme))]
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public string RetailBase;

		[Caption("Package")]
		[Theme(typeof(RowHeightTheme))]
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public string RetailPackage;

		[Caption("Total")]
		[Theme(typeof(RowHeightTheme))]
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public string RetailTotal;

		[Section]
		[NavbarButton("Book Vehicle")]
		public void BookVehicle()
		{
			var progress = new ProgressHud() { TitleText = "Retrieving Values" };
			progress.ShowWhileExecuting(()=>
			{
				DataContext.WholesaleBase = 11250.00f;
				DataContext.WholesalePackage = 1050.00f;
				DataContext.WholesaleTotal = 11825.00f;
		
				DataContext.RetailBase = 14700.00f;
				DataContext.RetailPackage = 1400.00f;
				DataContext.RetailTotal = 15625.00f;
				
				DataContext.InventoryItem.KelleyBooked = true;

				Thread.Sleep(1000);
			}, true);
		}
	}
}

