namespace Samples
{
	using System;
	using MonoTouch.MVVM;
	using MonoTouch.Dialog;
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
			return new RootElement<int>("Demos")
			{
				new Section()
				{
					new ButtonElement("Movie List Sample"){ Command = new UICommand<object>((o)=>Push(new MovieListView()), null) }
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

