//
// UITableViewElementCell.cs: defines the base UITableViewCell ancestor
//
// Author:
//   Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
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
namespace MonoMobile.Views
{
	using System;
	using System.Drawing;
	using MonoMobile.Views;
	using MonoTouch.CoreGraphics;
	using MonoTouch.Foundation;
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
		private const float StandardAccessoryWidth = 30;

		private UITableViewCellContentView _ContentView { get; set; }
		
		private CellPosition CellPosition 
		{
			get
			{	
				if (IndexPath == null && Element != null)
					IndexPath = Element.IndexPath;

				if (IndexPath != null)
				{
					var numRows = TableView.NumberOfRowsInSection(IndexPath.Section);
	
					if (IndexPath.Row == 0 && IndexPath.Row == numRows - 1)
					{
						return CellPosition.Single;
					}
					else if (IndexPath.Row == numRows -1)
					{
						return CellPosition.Bottom;
					}
					else if (IndexPath.Row == 0)
					{
						return CellPosition.Top;
					}
					else if (IndexPath.Row != numRows - 1)
					{
						return CellPosition.Middle;
					}
				}

				return CellPosition.Single;
			}
		}
		public bool ShouldDrawBorder { get; set; }
		public virtual IElement Element { get; set; }

		public UITableView TableView { get { return Element.TableView; } }
		public NSIndexPath IndexPath { get; set; }

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
				if (Element.Theme.DrawElementViewAction != null)
				{
					_ContentView = new UITableViewCellContentView(this);
					InsertSubview(_ContentView, 0);
				}
			}
			
			Opaque = false;
		}

		public RectangleF RecalculateContentFrame(RectangleF frame, bool showCaption)
		{
			var screenWidth = UIDevice.CurrentDevice.GetActualWidth();
			var indentation = UIDevice.CurrentDevice.GetIndentation();
			var margin = UIDevice.CurrentDevice.GetDeviceMargin();
			var fixedGap = UIDevice.CurrentDevice.GetFixedGap();
 
			var indentedSides = 1;
			if (Element.TableView.Style == UITableViewStyle.Grouped)
			{
				indentedSides = 2;
			}
			
			float x = 0;
			float height = Bounds.Height;

			var sizeable = Element as ISizeable;
			if (sizeable != null)
			{
				height = sizeable.GetHeight(TableView, Element.IndexPath);
			}
			SizeF captionSize = new SizeF(0, height);
			var caption = TextLabel.Text;

			if (!string.IsNullOrEmpty(caption) && showCaption)
			{
				captionSize = TextLabel.StringSize(caption, UIFont.FromName(TextLabel.Font.Name, UIFont.LabelFontSize));
				captionSize.Width += ((margin * 2) * indentedSides);
			}
			
			x = captionSize.Width + fixedGap;
			float y = 0;
			float width = screenWidth - captionSize.Width - (indentation * 2) - (margin * 3);
			float accessoryWidth = AccessoryView != null ? AccessoryView.Bounds.Width : Accessory != UITableViewCellAccessory.None ? StandardAccessoryWidth + (margin * 2) : 0; 

			RectangleF actualFrame;
			
			if (frame == RectangleF.Empty)
				actualFrame = new RectangleF(-1, -1, width + indentation + 1, captionSize.Height + 1);
			else
				actualFrame = new RectangleF(x, y, width - accessoryWidth, height - y);
			 
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
			if (!Highlighted)
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
				
				if (Element.Theme.DrawElementViewAction != null)
					Element.Theme.DrawElementViewAction(innerRect, context, this);

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

		public RectangleF CalculateInnerRect()
		{
			var indentationOffset = 0f;
			var borderOffset = 1;
			var indentation = UIDevice.CurrentDevice.GetIndentation();
			var gap = UIDevice.CurrentDevice.GetFixedGap();
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
				gap = 0;

			if (TableView.Style == UITableViewStyle.Grouped)
				indentationOffset = indentation + gap;
			
//			if (TableView.SeparatorStyle == UITableViewCellSeparatorStyle.DoubleLineEtched)
//				borderOffset = 2;
//			if (TableView.SeparatorStyle == UITableViewCellSeparatorStyle.SingleLineEtched)
//				borderOffset = 1;
//			if (TableView.SeparatorStyle == UITableViewCellSeparatorStyle.SingleLine)
//				borderOffset = 0;

			var rect = new RectangleF(Bounds.X + indentationOffset - borderOffset, Bounds.Y, Bounds.Width - (indentationOffset * 2) + (borderOffset * 2), Bounds.Height);
			
			return rect;
		}
		
		public CGPath GetCellBorderPath(RectangleF rect)
		{
			var cornerRadius = 10;
			
			float minx = rect.GetMinX(), midx = rect.GetMidX(), maxx = rect.GetMaxX();
			float miny = rect.GetMinY(), midy = rect.GetMidY(), maxy = rect.GetMaxY();
			
			CGPath path = new CGPath();
			
			var cellPosition = CellPosition;

			if (cellPosition == CellPosition.Top)
			{
				minx = minx + 1;
				miny = miny + 1;
				
				maxx = maxx - 1;
				
				path.MoveToPoint(minx, maxy);
				path.AddArcToPoint(minx, miny, midx, miny, cornerRadius);
				path.AddArcToPoint(maxx, miny, maxx, maxy, cornerRadius);
				path.AddLineToPoint(maxx, maxy);
			}
			else if (cellPosition == CellPosition.Bottom)
			{
				minx = minx + 1;
				
				maxx = maxx - 1;
				maxy = maxy - 1;
				
				path.MoveToPoint(minx, miny);
				path.AddArcToPoint(minx, maxy, midx, maxy, cornerRadius);
				path.AddArcToPoint(maxx, maxy, maxx, miny, cornerRadius);
				path.AddLineToPoint(maxx, miny);
			}
			else if (cellPosition == CellPosition.Middle)
			{
				minx = minx + 1;
				maxx = maxx - 1;
				
				path.MoveToPoint(minx, miny);
				path.AddLineToPoint(maxx, miny);
				path.AddLineToPoint(maxx, maxy);
				path.AddLineToPoint(minx, maxy);
			}
			else if (cellPosition == CellPosition.Single)
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
	}
	
	public class UITableViewCellContentView : UIView
	{
		protected UITableViewElementCell Cell { get; set; }

		public UITableViewCellContentView(NSCoder coder) : base(coder)
		{
		}
		public UITableViewCellContentView(NSObjectFlag t) : base(t)
		{
		}
		public UITableViewCellContentView(IntPtr handle) : base(handle)
		{
		}
		public UITableViewCellContentView(RectangleF frame) : base(frame)
		{
		}

		public UITableViewCellContentView(UITableViewElementCell cell) : base(cell.Bounds)
		{
			Cell = cell;

			Opaque = false;
		}

		public override void Draw(RectangleF rect)
		{	
			if (Cell != null)
			{
				Cell.DrawContentView();
			}
		}
	}

	public class DisabledCellView : UIView
	{
		private UITableViewElementCell _Cell; 
 
		public DisabledCellView(NSCoder coder) : base(coder)
		{
		}
		public DisabledCellView(NSObjectFlag t) : base(t)
		{
		}
		public DisabledCellView(IntPtr handle) : base(handle)
		{
		}
		public DisabledCellView(RectangleF frame) : base(frame)
		{
		}

		public DisabledCellView(UITableViewElementCell cell) : base(cell.Bounds)
		{		
			BackgroundColor = UIColor.Clear;
			Opaque = false;

			if(cell != null)
			{
				_Cell = cell;
				Bounds = _Cell.CalculateInnerRect();
			}
		}

		public override void Draw(RectangleF rect)
		{
			if (_Cell != null)
			{
				CGContext context = UIGraphics.GetCurrentContext();
				
				CGPath path;
				var borderRect = rect;

				if (_Cell.TableView.Style == UITableViewStyle.Grouped)
				{
					borderRect = _Cell.CalculateInnerRect();
					path = _Cell.GetCellBorderPath(borderRect);
				} 
				else
				{
					path = new CGPath();
					path.AddRect(borderRect);
				}
				
				context.AddPath(path);
				_Cell.Element.Theme.DisabledColor.SetFill();
				context.DrawPath(CGPathDrawingMode.Fill);
			}
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{	
		}
		public override void TouchesMoved(NSSet touches, UIEvent evt)
		{
		}
		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
		}
	}
}

