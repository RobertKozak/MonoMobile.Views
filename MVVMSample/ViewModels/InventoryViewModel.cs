namespace MVVMSample
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using MonoMobile.MVVM;

	public class InventoryViewModel : ViewModel
	{
		private InventoryModel Model { get { return (InventoryModel)base.Model; } set { base.Model = value; } }

		public ObservableCollection<InventoryItemViewModel> Inventory { get { return Get(()=>Model.Inventory); } }

		public InventoryViewModel(InventoryModel model)
		{
			Model = model;
		}
	}
}

