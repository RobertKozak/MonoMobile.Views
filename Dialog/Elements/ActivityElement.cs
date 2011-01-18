namespace MonoTouch.Dialog
{
	using System.Drawing;
	using MonoTouch.UIKit;

	public class ActivityElement : UIViewElement, IElementSizing
	{
		public ActivityElement() : base ("", new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray), false)
		{
			var sbounds = UIScreen.MainScreen.Bounds;			
			var uia = Value as UIActivityIndicatorView;
			
			uia.StartAnimating();
			
			var vbounds = Value.Bounds;
			Value.Frame = new RectangleF((sbounds.Width-vbounds.Width)/2, 4, vbounds.Width, vbounds.Height + 0);
			Value.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleRightMargin;
		}
		
		public bool Animating {
			get
			{
				return ((UIActivityIndicatorView)Value).IsAnimating;
			}
			set
			{
				var activity = Value as UIActivityIndicatorView;

				if (value)
					activity.StartAnimating();
				else
					activity.StopAnimating();
			}
		}
		
		public override float GetHeight (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			return base.GetHeight(tableView, indexPath)+ 8;
		}		
	}
}

