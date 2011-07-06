using System;
using MonoMobile.MVVM;
using MonoTouch.Foundation;

namespace DealerCenter
{
	[Preserve(AllMembers = true)]
	public class NadaViewModel : ViewModel
	{
		public InventoryItemViewModel InventoryItem
		{
			get { return Get(() => InventoryItem); }
			set { Set(() => InventoryItem, value); }
		}
		
		public string Period
		{
			get { return Get(() => Period); }
			set { Set(() => Period, value); }
		}

		public double BaseTradeClean 
		{
			get { return Get(() => BaseTradeClean); }
			set { Set(() => BaseTradeClean, value); } 
		} 

		public double BaseTradeAverage
		{
			get { return Get(() => BaseTradeAverage); }
			set { Set(() => BaseTradeAverage, value); }
		}
		
		public double BaseTradeRough
		{
			get { return Get(() => BaseTradeRough); }
			set { Set(() => BaseTradeRough, value); }
		}
		
		public double BaseLoan
		{
			get { return Get(() => BaseLoan); }
			set { Set(() => BaseTradeClean, value); }
		}

		public double BaseRetail
		{
			get { return Get(() => BaseRetail); }
			set { Set(() => BaseRetail, value); }
		}
		
		public double PackageTradeClean
		{
			get { return Get(() => PackageTradeClean); }
			set { Set(() => PackageTradeClean, value); }
		}

		public double PackageTradeAverage
		{
			get { return Get(() => PackageTradeAverage); }
			set { Set(() => PackageTradeAverage, value); }
		}

		public double PackageTradeRough
		{
			get { return Get(() => PackageTradeRough); }
			set { Set(() => PackageTradeRough, value); }
		}

		public double PackageLoan
		{
			get { return Get(() => PackageLoan); }
			set { Set(() => PackageLoan, value); }
		}

		public double PackageRetail
		{
			get { return Get(() => PackageRetail); }
			set { Set(() => PackageRetail, value); }
		}

		public double TotalTradeClean
		{
			get { return Get(() => TotalTradeClean); }
			set { Set(() => TotalTradeClean, value); }
		}

		public double TotalTradeAverage
		{
			get { return Get(() => TotalTradeAverage); }
			set { Set(() => TotalTradeAverage, value); }
		}

		public double TotalTradeRough
		{
			get { return Get(() => TotalTradeRough); }
			set { Set(() => TotalTradeRough, value); }
		}

		public double TotalLoan
		{
			get { return Get(() => TotalLoan); }
			set { Set(() => TotalLoan, value); }
		}

		public double TotalRetail
		{
			get { return Get(() => TotalRetail); }
			set { Set(() => TotalRetail, value); }
		}

		public NadaViewModel()
		{
			Period = "2011 May";
		}

		public override string ToString()
		{
			return string.Format ("{0:c2}", TotalTradeClean);
		}
	}
}

