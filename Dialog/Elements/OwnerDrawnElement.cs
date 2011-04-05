using System;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.CoreFoundation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;

namespace MonoTouch.Dialog
{
	public abstract class OwnerDrawnElement : Element, ISizeable
	{
		public UITableViewCellStyle Style
		{
			get;
			set;
		}

		public OwnerDrawnElement(UITableViewCellStyle style) : base("")
		{
			Style = style;
		}

		public float GetHeight(UITableView tableView, NSIndexPath indexPath)
		{
			return Height(tableView.Bounds);
		}

		public override UITableViewElementCell NewCell()
		{
			Cell = new OwnerDrawnCell(this, Style, Id);

			return Cell;
		}

		public abstract void Draw(RectangleF bounds, CGContext context, UIView view);

		public abstract float Height(RectangleF bounds);

		private class OwnerDrawnCell : UITableViewElementCell
		{
			private OwnerDrawnCellView _View;

			public OwnerDrawnCell(OwnerDrawnElement element, UITableViewCellStyle style, string cellReuseIdentifier) : base(style, cellReuseIdentifier, element)
			{
				Element = element;
			}

			public new OwnerDrawnElement Element
			{
				get
				{
					return _View.Element;
				}
				set
				{
					if (_View == null)
					{
						_View = new OwnerDrawnCellView(value);
						ContentView.Add(_View);
					} else
					{
						_View.Element = value;
					}
					
					
				}
			}

			public override void LayoutSubviews()
			{
				base.LayoutSubviews();
				
				_View.Frame = ContentView.Bounds;
			}
		}

		class OwnerDrawnCellView : UIView
		{
			private OwnerDrawnElement _Element;

			public OwnerDrawnCellView(OwnerDrawnElement element)
			{
				Element = element;
			}

			public OwnerDrawnElement Element
			{
				get
				{
					return _Element;
				}
				set
				{
					_Element = value;
					if (_Element != null)
						SetNeedsDisplay();
				}
			}

			public override void Draw(RectangleF rect)
			{
				if (_Element == null)
					return;
				
				CGContext context = UIGraphics.GetCurrentContext();
				_Element.Draw(rect, context, this);
			}
		}
	}
}

