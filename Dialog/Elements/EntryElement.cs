//
// EntryElement.cs
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
namespace MonoMobile.MVVM
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Linq;
	using MonoMobile.MVVM;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	public enum EditMode
	{
		ReadOnly,
		NoCaption,
		WithCaption
	}
	
	[Preserve(AllMembers = true)]
	public partial class EntryElement : StringElement, IFocusable, ISearchable, ISelectable
	{
		private UIKeyboardToolbar _KeyboardToolbar;
		private NSTimer _Timer;
		private IFocusable _Focus;

		public UICustomTextField Entry { get; set; }
		
		public EditMode EditMode { get; set; }
		public string Placeholder { get; set; }
		public bool IsPassword { get; set; }
		public string Text { get; set; }
		public UIReturnKeyType ReturnKeyType { get; set; }
		public UIKeyboardType KeyboardType { get; set; }
		public UITextAutocorrectionType AutoCorrectionType { get; set; }
		public UITextAutocapitalizationType AutoCapitalizationType { get; set; }

		public EntryElement(string caption) : base(caption)
		{
			KeyboardType = UIKeyboardType.Default;
			EditMode = EditMode.WithCaption;
		}

		public EntryElement() : this("")
		{
		}

		public EntryElement(RectangleF frame) : this()
		{
			Frame = frame;
		}

		public override UITableViewElementCell NewCell()
		{
			Theme.CellStyle = EditMode == EditMode.ReadOnly ? UITableViewCellStyle.Value1 : UITableViewCellStyle.Default;
			
			return base.NewCell();
		}

		public override void InitializeCell(UITableView tableView)
		{
			ShowCaption = EditMode != EditMode.NoCaption;
			if (!ShowCaption)
			{
				DetailTextAlignment = UITextAlignment.Left;
			}
			
			base.InitializeCell(tableView);
			
			Cell.SelectionStyle = UITableViewCellSelectionStyle.None;
		}

		public override void InitializeContent()
		{ 
			if (EditMode != EditMode.ReadOnly)
			{
				Entry = new UICustomTextField() 
				{ 
					BackgroundColor = UIColor.Clear, 
					PlaceholderColor = Theme.PlaceholderColor, 
					PlaceholderAlignment = Theme.PlaceholderAlignment,
					VerticalAlignment = UIControlContentVerticalAlignment.Center,
					AutocorrectionType = AutoCorrectionType,
					AutocapitalizationType = AutoCapitalizationType,
					Tag = 1 
				};
		
				if (EditMode == EditMode.NoCaption)
					Entry.Placeholder = Caption;
				else
					Entry.Placeholder = Placeholder;

				Entry.SecureTextEntry = IsPassword;
				Entry.Font = DetailTextFont;
				Entry.KeyboardType = KeyboardType;
				Entry.TextAlignment = DetailTextAlignment;
				Entry.ReturnKeyType = ReturnKeyType;

				if (DetailTextColor != null)
					Entry.TextColor = DetailTextColor;
				
				_KeyboardToolbar = new UIKeyboardToolbar(this);
				Entry.InputAccessoryView = _KeyboardToolbar;
				Entry.ReturnKeyType = UIReturnKeyType.Default;

				Entry.Started += (s, e) =>
				{
					ValueProperty.ConvertBack<string>();				
				};
			
				Entry.ShouldReturn = delegate
				{
					Entry.ResignFirstResponder();

					return true;
				};
				
				Entry.EditingDidEnd += delegate 
				{
					ValueProperty.Update();
					Entry.Text = Value;
				};


				ContentView = Entry;
			}
			else
			{
				if (DetailTextLabel != null)
					DetailTextLabel.Text = Value;
			}
		}

		public void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			if (Entry != null)
			{
				Entry.InvokeOnMainThread(()=>{
					Entry.BecomeFirstResponder();
				});
			}
		}

		public void MoveNext()
		{
			var elements = Section.Elements.Where((e)=>e is IFocusable).Cast<IFocusable>().ToList();
			MoveFocus(elements); 
		}
		
		public void MovePrev()
		{
			var elements = Section.Elements.Where((e)=>e is IFocusable).Cast<IFocusable>().ToList();
			elements.Reverse();
			
			MoveFocus(elements);
		}

		private void MoveFocus(IList<IFocusable> elements)
		{
			_Focus = null;
			
			var nextElements = elements.SkipWhile(e=>e != this);
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
			
			_Focus.Entry.BecomeFirstResponder();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && Entry != null)
			{
				Entry.Dispose();
				Entry = null;
			}
			
			base.Dispose(disposing);
		}
	}
}

