namespace DealerCenter
{
	using System;
	using MonoTouch.Foundation;
	using MonoMobile.MVVM;
	
	[Preserve(AllMembers = true)]
	public class InventoryMiniViewDataTemplate : ElementDataTemplate
	{
		public InventoryMiniView View { get { return Element as InventoryMiniView; } }

		public BindableProperty FavoriteProperty = BindableProperty.Register("Hidden");
		
		public InventoryMiniViewDataTemplate(IElement element) : base(element)
		{

		}

		public override void BindProperties()
		{
			base.BindProperties();

		//	FavoriteProperty.BindTo(View.Favorite, "Favorite", DataContext);
		}
	}
}

