//
// ComposableViewListCell.cs:
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
	using System.Collections.Generic;
	using System.Drawing;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

//	public enum CellPosition
//	{
//		Single,
//		Top,
//		Middle,
//		Bottom
//	}
	
	public class ComposableViewListCell : UITableViewCell
	{
		//private const float StandardAccessoryWidth = 30;

		private UIView _CompositeView { get; set; }
		
//		private CellPosition CellPosition 
//		{
//			get
//			{	
//				if (IndexPath == null && Element != null)
//					IndexPath = Element.IndexPath;
//
//				if (IndexPath != null)
//				{
//					var numRows = TableView.NumberOfRowsInSection(IndexPath.Section);
//	
//					if (IndexPath.Row == 0 && IndexPath.Row == numRows - 1)
//					{
//						return CellPosition.Single;
//					}
//					else if (IndexPath.Row == numRows -1)
//					{
//						return CellPosition.Bottom;
//					}
//					else if (IndexPath.Row == 0)
//					{
//						return CellPosition.Top;
//					}
//					else if (IndexPath.Row != numRows - 1)
//					{
//						return CellPosition.Middle;
//					}
//				}
//
//				return CellPosition.Single;
//			}
//		}

		public NSIndexPath IndexPath { get; set; }
		public IList<UIView> ViewList { get; set; }
		public ListSource ListSource { get; set; }
		public Theme Theme { get; set; }

		public ComposableViewListCell(NSCoder coder) : base(coder) {}
		public ComposableViewListCell(IntPtr handle) : base (handle) {}

		public ComposableViewListCell(UITableViewCellStyle style, NSString reuseIdentifier, NSIndexPath indexPath, IList<Type> viewTypes, ListSource listSource) : base(style, reuseIdentifier)
		{
			IndexPath = indexPath;
			ListSource = listSource;
			
			_CompositeView = new UIView(Bounds);

			if (ViewList == null)
				CreateViewList(viewTypes);
			
			Theme = Theme.CreateTheme(listSource.Controller.Theme);

			AddSubview(_CompositeView);
		}

		protected void CreateViewList(IList<Type> viewTypes)
		{
			var frame = new RectangleF(0, 0, ContentView.Bounds.Width, ContentView.Bounds.Height);
			
			if (ViewList != null)
			{
				foreach(var view in ViewList)
				{
					view.Dispose();
				}

				ViewList.Clear();
				ViewList = null;
			}
			
			ViewList = new List<UIView>();
			
			if (viewTypes != null)
			{
				foreach (var viewType in viewTypes)
				{
					UIView view = null;
					var hasFrameCtor = viewType.GetConstructor(new Type[] { typeof(RectangleF) }) != null;
					if (hasFrameCtor)
					{
						view = Activator.CreateInstance(viewType, new object[] { frame }) as UIView;
					}
					else
					{
						view = Activator.CreateInstance(viewType) as UIView;
					}
					
					var dc = view as IDataContext<object>;
					if (dc != null)
					{
						var item = ListSource.GetSectionData(0)[IndexPath.Row];
						dc.DataContext = item;
					}

					var initializeCell = view as IInitializeCell;
					if (initializeCell != null)
					{
//						var newCellStyle = initializeCell.CellStyle;
//						if (newCellStyle != cellStyle)
//						{
//							// recreate cell with new style
//							cell = new UITableViewCell(newCellStyle, cellId) { };
//						}
	
						initializeCell.Cell = this;
						initializeCell.Controller = ListSource.Controller;
					}
					
					var themeable = view as IThemeable;
					if (themeable != null)
					{
						if (themeable.Theme != null)
						{
							themeable.Theme.Cell = this;
						}
								
						themeable.InitializeTheme(this);
					}

					var cellContent = view as ICellContent;
					if (cellContent != null)
					{
						if (cellContent.CellContentView != null)
						{
							_CompositeView.AddSubview(cellContent.CellContentView);
						}
					}
					
					ViewList.Add(view);
				}
			}
		}

		public override void Draw(RectangleF rect)
		{
			if (ListSource != null)
			{
				Theme.Cell = this;
				
				TextLabel.BackgroundColor = UIColor.Clear;
				if (DetailTextLabel != null)
					DetailTextLabel.BackgroundColor = UIColor.Clear;

				if (ListSource.IsRootCell)
				{
					Accessory = UITableViewCellAccessory.DisclosureIndicator;
					TextLabel.Text = ListSource.Caption;
					
					if (ListSource.IsMultiselect && DetailTextLabel != null)
					{
						DetailTextLabel.Text = ListSource.SelectedItems.Count.ToString();
					}
					else
					{
						if (ListSource.SelectedItem != null)
						{
							if (ListSource.ReplaceCaptionWithSelection)
							{
								TextLabel.Text = ListSource.SelectedItem.ToString();
							}
							else if (DetailTextLabel != null)
								{
									DetailTextLabel.Text = ListSource.SelectedItem.ToString();
								}
						}
					}
				}
				else
				{
					var sectionData = ListSource.GetSectionData(0);
					if (sectionData.Count > 0)
					{
						TextLabel.Text = sectionData[IndexPath.Row].ToString();
					}
				}
			}
	
			if (ViewList != null)
			{
				foreach (var view in ViewList)
				{
					var dc = view as IDataContext<object>;
					if (dc != null)
					{
						dc.DataContext = ListSource.GetSectionData(0)[IndexPath.Row];
					}

					var updateable = view as IUpdateable;
					if (updateable != null)
					{
						updateable.UpdateCell(this, IndexPath);
					}
			
					var themeable = view as IThemeable;
					if (themeable != null)
					{			
						themeable.ApplyTheme(this);
					}

					var customDraw = view as ICustomDraw;
					if (customDraw != null)
					{
						customDraw.Draw(rect);
					}
				}
			}
		}
		
//		public RectangleF RecalculateContentFrame(RectangleF frame, bool showCaption)
//		{
//			var screenWidth = UIDevice.CurrentDevice.GetActualWidth();
//			var indentation = UIDevice.CurrentDevice.GetIndentation();
//			var margin = UIDevice.CurrentDevice.GetDeviceMargin();
//			var fixedGap = UIDevice.CurrentDevice.GetFixedGap();
// 
//			var indentedSides = 1;
//			if (Element.TableView.Style == UITableViewStyle.Grouped)
//			{
//				indentedSides = 2;
//			}
//			
//			float x = 0;
//			float height = Bounds.Height;
//
//			var sizeable = Element as ISizeable;
//			if (sizeable != null)
//			{
//				height = sizeable.GetHeight(TableView, Element.IndexPath);
//			}
//			SizeF captionSize = new SizeF(0, height);
//			var caption = TextLabel.Text;
//
//			if (!string.IsNullOrEmpty(caption) && showCaption)
//			{
//				captionSize = TextLabel.StringSize(caption, UIFont.FromName(TextLabel.Font.Name, UIFont.LabelFontSize));
//				captionSize.Width += ((margin * 2) * indentedSides);
//			}
//
//			x = captionSize.Width + fixedGap;
//			float y = 0;
//			float width = screenWidth - captionSize.Width - (indentation * 2) - (margin * 3);
//			float accessoryWidth = AccessoryView != null ? AccessoryView.Bounds.Width : Accessory != UITableViewCellAccessory.None ? StandardAccessoryWidth + (margin * 2) : 0; 
//
//			RectangleF actualFrame;
//			
//			var innerRect = CalculateInnerRect();
//			if (frame == RectangleF.Empty)
//				actualFrame = new RectangleF(-indentedSides, -indentedSides, innerRect.Width + indentedSides, captionSize.Height + (indentedSides * 2));
//			else
//				actualFrame = new RectangleF(x, y, width - accessoryWidth, height - y);
//			 
//			return actualFrame;
//		}
//
//		public override RectangleF Frame
//		{
//			get { return base.Frame; }
//			set {
//				base.Frame = value;
//				
//				if (_ContentView != null)
//				{
//					RectangleF bounds = this.Bounds;
//					_ContentView.Frame = bounds;
//				}
//				
//			}
//		}
//
////		public override void SetNeedsDisplay()
////		{
////			base.SetNeedsDisplay();
////
////			if (_ContentView != null)
////				_ContentView.SetNeedsDisplay();
////		}
//
//		public override void PrepareForReuse()
//		{
//			base.PrepareForReuse();
//		}
//
//		public void DrawContentView()
//		{
//			if (!Highlighted)
//			{
//				var borderRect = Bounds;
//
//				var innerRect = CalculateInnerRect(); 
//
//				CGPath path = null;
//				
//				var backgroundColor = TableView.BackgroundColor;
//
//				CGContext context = UIGraphics.GetCurrentContext();
//				context.SaveState();
//				
//
//				context.SetFillColor(backgroundColor.CGColor);
//	
//				if (TableView.Style == UITableViewStyle.Grouped)
//				{
//					borderRect = CalculateInnerRect();
//					path = GetCellBorderPath(borderRect);
//				}
//				else
//				{
//					path = new CGPath();
//					path.AddRect(borderRect);
//				}
//				
//				context.AddPath(path);
//				context.Clip();
//	
//				ShouldDrawBorder = false;
//				
//				if (Element.Theme.DrawElementViewAction != null)
//				{
//					Element.Theme.DrawElementViewAction(innerRect, context, this);
//				}
//				
//				UIGraphics.BeginImageContext(Bounds.Size);
//				UIImage image = UIGraphics.GetImageFromCurrentImageContext();
//				UIGraphics.EndImageContext();
//
//				BackgroundView = new UIImageView(image);
//
//				context.RestoreState();
//
//				if (ShouldDrawBorder)
//				{
//					DrawBorder(context, path);
//				}
//			}
//		}
//
//		private void DrawBorder(CGContext context, CGPath path)
//		{
//			var separatorColor = Element.Theme.SeparatorColor ?? TableView.SeparatorColor;
//
//		    context.SetLineWidth(1);
//
//			context.SetStrokeColor(separatorColor.CGColor);
//			context.SetShadowWithColor(new SizeF(0,0), 0f, UIColor.Clear.CGColor);
//			context.AddPath(path);
//			context.DrawPath(CGPathDrawingMode.Stroke);
//
//			return;
//		}
//
//		public RectangleF CalculateInnerRect()
//		{
//			var indentationOffset = 0f;
//			var borderOffset = GetBorderOffset();
//			var indentation = UIDevice.CurrentDevice.GetIndentation();
//			var gap = UIDevice.CurrentDevice.GetFixedGap();
//			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
//				gap = 0;
//
//			if (TableView.Style == UITableViewStyle.Grouped)
//				indentationOffset = indentation + gap;
//			
//			var frame = TableView.Frame;
//			var rect = new RectangleF(Bounds.X + indentationOffset - borderOffset, Bounds.Y, frame.Width - ((indentationOffset * 2) + (borderOffset * -1)), Bounds.Height);
//			
//			return rect;
//		}
	//
//		public int GetBorderOffset()
//		{
//			var borderOffset = 1;
//			
//			if (TableView.SeparatorStyle == UITableViewCellSeparatorStyle.DoubleLineEtched)
//				borderOffset = 2;
//			if (TableView.SeparatorStyle == UITableViewCellSeparatorStyle.SingleLineEtched)
//				borderOffset = 1;
//			if (TableView.SeparatorStyle == UITableViewCellSeparatorStyle.SingleLine)
//				borderOffset = 0;
//
//			return borderOffset;
//		}
//
//		public CGPath GetCellBorderPath(RectangleF rect)
//		{
//			var cornerRadius = 10;
//			
//			float minx = rect.GetMinX(), midx = rect.GetMidX(), maxx = rect.GetMaxX();
//			float miny = rect.GetMinY(), midy = rect.GetMidY(), maxy = rect.GetMaxY();
//			
//			CGPath path = new CGPath();
//			
//			var cellPosition = CellPosition;
//
//			if (cellPosition == CellPosition.Top)
//			{
//				minx = minx + 1;
//				miny = miny + 1;
//				
//				maxx = maxx - 1;
//				
//				path.MoveToPoint(minx, maxy);
//				path.AddArcToPoint(minx, miny, midx, miny, cornerRadius);
//				path.AddArcToPoint(maxx, miny, maxx, maxy, cornerRadius);
//				path.AddLineToPoint(maxx, maxy);
//			}
//			else if (cellPosition == CellPosition.Bottom)
//			{
//				minx = minx + 1;
//				
//				maxx = maxx - 1;
//				maxy = maxy - 1;
//				
//				path.MoveToPoint(minx, miny);
//				path.AddArcToPoint(minx, maxy, midx, maxy, cornerRadius);
//				path.AddArcToPoint(maxx, maxy, maxx, miny, cornerRadius);
//				path.AddLineToPoint(maxx, miny);
//			}
//			else if (cellPosition == CellPosition.Middle)
//			{
//				minx = minx + 1;
//				maxx = maxx - 1;
//				
//				path.MoveToPoint(minx, miny);
//				path.AddLineToPoint(maxx, miny);
//				path.AddLineToPoint(maxx, maxy);
//				path.AddLineToPoint(minx, maxy);
//			}
//			else if (cellPosition == CellPosition.Single)
//			{
//				minx = minx + 1;
//				miny = miny + 1;
//				
//				maxx = maxx - 1;
//				maxy = maxy - 1;
//				
//				path.MoveToPoint(minx, midy);
//				path.AddArcToPoint(minx, miny, midx, miny, cornerRadius);
//				path.AddArcToPoint(maxx, miny, maxx, midy, cornerRadius);
//				path.AddArcToPoint(maxx, maxy, midx, maxy, cornerRadius);
//				path.AddArcToPoint(minx, maxy, minx, midy, cornerRadius);
//				
//			}
//
//			path.CloseSubpath();
//			return path;
//		}
	}
}

