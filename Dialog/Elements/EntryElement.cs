using System.Reflection;
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
namespace MonoTouch.Dialog
{
	using System;
	using System.Drawing;
	using MonoTouch.Foundation;
	using MonoMobile.MVVM;
	using MonoTouch.UIKit;

	public enum EditMode
	{
		ReadOnly,
		NoCaption,
		WithCaption
	}
	
	public class EntryElement : StringElement, IFocusable, ISearchable, ISelectable
	{
		public UITextField Entry { get; set; }
		
		public EditMode EditMode { get; set; }
		public string Placeholder { get; set; }
		public bool IsPassword { get; set; }
		public string Text { get; set; }
		public UIReturnKeyType ReturnKeyType { get; set; }
		public UIKeyboardType KeyboardType { get; set; }
		public UITextAutocorrectionType AutoCorrectionType { get; set; }
		public UITextAutocapitalizationType AutoCapitalizationType { get; set; }
		
		public BindableProperty PlaceholderProperty = BindableProperty.Register("Placeholder");
		public BindableProperty IsPasswordProperty = BindableProperty.Register("IsPassword");
		public BindableProperty TextProperty = BindableProperty.Register("Text");
		public BindableProperty ReturnKeyTypeProperty = BindableProperty.Register("ReturnKeyboardType");
		public BindableProperty KeyboardTypeProperty = BindableProperty.Register("KeyboardType");
		public BindableProperty EditModeProperty = BindableProperty.Register("EditMode");

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

				Entry.Started += (s, e) =>
				{
					ValueProperty.ConvertBack<string>();

					IFocusable self = null;
					var returnType = UIReturnKeyType.Done;
					
					foreach (IElement element in (Parent as ISection).Elements) 
					{
						if (element == null)
							continue;
	
						if (element == this)
							self = this; 
						else if (self != null && element is IFocusable)
						{
							returnType = UIReturnKeyType.Next;
							break;
						}
					}
					
					Entry.ReturnKeyType = returnType;
				};
			
				Entry.ShouldReturn = delegate
				{
					IFocusable focus = null;
					foreach (IElement element in (Parent as ISection).Elements) 
					{
						if (element == null) 
							continue;
	
						if (element == this)
							focus = this; 
						else if (focus != null && element is IFocusable)
						{
							focus = (IFocusable)element;
							if (focus.Entry != null) break;
							focus = this;
						}
					}
					
					if (focus != this) 
					{
						TableView.ScrollToRow(focus.IndexPath, UITableViewScrollPosition.Top, true);
						focus.Entry.BecomeFirstResponder();
						
					}
					else
					{
						focus.Entry.ResignFirstResponder();
					}

					
					return true;
				};

				Entry.EditingDidEnd += delegate 
				{
					ValueProperty.Update();
				};

				ContentView = Entry;
			}
			else
			{
				if (DetailTextLabel != null)
					DetailTextLabel.Text = Value;
			}
		}
		
		public override void BindProperties()
		{
			base.BindProperties();

			EditModeProperty.BindTo(this, "EditMode");

			if (Entry != null)
			{
				PlaceholderProperty.BindTo(this, "Entry.Placeholder");
				IsPasswordProperty.BindTo(this, "Entry.SecureTextEntry");
				TextProperty.BindTo(this, "Entry.Text");
				ValueProperty.BindTo(this, "Entry.Text");
				DetailTextAlignmentProperty.BindTo(this, "Entry.TextAlignment");
				ReturnKeyTypeProperty.BindTo(this, "Entry.ReturnKeyType");
				KeyboardTypeProperty.BindTo(this, "Entry.KeyboardType");
				DetailTextFontProperty.BindTo(this, "Entry.Font");
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

