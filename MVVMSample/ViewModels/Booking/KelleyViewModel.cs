using System;
using MonoMobile.MVVM;
namespace DealerCenter
{
	public class KelleyViewModel : ViewModel
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

		public double WholesaleBase
		{
			get { return Get(() => WholesaleBase); }
			set { Set(() => WholesaleBase, value); }
		}

		public double WholesalePackage
		{
			get { return Get(() => WholesalePackage); }
			set { Set(() => WholesalePackage, value); }
		}

		public double WholesaleTotal
		{
			get { return Get(() => WholesaleTotal); }
			set { Set(() => WholesaleTotal, value); }
		}

		public double RetailBase
		{
			get { return Get(() => RetailBase); }
			set { Set(() => RetailBase, value); }
		}

		public double RetailPackage
		{
			get { return Get(() => RetailPackage); }
			set { Set(() => RetailPackage, value); }
		}

		public double RetailTotal
		{
			get { return Get(() => RetailTotal); }
			set { Set(() => RetailTotal, value); }
		}

		public KelleyViewModel()
		{
			Period = "5/20-5/26 2011";
		}

		public override string ToString()
		{
			return string.Format("{0:c2}", WholesaleTotal);
		}
	}
}

