////
//// MultilineElement.cs
////
//// Author:
////  Miguel de Icaza (miguel@gnome.org)
////
//// Copyright 2010, Novell, Inc.
////
//// Code licensed under the MIT X11 license
////
//// Permission is hereby granted, free of charge, to any person obtaining
//// a copy of this software and associated documentation files (the
//// "Software"), to deal in the Software without restriction, including
//// without limitation the rights to use, copy, modify, merge, publish,
//// distribute, sublicense, and/or sell copies of the Software, and to
//// permit persons to whom the Software is furnished to do so, subject to
//// the following conditions:
////
//// The above copyright notice and this permission notice shall be
//// included in all copies or substantial portions of the Software.
////
//// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////
//namespace MonoMobile.Views
//{
//	using System;
//	using System.Drawing;
//	using MonoTouch.Foundation;
//	using MonoTouch.UIKit;
//	
//	[Preserve(AllMembers = true)]
//	public class MultilineElement : FocusableElement
//	{
//		private UIKeyboardToolbar _KeyboardToolbar;
//		private UICustomTextView _InputControl;
//
//		public int Lines { get; set; }
//
//		public UIKeyboardType KeyboardType { get; set; }
//		public UITextAutocorrectionType AutoCorrectionType { get; set; }
//		public UITextAutocapitalizationType AutoCapitalizationType { get; set; }
//		public UIReturnKeyType ReturnKeyType { get; set; }
//
//		public MultilineElement() : this("")
//		{
//		}
//		
//		public MultilineElement(string caption) : base(caption)
//		{
//			KeyboardType = UIKeyboardType.Default;
//			EditMode = EditMode.WithCaption;
//			AutoCapitalizationType = UITextAutocapitalizationType.Sentences;
//			AutoCorrectionType = UITextAutocorrectionType.Yes;
//			Lines = 0;
//
//			DataBinding = new MultilineElementDataBinding(this);
//		}
//
////		public MultilineElement(RectangleF frame) : this()
////		{
////			Frame = frame;
////		}
//		
//		public override UITableViewElementCell NewCell(NSString cellId, NSIndexPath indexPath)
//		{
//			return base.NewCell(cellId, indexPath);
//		}
//
//		public override void InitializeCell(UITableView tableView)
//		{
//			Theme.DetailTextAlignment = UITextAlignment.Left;
//
//			base.InitializeCell(tableView);
//
//			ShowCaption = ShowCaption && EditMode != EditMode.NoCaption;
//
//			Cell.SelectionStyle = UITableViewCellSelectionStyle.None;
//			Cell.Accessory = UITableViewCellAccessory.None;
//		}
//		
//		public override void InitializeContent()
//		{ 
//			if (EditMode != EditMode.ReadOnly)
//			{
//				_InputControl = new UICustomTextView(new RectangleF(0, 0, Cell.Bounds.Width, Cell.Bounds.Height)) 
//				{ 
//					BackgroundColor = UIColor.Clear, 
//					AutocorrectionType = AutoCorrectionType,
//					AutocapitalizationType = AutoCapitalizationType,
//					AlwaysBounceVertical = false,
//					AlwaysBounceHorizontal = false,
//					ScrollEnabled = true,
//					Font = UIFont.SystemFontOfSize(UIFont.LabelFontSize),
//					TextColor = UIColor.DarkTextColor,
//					Tag = 1,	
//				};
//				
//				_InputControl.AutoScroll = Lines > 0;
//
//				if (Theme.DetailTextFont != null)
//					_InputControl.Font = Theme.DetailTextFont;
//	
//				if (Theme.DetailTextColor != null)
//					_InputControl.TextColor = Theme.DetailTextColor;
//
//				_InputControl.KeyboardType = KeyboardType;
//				_InputControl.TextAlignment = Theme.DetailTextAlignment;
//				_InputControl.ReturnKeyType = ReturnKeyType;
//				
//				_KeyboardToolbar = new UIKeyboardToolbar(this);
//				 _InputControl.InputAccessoryView = _KeyboardToolbar;
//
//				_InputControl.Started += (s, e) =>
//				{
//					//DataContextProperty.ConvertBack<string>();	
//				};
//				
//				_InputControl.Changed += InputControlTextChanged;
//
//				_InputControl.Ended += delegate 
//				{ 			
//					DataBinding.UpdateDataContext();
//
//					_InputControl.ContentOffset = new PointF(0, 8);
//
//				};
//
//				ElementView = _InputControl;
//			}
//		}
//
//		public override void UpdateCell()
//		{
//			base.UpdateCell();	
//			
//			new Wait(TimeSpan.FromMilliseconds(300), ()=> 
//			{
//				InputControlTextChanged(this, EventArgs.Empty);
//				_InputControl.ContentOffset = new PointF(0, 8);
//			});
//		}
//		
//		protected override void Dispose(bool disposing)
//		{
//			if (disposing && ElementView != null)
//			{
//				ElementView.Dispose();
//				ElementView = null;
//			}
//			
//			base.Dispose(disposing);
//		}
//
//		public float GetWidth()
//		{
//			var screenWidth = UIDevice.CurrentDevice.GetActualWidth();
//			var indentation = UIDevice.CurrentDevice.GetIndentation();
//			var margin = UIDevice.CurrentDevice.GetDeviceMargin();
//			var fixedGap = UIDevice.CurrentDevice.GetFixedGap();
// 
//			var indentedSides = 1;
//			if (TableView != null && TableView.Style == UITableViewStyle.Grouped)
//			{
//				indentedSides = 2;
//			}
//			
//			float elementHeight = 0;
//
//			if (ElementView != null)
//				elementHeight = ElementView.Bounds.Height;
//
//			SizeF captionSize = new SizeF(-1, elementHeight);
//			if (!string.IsNullOrEmpty(Caption) && ShowCaption)
//			{
//				if (TextLabel != null)
//					captionSize = TextLabel.StringSize(Caption, TextLabel.Font);
//				captionSize.Width += ((margin * 2) * indentedSides);
//			}
//			
//			float width = screenWidth - captionSize.Width - (indentation * 2) - (margin * 3) - fixedGap;
//			
//			return width;
//		}
//		
//		public override float GetHeight(UITableView tableView, NSIndexPath indexPath)
//		{
//			var newHeight = GetHeight(tableView);
//			
//			return Math.Max((float)Math.Truncate(newHeight), tableView.RowHeight);
//		}
//
//		private float GetHeight(UITableView tableView)
//		{
//			var indentation = UIDevice.CurrentDevice.GetIndentation();
//			var margin = UIDevice.CurrentDevice.GetDeviceMargin();
//
//			var size = new SizeF(GetWidth(), float.MaxValue);
//			if (ElementView != null)
//				size.Width = ElementView.Frame.Width;
//
//			float newHeight = 0;
//
//			if (_InputControl != null && !string.IsNullOrEmpty(_InputControl.Text))
//			{
//				if (Lines == 0)
//				{
//					newHeight = (tableView.StringSize(_InputControl.Text, _InputControl.Font, size, UILineBreakMode.WordWrap).Height);
//			
//					newHeight += (indentation * 2) + (_InputControl.Font.LineHeight * 0.80f) + margin;
//				}
//				else
//				{
//					newHeight = (Lines * _InputControl.Font.LineHeight) + (indentation * 2) + margin;
//				}
//			}	
//			else
//				newHeight = (Lines * 21f) + (indentation * 2) + margin;
//
//			return newHeight;
//		}
//		
//		private void InputControlTextChanged(object sender, EventArgs e)
//		{
//			NSRange range = new NSRange(_InputControl.Text.Length - 1, 1);
//			_InputControl.ScrollRangeToVisible(range);
//
//			var size = new SizeF(GetWidth(), float.MaxValue);
//			
//			var height = GetHeight(TableView);
//
//			var frame = Cell.RecalculateContentFrame(new RectangleF(0, 0, size.Width, height), ShowCaption);
//			
//			if (height > frame.Height)
//			{
//				_InputControl.Frame = frame;
//
//				TableView.BeginUpdates();
//				TableView.EndUpdates();
//			}
//		}
//	}
//
//	public class UICustomTextView: UITextView
//	{
//		public bool AutoScroll { get; set; }
//
//		public UICustomTextView(IntPtr handle) : base(handle)
//		{
//		}
//
//		public UICustomTextView(RectangleF frame): base(frame)
//		{
//		}
//
//		public override UIEdgeInsets ContentInset {
//			get 
//			{ 
//				return base.ContentInset; 
//			}
//			set 
//			{
//				if (value.Bottom > 8)
//				{
//					value.Bottom = 0;
//					value.Top = -8;
//				}
//				base.ContentInset = value;
//			}
//		}
//		
//		public override void ScrollRectToVisible(RectangleF rect, bool animated)
//		{
//			if (AutoScroll)
//				base.ScrollRectToVisible(rect, animated);
//		}
//		
//		public override void SetContentOffset(PointF contentOffset, bool animated)
//		{
//			if (AutoScroll)
//			{
//				if (Tracking || Decelerating)
//				{
//					//initiated by user...
//					contentOffset = PointF.Empty;
//				} 
//				else
//				{
//					
//					float bottomOffset = (ContentSize.Height - Frame.Size.Height + ContentInset.Bottom);
//					if (contentOffset.Y < bottomOffset && ScrollEnabled)
//					{
//						//contentOffset = PointF.Empty;
//						//maybe use scrollRangeToVisible?
//					}
//					
//				}
//
//				base.SetContentOffset(contentOffset, false);
//			}
//		}
//	}
//}
//
