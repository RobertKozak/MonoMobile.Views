//
// UITableViewElementCell.cs: defines the base UITableViewCell ancestor
//
// Author:
//   Robert Kozak (rkozak@nowcom.com)
//
// Copyright 2011, Nowcom Corporation
//
// Code licensed under the MIT X11 license
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
namespace MonoTouch.Dialog
{
	using System;
	using System.Drawing;
	using System.Reflection;
	using MonoTouch.CoreGraphics;
	using MonoTouch.Foundation;
	using MonoMobile.MVVM;
	using MonoTouch.UIKit;

	public enum CellPosition
	{
		Single,
		Top,
		Middle,
		Bottom
	}
	
	public class UITableViewElementCell : UITableViewCell
	{
		private UITableViewCellContentView _ContentView { get; set; }
		
		private CellPosition CellPosition 
		{
			get
			{
				var indexPath = Element.IndexPath;
				var numRows = TableView.NumberOfRowsInSection(indexPath.Section);

				if ( indexPath.Row == 0 && indexPath.Row == numRows - 1)
				{
					return CellPosition.Single;
				}
				else if (indexPath.Row == numRows -1)
				{
					return CellPosition.Bottom;
				}
				else if (indexPath.Row == 0)
				{
					return CellPosition.Top;
				}
				else if (indexPath.Row != numRows - 1)
				{
					return CellPosition.Middle;
				}

				return CellPosition.Single;
			}
		}
		public bool ShouldDrawBorder { get; set; }
		public virtual IElement Element { get; set; }

		public UITableView TableView { get { return Superview as UITableView; } }

		public UITableViewElementCell() :base() {}
		public UITableViewElementCell(NSCoder coder) : base(coder) {}
		public UITableViewElementCell(NSObjectFlag t) : base(t) {}
		public UITableViewElementCell(IntPtr handle) : base (handle) {}

		public UITableViewElementCell(UITableViewCellStyle style, string reuseIdentifier, IElement element) : this(style, new NSString(reuseIdentifier), element)
		{
		}

		public UITableViewElementCell(UITableViewCellStyle style, NSString reuseIdentifier, IElement element) : base(style, reuseIdentifier)
		{
			Element = element;
			if (Element != null && Element.Theme != null)
			{
			//	var type = Element.Theme.GetType();
			//	if (type.IsSubclassOf(typeof(Theme)))
			//	{
					//var drawMethodDefined = type.GetMethod("DrawContentView", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance) != null;

					if (Element.Theme.DrawContentViewAction != null)
					{
						_ContentView = new UITableViewCellContentView(this);
						InsertSubview(_ContentView, 0);
					}
			//	}
			}
			
			Opaque = false;
		}

		public RectangleF RecalculateContentFrame(RectangleF frame, bool showCaption)
		{
			var indentedSides = 1;
			if (Element.TableView.Style == UITableViewStyle.Grouped)
			{
				indentedSides = 2;
			}
			
			SizeF captionSize = new SizeF((IndentationWidth + (IndentationWidth / 2)), Bounds.Height);
			var caption = TextLabel.Text;
			if (!string.IsNullOrEmpty(caption) && showCaption)
			{
				captionSize = TextLabel.StringSize(caption, UIFont.FromName(TextLabel.Font.Name, UIFont.LabelFontSize));
				captionSize.Width += (IndentationWidth * indentedSides);
			}
			
			float x = captionSize.Width - (IndentationWidth / 2);
			float y = (float)Math.Round((double)Bounds.Height - (double)captionSize.Height) / 2;
			float width = Bounds.Width - captionSize.Width - (IndentationWidth * ((indentedSides * 2) - 1)) + (IndentationWidth / 2);
			
			RectangleF actualFrame;
			
			if (frame == RectangleF.Empty)
				actualFrame = new RectangleF(x, y, width, captionSize.Height);
			else
				actualFrame = new RectangleF(x, y, frame.Width, frame.Height);
			
			return actualFrame;
		}

		public override RectangleF Frame
		{
			get { return base.Frame; }
			set {
				base.Frame = value;
				
				if (_ContentView != null)
				{
					RectangleF bounds = this.Bounds;
					_ContentView.Frame = bounds;
				}
				
			}
		}

		public override void SetNeedsDisplay()
		{
			base.SetNeedsDisplay();

			if (_ContentView != null)
				_ContentView.SetNeedsDisplay();
		}

		public override void PrepareForReuse()
		{
			base.PrepareForReuse();
			//Element.Dispose();
			Element = null;
		}
		
		public void DrawContentView()
		{
			if (!Highlighted || Element.Theme.DrawWhenHighlighted)
			{
				var borderRect = Bounds;

				var innerRect = CalculateInnerRect(); 

				CGPath path = null;
				
				var backgroundColor = TableView.BackgroundColor;

				CGContext context = UIGraphics.GetCurrentContext();
				context.SaveState();
				
				context.SetFillColorWithColor(backgroundColor.CGColor);
	
				if (TableView.Style == UITableViewStyle.Grouped)
				{
					borderRect = CalculateInnerRect();
					//borderRect = new RectangleF(Bounds.X + IndentationWidth - 1, Bounds.Y - 1, (Bounds.Width - IndentationWidth * 2) + 2, Bounds.Height + 1);
					path = GetCellBorderPath(borderRect);
				}
				else
				{
					path = new CGPath();
					path.AddRect(borderRect);
				}
				
				context.AddPath(path);
				context.Clip();
	
				ShouldDrawBorder = false;
				if (Element.Theme.DrawContentViewAction != null)
					Element.Theme.DrawContentViewAction(innerRect, context, this);
	
				context.RestoreState();
	
				if (ShouldDrawBorder)
				{
					DrawBorder(context, path);
				}
			}
		}

		private void DrawBorder(CGContext context, CGPath path)
		{
			var separatorColor = Element.Theme.SeparatorColor ?? TableView.SeparatorColor;

		    context.SetLineWidth(1);

			context.SetStrokeColorWithColor(separatorColor.CGColor);
			context.SetShadowWithColor(new SizeF(0,0), 0f, UIColor.Clear.CGColor);
			context.AddPath(path);
			context.DrawPath(CGPathDrawingMode.Stroke);

			return;
		}

		private RectangleF CalculateInnerRect()
		{
			var indentationOffset = 0f;
			var borderOffset = 1;

			if (TableView.Style == UITableViewStyle.Grouped)
				indentationOffset = IndentationWidth;
			
//			if (TableView.SeparatorStyle == UITableViewCellSeparatorStyle.DoubleLineEtched)
//				borderOffset = 2;
//			if (TableView.SeparatorStyle == UITableViewCellSeparatorStyle.SingleLineEtched)
//				borderOffset = 1;
//			if (TableView.SeparatorStyle == UITableViewCellSeparatorStyle.SingleLine)
//				borderOffset = 0;

			var rect = new RectangleF(Bounds.X + indentationOffset - borderOffset, Bounds.Y, Bounds.Width - (indentationOffset * 2) + (borderOffset * 2), Bounds.Height);
			
			return rect;
		}
		
		private CGPath GetCellBorderPath(RectangleF rect)
		{
			var cornerRadius = 10;
			
			float minx = rect.GetMinX(), midx = rect.GetMidX(), maxx = rect.GetMaxX();
			float miny = rect.GetMinY(), midy = rect.GetMidY(), maxy = rect.GetMaxY();
			
			CGPath path = new CGPath();
			
			if (CellPosition == CellPosition.Top)
			{
				minx = minx + 1;
				miny = miny + 1;
				
				maxx = maxx - 1;
				
				path.MoveToPoint(minx, maxy);
				path.AddArcToPoint(minx, miny, midx, miny, cornerRadius);
				path.AddArcToPoint(maxx, miny, maxx, maxy, cornerRadius);
				path.AddLineToPoint(maxx, maxy);
			}
			else if (CellPosition == CellPosition.Bottom)
			{
				minx = minx + 1;
				
				maxx = maxx - 1;
				maxy = maxy - 1;
				
				path.MoveToPoint(minx, miny);
				path.AddArcToPoint(minx, maxy, midx, maxy, cornerRadius);
				path.AddArcToPoint(maxx, maxy, maxx, miny, cornerRadius);
				path.AddLineToPoint(maxx, miny);
			}
			else if (CellPosition == CellPosition.Middle)
			{
				minx = minx + 1;
				maxx = maxx - 1;
				
				path.MoveToPoint(minx, miny);
				path.AddLineToPoint(maxx, miny);
				path.AddLineToPoint(maxx, maxy);
				path.AddLineToPoint(minx, maxy);
			}
			else if (CellPosition == CellPosition.Single)
			{
				minx = minx + 1;
				miny = miny + 1;
				
				maxx = maxx - 1;
				maxy = maxy - 1;
				
				path.MoveToPoint(minx, midy);
				path.AddArcToPoint(minx, miny, midx, miny, cornerRadius);
				path.AddArcToPoint(maxx, miny, maxx, midy, cornerRadius);
				path.AddArcToPoint(maxx, maxy, midx, maxy, cornerRadius);
				path.AddArcToPoint(minx, maxy, minx, midy, cornerRadius);
				
			}

			path.CloseSubpath();
			return path;
		}

		class UITableViewCellContentView : UIView
		{
			private UITableViewElementCell _Cell { get; set; }

			public UITableViewCellContentView(NSCoder coder) : base(coder) {}
			public UITableViewCellContentView(NSObjectFlag t) : base(t) {}
			public UITableViewCellContentView(IntPtr handle) : base(handle) {}
			public UITableViewCellContentView(RectangleF frame) : base(frame) {}

			public UITableViewCellContentView(UITableViewElementCell cell) : base(RectangleF.Empty)
			{
				_Cell = cell;
				Opaque = false;
			}

			public override void Draw(RectangleF rect)
			{
				if (_Cell != null )
				{
					_Cell.DrawContentView();
				}
			}
		}
	}
}

