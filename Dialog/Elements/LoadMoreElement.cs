//
// LoadMoreElement.cs
//
// Author:
//   Miguel de Icaza (miguel@gnome.org)
//
// Copyright 2010, Novell, Inc.
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
//
// This cell does not perform cell recycling, do not use as
// sample code for new elements. 
//
namespace MonoTouch.Dialog
{
	using System;
	using System.Drawing;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	public class LoadMoreElement : Element, IElementSizing
	{
		public string NormalCaption { get; set; }
		public string LoadingCaption { get; set; }
		
		private Action<LoadMoreElement> _Tapped = null;
		
		private UITableViewElementCell cell;
		private UIActivityIndicatorView activityIndicator;
		private UILabel caption;
		private UIFont _Font;
		
		public LoadMoreElement (string normalCaption, string loadingCaption, Action<LoadMoreElement> tapped) : this (normalCaption, loadingCaption, tapped, UIFont.BoldSystemFontOfSize (16), UIColor.Black)
		{
		}
		
		public LoadMoreElement (string normalCaption, string loadingCaption, Action<LoadMoreElement> tapped, UIFont font, UIColor textColor) : base ("")
		{
			NormalCaption = normalCaption;
			LoadingCaption = loadingCaption;
			_Tapped = tapped;
			_Font = font;
			
			cell = new UITableViewElementCell(UITableViewCellStyle.Default, Id);
			
			activityIndicator = new UIActivityIndicatorView()
			{
				ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray,
				Hidden = true
			};

			activityIndicator.StopAnimating();
			
			caption = new UILabel()
			{
				Font = font,
				Text = NormalCaption,
				TextColor = textColor,
				BackgroundColor = UIColor.Clear,
				TextAlignment = UITextAlignment.Center,
				AdjustsFontSizeToFitWidth = false,
			};
			
			Layout();
			
			cell.ContentView.AddSubview(caption);
			cell.ContentView.AddSubview(activityIndicator);
		}
		
		public bool Animating
		{
			get
			{
				return activityIndicator.IsAnimating;
			}
			set
			{
				if (value)
				{
					caption.Text = LoadingCaption;
					activityIndicator.Hidden = false;
					activityIndicator.StartAnimating();
				}
				else
				{
					activityIndicator.StopAnimating();
					activityIndicator.Hidden = true;
					caption.Text = NormalCaption;
				}
				Layout ();
			}
		}
				
		public override UITableViewElementCell GetCell(UITableView tv)
		{
			Layout();
			return cell as UITableViewElementCell;
		}
				
		public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			tableView.DeselectRow(path, true);
			
			if (Animating)
				return;
			
			if (_Tapped != null)
			{
				Animating = true;
				Layout ();
				_Tapped(this);
			}
		}
		
		private SizeF GetTextSize()
		{
			return new NSString(caption.Text).StringSize(_Font, UIScreen.MainScreen.Bounds.Width, UILineBreakMode.TailTruncation);
		}
		
		private const int pad = 10;
		private const int isize = 20;
		
		public float GetHeight(UITableView tableView, NSIndexPath indexPath)
		{
			return GetTextSize().Height + 2*pad;
		}
		
		private void Layout()
		{
			var sbounds = cell.ContentView.Bounds;

			var size = GetTextSize();
			
			if (!activityIndicator.Hidden)
				activityIndicator.Frame = new RectangleF((sbounds.Width-size.Width)/2-isize*2, pad, isize, isize);

			caption.Frame = new RectangleF(10, pad, sbounds.Width-20, size.Height);
		}
		
		public UITextAlignment Alignment
		{
			get
			{
				return caption.TextAlignment;
			}
			set
			{
				caption.TextAlignment = value;
			}
		}
		public UITableViewCellAccessory Accessory
		{
			get
			{
				return cell.Accessory;
			}
			set
			{
				cell.Accessory = value;
			}
		}
	}
}

