//
// FocusableView.cs
//
// Author:
//   Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak
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
	using System.Collections.Generic;
	using System.Linq;
	using MonoMobile.Views;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	[Preserve(AllMembers = true)]
	public abstract class FocusableView : CellView, ISelectable, IFocusable, ICellContent
	{
		protected IFocusable _Focus;
		
		protected UIPlaceholderTextField InputView;

		public override UITableViewCellStyle CellStyle { get { return UITableViewCellStyle.Value1; } }
		public UITableViewCell Cell { get; set; }
		public EditMode EditMode { get; set; }

		public bool IsNeedsFirstResponder { get; set; }
		public UIControl Control { get; set; }

		public FocusableView(RectangleF frame) : base(frame)
		{
		}

		public override void UpdateCell(UITableViewCell cell, NSIndexPath indexPath)
		{
			cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			InputView = new UIPlaceholderTextField(cell.Bounds) 
			{ 
				BackgroundColor = UIColor.Clear, 
				Tag = 1, 
				Hidden = true
			};

			if (IsNeedsFirstResponder)
			{
				IsNeedsFirstResponder = false;
				InputView.BecomeFirstResponder();
			}

			cell.ContentView.Add(InputView);
		}
		
		public virtual void Selected(DialogViewController dvc, UITableView tableView, object item, NSIndexPath indexPath)
		{
			if (InputView != null)
			{
				InputView.InvokeOnMainThread(() => 
				{ 
					InputView.BecomeFirstResponder(); 
				});
			}

			tableView.DeselectRow(indexPath, true);
		}

		public void MoveNext()
		{
//			var elements = from section in Controller.Sections 
//				from element in section.Elements
//					where (element is IFocusable && ((IFocusable)element).EditMode != EditMode.ReadOnly)
//					select element as IFocusable; 
//					
//			MoveFocus(elements.ToList());
		}

		public void MovePrev()
		{
//			var elements = (from section in Root.Sections
//				from element in section.Elements
//				where (element is IFocusable && ((IFocusable)element).EditMode != EditMode.ReadOnly)
//				select element as IFocusable).Reverse();
//
//			MoveFocus(elements.ToList());
		}

		private void MoveFocus(IList<IFocusable> elements)
		{
//			_Focus = null;
//			
//			var nextElements = elements.SkipWhile(e => e != this);
//			_Focus = nextElements.Skip(1).FirstOrDefault();
//			if (_Focus == null)
//			{
//				_Focus = elements.FirstOrDefault();
//				TableView.ScrollToRow(_Focus.IndexPath, UITableViewScrollPosition.Top, true);
//			}
//			else
//			{
//				TableView.ScrollToRow(_Focus.IndexPath, UITableViewScrollPosition.Top, true);
//				
//			}
//			
//			_Focus.IsNeedsFirstResponder = true;
//
//			if (_Focus.ElementView != null)
//			{
//				_Focus.ElementView.BecomeFirstResponder();		
//				_Focus.IsNeedsFirstResponder = !_Focus.ElementView.IsFirstResponder;
//			}
		}
	}
}

