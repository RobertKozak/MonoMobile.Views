namespace DealerCenter
{
	using System;
	using MonoTouch.UIKit;
	using MonoMobile.MVVM;
	using MonoTouch.Foundation;
	
	[Preserve(AllMembers = true)]
	public class RowHeightTheme : Theme
	{
		public RowHeightTheme()
		{
			CellHeight = 30;
			TextFont = UIFont.BoldSystemFontOfSize(14);
			DetailTextFont = UIFont.BoldSystemFontOfSize(14);
			DetailTextColor = UIColor.DarkGray;
		}
	}
}

