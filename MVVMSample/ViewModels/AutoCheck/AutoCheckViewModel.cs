using System;
using MonoMobile.MVVM;
using System.Collections.Generic;
namespace DealerCenter
{
	public class AutoCheckViewModel : ViewModel
	{
		public InventoryItemViewModel Inventory
		{
			get { return Get(() => Inventory); }
			set { Set(() => Inventory, value); }
		}
		
		public string CountryOfAssembly 
		{
			get { return Get(() => CountryOfAssembly); }
			set { Set(() => CountryOfAssembly, value); }
		}

		public AutoCheckRecordSummary RecordSummary
		{
			get { return Get(() => RecordSummary); }
			set { Set(() => RecordSummary, value); }
		}

		public AutoCheckHistoryViewModel History
		{
			get { return Get(() => History); }
			set { Set(() => History, value); }
		}

		public override string ToString()
		{
			return string.Empty;
		}
	}
}

