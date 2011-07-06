using System;
using MonoMobile.MVVM;
using System.Collections.Generic;
namespace DealerCenter
{
	public class BlackBookViewModel : ViewModel
	{
		public InventoryItemViewModel InventoryItem
		{
			get { return Get(() => InventoryItem); }
			set { Set(() => InventoryItem, value); }
		}

		public List<string> Period
		{
			get { return Get(() => Period); }
			set { Set(() => Period, value); }
		}

		public double WholesaleXClean
		{
			get { return Get(() => WholesaleXClean); }
			set { Set(() => WholesaleXClean, value); }
		}

		public double WholesaleClean
		{
			get { return Get(() => WholesaleClean); }
			set { Set(() => WholesaleClean, value); }
		}

		public double WholesaleAverage
		{
			get { return Get(() => WholesaleAverage); }
			set { Set(() => WholesaleAverage, value); }
		}

		public double WholesaleRough
		{
			get { return Get(() => WholesaleRough); }
			set { Set(() => WholesaleRough, value); }
		}
		
		public double RetailXClean
		{
			get { return Get(() => RetailXClean); }
			set { Set(() => RetailXClean, value); }
		}

		public double RetailClean
		{
			get { return Get(() => RetailClean); }
			set { Set(() => RetailClean, value); }
		}

		public double RetailAverage
		{
			get { return Get(() => RetailAverage); }
			set { Set(() => RetailAverage, value); }
		}

		public double RetailRough
		{
			get { return Get(() => RetailRough); }
			set { Set(() => RetailRough, value); }
		}


		public BlackBookViewModel()
		{
			Period = new List<string>() { "5/23/2011", "5/30/2011" };
		}

		public override string ToString()
		{
			return string.Format("{0:c2}", WholesaleAverage);
		}
	}
}

