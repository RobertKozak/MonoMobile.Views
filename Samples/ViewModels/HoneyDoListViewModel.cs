using System;
using MonoMobile.Views;
using System.Collections.Generic;
namespace Samples
{
	public class HoneyDoListViewModel : ViewModel
	{
		public List<string> Items 
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
			Items = new List<string>(new string []
			{
				"Take out trash",
				"Buy Groceries",
				"Go to bank",
				"Finish MultiselectCollection support"
			});
			
		}
	}
}

