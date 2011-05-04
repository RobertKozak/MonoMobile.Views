using System;
using MonoMobile.MVVM;
namespace Samples
{
	public class HoneyDoListViewModel : ViewModel
	{
		public MultiselectCollection<string> Items 
		{
			get { return Get(()=>Items); }
			private set { Set(()=>Items, value); }
		}

		public string Caption
		{
			get { return Get(()=>Caption, "To Do List"); }
			set { Set(()=>Caption, value); }
		}

		public HoneyDoListViewModel()
		{
			Items = new MultiselectCollection<string>(new string []
			{
				"Take out trash",
				"Buy Groceries",
				"Go to bank",
				"Finish MultiselectCollection support"
			});
			
		}
	}
}

