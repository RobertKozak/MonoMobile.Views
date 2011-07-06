namespace DealerCenter
{
	using System.Collections.Generic;
	using System.Threading;
	using MonoMobile.MVVM;
	using MonoTouch.Foundation;

	[Theme(typeof(GroupedTheme))]
	[Preserve(AllMembers = true)]
	public class BlackBookView : View
	{
		protected new BlackBookViewModel DataContext { get { return (BlackBookViewModel)GetDataContext(); } }

		[Section]
		public List<string> Period = new List<string>();
		
	[Section("Wholesale")]

		[Caption("XClean")]
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		[Theme(typeof(RowHeightTheme))]
		public string WholesaleXClean;

		[Caption("Clean")]
		[Theme(typeof(RowHeightTheme))]
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public string WholesaleClean;

		[Caption("Average")]
		[Theme(typeof(RowHeightTheme))]
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public string WholesaleAverage;

		[Caption("Rough")]
		[Theme(typeof(RowHeightTheme))]
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public string WholesaleRough;


	[Section("Retail")]

		[Caption("XClean")]
		[Theme(typeof(RowHeightTheme))]
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public string RetailXClean;//		[Caption("Total Wholesale")]

		[Caption("Clean")]
		[Theme(typeof(RowHeightTheme))]
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public string RetailClean;

		[Caption("Average")]
		[Theme(typeof(RowHeightTheme))]
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public string RetailAverage;

		[Caption("Rough")]
		[Theme(typeof(RowHeightTheme))]
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public string RetailRough;

	[Section]

		[NavbarButton("Book Vehicle")]
		public void BookVehicle()
		{
			var progress = new ProgressHud() { TitleText = "Retrieving Values" };
			progress.ShowWhileExecuting(() =>
			{
				DataContext.WholesaleXClean = 0f;
				DataContext.WholesaleClean = 15775.00f;
				DataContext.WholesaleAverage = 13400f;
				DataContext.WholesaleRough = 11575.00f;
				
				DataContext.RetailXClean = 0f;
				DataContext.RetailClean = 19050.00f;
				DataContext.RetailAverage = 16300.00f;
				DataContext.RetailRough = 13950.00f;

				DataContext.InventoryItem.BlackBookBooked = true;
				Thread.Sleep(1000);
			}, true);
		}
	}
}

