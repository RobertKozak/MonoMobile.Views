//
// FocusableElement.cs
//
// Author:
//   Robert Kozak (rkozak@gmail.com) Twitter:@robertkozak
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
namespace MonoMobile.MVVM
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using MonoMobile.MVVM;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	public abstract class FocusableElement : StringElement, ISelectable, IFocusable
	{
		protected IFocusable _Focus;
		protected NSTimer _Timer;
		protected UICustomTextField _Dummy { get; set; }

		public UICustomTextField InputControl { get; set; }
		public UIControl Control { get; set; }

		public FocusableElement(string caption) : base(caption)
		{
		}

		public override UITableViewElementCell NewCell()
		{
			return new UITableViewElementCell(UITableViewCellStyle.Default, Id, this);
		}

		public override void InitializeContent()
		{
			_Dummy = new UICustomTextField(Bounds);
			_Dummy.ShouldBeginEditing = tf =>
			{
				InputControl.BecomeFirstResponder();
				return false;
			};
			
			InputControl = new UICustomTextField(Bounds) { BackgroundColor = UIColor.Clear, Tag = 1, Hidden = true };
			
			ContentView = _Dummy;
			ContentView.AddSubview(InputControl);
		}

		public void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			InputControl.BecomeFirstResponder();
		}

		public void MoveNext()
		{
			var elements = from section in Root.Sections 
				from element in section.Elements 
					where (element is IFocusable)
					select element as IFocusable; 
					
			MoveFocus(elements.ToList());
		}

		public void MovePrev()
		{
			var elements = (from section in Root.Sections
				from element in section.Elements
				where (element is IFocusable)
				select element as IFocusable).Reverse();

			MoveFocus(elements.ToList());
		}

		private void MoveFocus(IList<IFocusable> elements)
		{
			_Focus = null;
			
			var nextElements = elements.SkipWhile(e => e != this);
			_Focus = nextElements.Skip(1).FirstOrDefault();
			if (_Focus == null)
				_Focus = elements.FirstOrDefault();
			
			_Timer = NSTimer.CreateScheduledTimer(TimeSpan.FromMilliseconds(300), FocusTimer);
			
			TableView.ScrollToRow(_Focus.IndexPath, UITableViewScrollPosition.Top, true);
		}

		private void FocusTimer()
		{
			_Timer.Invalidate();
			_Timer = null;
			
			_Focus.InputControl.BecomeFirstResponder();
		}
	}
}

