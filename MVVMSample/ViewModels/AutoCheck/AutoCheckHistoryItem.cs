using System;
using MonoMobile.MVVM;
namespace DealerCenter
{
	public class AutoCheckHistoryItem : ViewModel
	{
		public string Description
		{
			get { return Get(() => Description); }
			set { Set(() => Description, value); }
		}

		public bool Result
		{
			get { return Get(() => Result); }
			set { Set(() => Result, value); }
		}

		public override string ToString()
		{
			return Description;
		}
	}
}

