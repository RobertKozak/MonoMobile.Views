using System;
using MonoMobile.MVVM;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Threading;

namespace DealerCenter
{
	[Theme(typeof(GroupedTheme))]
	[Preserve(AllMembers = true)]
	public class NadaBookView : View
	{
		protected new NadaViewModel DataContext { get { return (NadaViewModel)GetDataContext(); } }

	[Section]
	
		public string Period;

	[Section("Base")]

		[Caption("Trade Clean")]
		[Theme(typeof(RowHeightTheme))]
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public string BaseTradeClean;

		[Caption("Trade Average")]
		[Theme(typeof(RowHeightTheme))]
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public string BaseTradeAverage;

		[Caption("Trade Rough")]
		[Theme(typeof(RowHeightTheme))]
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public string BaseTradeRough;

		[Caption("Loan")]
		[Theme(typeof(RowHeightTheme))]
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public string BaseLoan;

		[Caption("Retail")]
		[Theme(typeof(RowHeightTheme))]
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public string BaseRetail;
		
	[Section("Package")]

		[Caption("Trade Clean")]
		[Theme(typeof(RowHeightTheme))]
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public string PackageTradeClean;

		[Caption("Trade Average")]
		[Theme(typeof(RowHeightTheme))]
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public string PackageTradeAverage;
		
		[Caption("Trade Rough")]
		[Theme(typeof(RowHeightTheme))]
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public string PackageTradeRough;
		
		[Caption("Loan")]
		[Theme(typeof(RowHeightTheme))]
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public string PackageLoan;
		
		[Caption("Retail")]
		[Theme(typeof(RowHeightTheme))]
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public double PackageRetail;
	
	[Section("Total")]

		[Caption("Trade Clean")]
		[Theme(typeof(RowHeightTheme))]
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public string TotalTradeClean;
		
		[Caption("Trade Average")]
		[Theme(typeof(RowHeightTheme))]
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public string TotalTradeAverage;

		[Caption("Trade Rough")]
		[Theme(typeof(RowHeightTheme))]
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public string TotalTradeRough;

		[Caption("Loan")]
		[Theme(typeof(RowHeightTheme))]
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public string TotalLoan;

		[Caption("Retail")]
		[Theme(typeof(RowHeightTheme))]
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public string TotalRetail;

	[Section]
		[NavbarButton("Book Vehicle")]
		public void BookVehicle()
		{	
			var progress = new ProgressHud() { TitleText = "Retrieving Values" };
			progress.ShowWhileExecuting(() =>
			{
				DataContext.BaseTradeClean = 13000.00f;
				DataContext.BaseTradeAverage = 11825.00f;
				DataContext.BaseTradeRough = 10400.00f;
				DataContext.BaseLoan = 11700.00f;
				DataContext.BaseRetail = 16200.00;
				
				DataContext.PackageTradeClean = 0f;
				DataContext.PackageTradeAverage = 0f;
				DataContext.PackageTradeRough = 0f;
				DataContext.PackageLoan = 0f;
				DataContext.PackageRetail = 0f;
				
				DataContext.TotalTradeClean = 13000f;
				DataContext.TotalTradeAverage = 11825.00f;
				DataContext.TotalTradeRough = 10400.00f;
				DataContext.TotalLoan = 11700.00f;
				DataContext.TotalRetail = 16200.00;
				
				DataContext.InventoryItem.NADABooked = true;
				Thread.Sleep(1000);
			}, true);
		}
	}
}

