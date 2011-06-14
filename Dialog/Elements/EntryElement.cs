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
	public partial class EntryElement : FocusableElement, IFocusable, ISearchable, ISelectable
	{
		private UIKeyboardToolbar _KeyboardToolbar;
 
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
				InputControl = new UICustomTextField(new RectangleF(0, 0, Cell.Bounds.Width, Cell.Bounds.Height)) 
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
					InputControl.Placeholder = Caption;
				else
					InputControl.Placeholder = Placeholder;

				InputControl.SecureTextEntry = IsPassword;
				InputControl.Font = DetailTextFont;
				InputControl.KeyboardType = KeyboardType;
				InputControl.TextAlignment = DetailTextAlignment;
				InputControl.ReturnKeyType = ReturnKeyType;

				if (DetailTextColor != null)
					InputControl.TextColor = DetailTextColor;
				
				_KeyboardToolbar = new UIKeyboardToolbar(this);
				InputControl.InputAccessoryView = _KeyboardToolbar;

				InputControl.ReturnKeyType = UIReturnKeyType.Default;

				InputControl.Started += (s, e) =>
				{
					//DataContextProperty.ConvertBack<string>();				
				};
			
				InputControl.ShouldReturn = delegate
				{
					InputControl.ResignFirstResponder();

					return true;
				};
				
				InputControl.EditingDidEnd += delegate 
				{
					DataContextProperty.Update();
				};


				ContentView = InputControl;
			}
			else
			{
				if (DetailTextLabel != null)
					DetailTextLabel.Text = (string)DataContext;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && InputControl != null)
			{
				InputControl.Dispose();
				InputControl = null;
			}
			
			base.Dispose(disposing);
		}
	}
}

