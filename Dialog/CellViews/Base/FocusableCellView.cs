//
// FocusableCellView.cs
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
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	[Preserve(AllMembers = true)]
	public abstract class FocusableCellView : CellView<string>, IFocusable, ICellContent
	{
		protected IFocusable _Focus;
		
		protected new UIPlaceholderTextField InputView;

		public UIView CellContentView { get; set; }
		public UIView CellBackgroundView { get; set; }
		public UIView CellSelectedBackgroundView { get; set; }

		public override UITableViewCellStyle CellStyle { get { return UITableViewCellStyle.Value1; } }
		public EditMode EditModeValue { get; set; }

		public bool IsNeedsFirstResponder { get; set; }
		public UIControl Control { get; set; }

		public NSIndexPath IndexPath { get; set; }

		public FocusableCellView(RectangleF frame) : base(frame)
		{
			InputView = new UIPlaceholderTextField(frame) 
			{ 	
				BackgroundColor = UIColor.Clear, 
				Tag = 1, 
			};
			
			InputView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
			
			CellContentView = InputView;
			Add(CellContentView);
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing && InputView != null)
			{
				InputView = null;
			}

			base.Dispose(disposing);
		}

		public override void UpdateCell(UITableViewCell cell, NSIndexPath indexPath)
		{
			IndexPath = indexPath;
			InputView.IndexPath = indexPath;
			
//			if (IsNeedsFirstResponder)
//			{
//				IsNeedsFirstResponder = false;
//				InputView.BecomeFirstResponder();
//			}

			cell.SelectionStyle = UITableViewCellSelectionStyle.None;
		}
		
		public override bool BecomeFirstResponder()
		{
			return InputView.BecomeFirstResponder();
		}

		public void DismissKeyboard()
		{
			var views = GetFocusableList();
			foreach(var view in views)
			{
				view.IsNeedsFirstResponder = false;
			}

			InputView.ResignFirstResponder();
		}

		public void MoveNext()
		{
			var views = GetFocusableList();	
			
			MoveFocus(views);
		}

		public void MovePrev()
		{
			var views = GetFocusableList();
			views.Reverse();

			MoveFocus(views);
		}
		
		private IList<IFocusable> GetFocusableList()
		{
			var sections = ((BaseDialogViewSource)Controller.TableView.Source).Sections;
			
			var views = new List<IFocusable>();
			foreach (var section in sections.Values)
			{
				foreach (var viewList in section.Views.Values)
				{
					foreach (var view in viewList)
					{
						var focusable = view as IFocusable;
						if (focusable != null && focusable.EditModeValue != EditMode.ReadOnly && focusable != this)
						{
							views.Add(focusable);
						}
					}
				}
			}

			return views;
		}

		private void MoveFocus(IList<IFocusable> focusables)
		{
			_Focus = null;
			
			_Focus = focusables.Skip(1).FirstOrDefault();

			if (_Focus == null)
			{
				_Focus = focusables.FirstOrDefault();
			}
		
			if (_Focus != null)
			{
				var indexPath = _Focus.IndexPath;
				Controller.TableView.ScrollToRow(indexPath, UITableViewScrollPosition.Top, true);

				_Focus.IsNeedsFirstResponder = true;
				
				_Focus.BecomeFirstResponder();	

				if (_Focus.InputView != null)
				{
					_Focus.IsNeedsFirstResponder = !_Focus.InputView.IsFirstResponder;
				}
			}
		}
	}
}

