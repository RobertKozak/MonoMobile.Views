//
// EntryElement.cs
//
// Author:
//  Miguel de Icaza (miguel@gnome.org)
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
		private UIPlaceholderTextField _InputControl;

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

			DataTemplate = new EntryElementDataTemplate(this);
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
			Theme.CellStyle = UITableViewCellStyle.Default;
			
			return base.NewCell();
		}

		public override void InitializeCell(UITableView tableView)
		{
			Theme.DetailTextAlignment = UITextAlignment.Right;
			
			ShowCaption = EditMode != EditMode.NoCaption;
			if (!ShowCaption)
			{
				Theme.DetailTextAlignment = UITextAlignment.Left;
			}

			base.InitializeCell(tableView);
			
			Cell.SelectionStyle = UITableViewCellSelectionStyle.None;
		}

		public override void InitializeContent()
		{ 
			if (EditMode != EditMode.ReadOnly)
			{
				_InputControl = new UIPlaceholderTextField(new RectangleF(0, 0, Cell.Bounds.Width, Cell.Bounds.Height)) 
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
					_InputControl.Placeholder = Caption;
				else
					_InputControl.Placeholder = Placeholder;
				
				_InputControl.SecureTextEntry = IsPassword;
				_InputControl.Font = Theme.DetailTextFont;
				_InputControl.KeyboardType = KeyboardType;
				_InputControl.TextAlignment = Theme.DetailTextAlignment;
				_InputControl.ReturnKeyType = ReturnKeyType;

				if (Theme.DetailTextColor != null)
					_InputControl.TextColor = Theme.DetailTextColor;
				
				_KeyboardToolbar = new UIKeyboardToolbar(this);
				_InputControl.InputAccessoryView = _KeyboardToolbar;

				_InputControl.ReturnKeyType = UIReturnKeyType.Default;

				_InputControl.Started += (s, e) =>
				{
					//DataContextProperty.ConvertBack<string>();				
				};
			
				_InputControl.ShouldReturn = delegate
				{
					_InputControl.ResignFirstResponder();

					return true;
				};
				
				_InputControl.EditingDidEnd += delegate 
				{
					DataTemplate.UpdateDataContext();
				};

				ContentView = _InputControl;
			}
		}
		
		public override void UpdateCell()
		{
			base.UpdateCell ();
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing && _InputControl != null)
			{
				_InputControl.Dispose();
				_InputControl = null;
			}
			
			base.Dispose(disposing);
		}
	}
}

