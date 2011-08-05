// 
//  DialogViewTable.cs
// 
//  Author:
//    Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
// 
//  Copyright 2011, Nowcom Corporation.
// 
//  Code licensed under the MIT X11 license
// 
//  Permission is hereby granted, free of charge, to any person obtaining
//  a copy of this software and associated documentation files (the
//  "Software"), to deal in the Software without restriction, including
//  without limitation the rights to use, copy, modify, merge, publish,
//  distribute, sublicense, and/or sell copies of the Software, and to
//  permit persons to whom the Software is furnished to do so, subject to
//  the following conditions:
// 
//  The above copyright notice and this permission notice shall be
//  included in all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//  LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//  OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//  WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
namespace MonoMobile.Views
{
	using System;
	using MonoTouch.UIKit;
	using System.Drawing;
	using MonoTouch.Foundation;
	using System.Linq;

	public class DialogViewTable : UITableView
	{
		private UIColor oldTextShadowColor = UIColor.Clear;
		private UIColor oldDetailTextShadowColor = UIColor.Clear;

		public DialogViewController Controller { get; set; }

		public DialogViewTable(RectangleF bounds, UITableViewStyle style) : base(bounds, style)
		{
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			ResetTextShadow(false, touches);
			
			base.TouchesBegan(touches, evt);
		}

		public override void TouchesCancelled(NSSet touches, UIEvent evt)
		{
			base.TouchesCancelled(touches, evt);
			
			ResetTextShadow(true, touches);
		}

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			base.TouchesEnded(touches, evt);
			
			if (Controller != null)
			{
				foreach (var section in Controller.Root.Sections)
				{
					foreach (var element in section.Elements)
					{
						var selected = element as IFocusable;
						if (selected != null && selected.ElementView != null && selected.ElementView.IsFirstResponder)
						{
							selected.ElementView.ResignFirstResponder();
							break;
						}
					}
				}
			}
			
			ResetTextShadow(true, touches);
		}
		
		private void ResetTextShadow(bool visible, NSSet touches)
		{
			var touch = touches.AnyObject as UITouch;
			var view = touch.View;
			
			if (view != null)
			{
				var cell = view.Superview as UITableViewElementCell;
				
				if (cell != null && cell.SelectionStyle != UITableViewCellSelectionStyle.None)
				{
					var textLabel = view.Subviews.FirstOrDefault() as UILabel;
					if (textLabel != null)
					{
						if (visible && oldTextShadowColor != null)
						{
							textLabel.ShadowColor = oldTextShadowColor;
						} else
						{
							oldTextShadowColor = textLabel.ShadowColor;
							textLabel.ShadowColor = UIColor.Clear;
						}
					}
					
					var detailTextLabel = view.Subviews.LastOrDefault() as UILabel;
					if (detailTextLabel != null)
					{
						if (visible && oldDetailTextShadowColor != null)
						{
							detailTextLabel.ShadowColor = oldDetailTextShadowColor;
						} else
						{
							oldDetailTextShadowColor = detailTextLabel.ShadowColor;
							detailTextLabel.ShadowColor = UIColor.Clear;
						}
					}
				}
			}
		}
	}
}

