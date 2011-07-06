namespace Samples
{
	using System;
	using MonoMobile.MVVM;
	using System.Collections.Generic;
	using System.Linq;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	public class MenuDialog : DialogViewController 
	{
		public MenuDialog(bool pushing) : base(pushing) 
		{
			PrepareRoot(CreateRoot());
			Autorotate = true;
		}
		
		private IRoot CreateRoot()
		{
			return new RootElement("Demos")
			{
				new Section()
				{
					new ButtonElement("Movie List Sample"){ Command = new UICommand<object>((o)=> { Push(new MovieListView()); return null; }, null) }
				},
			};
		}
		
		private void Push(View view)
		{ 
			var binding = new BindingContext(view, "");
			NavigationController.PushViewController(new DialogViewController(binding.Root, true), true);
		}
	}
}

