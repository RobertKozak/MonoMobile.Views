namespace DealerCenter
{
	using System;
	using MonoMobile.MVVM;

	public class LoginViewModel : ViewModel
	{
		public string UserName 
		{
			get { return Get(() => UserName, ()=> Load("UserName")); }
			set { Set(() => UserName, value); Save("UserName", value); }
		}

		public string Password
		{
			get { return Get(() => Password); }
			set { Set(() => Password, value); }
		}

		public bool UsingSampleData
		{
			get { return Get(() => UsingSampleData); }
			set { Set(() => UsingSampleData, value); }
		}
	}
}

