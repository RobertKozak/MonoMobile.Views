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
	using MonoTouch.UIKit;
	using MonoTouch.Foundation;

	/// <summary>
	/// An element that can be used to enter text.
	/// </summary>
	/// <remarks>
	/// This element can be used to enter text both regular and password protected entries. 
	///     
	/// The Text fields in a given section are aligned with each other.
	/// </remarks>
	public class EntryElement : Element<string>
	{
		private static  UITextField __UITextField = new UITextField();
		private  UITextAlignment __textAlignment = __UITextField.TextAlignment;
		private  UIKeyboardType __keyboard = __UITextField.KeyboardType; 
		private  UIFont _font = __UITextField.Font;

		/// <summary>
		/// The type of keyboard used for input, you can change
		/// this to use this for numeric input, email addressed,
		/// urls, phones.
		/// </summary>
		public UIKeyboardType KeyboardType = UIKeyboardType.Default;

		private string _Placeholder;
		private static UIFont _Font = UIFont.BoldSystemFontOfSize(17);
	
		private bool _IsPassword;
		public bool IsPassword
		{
			get
			{
				return _IsPassword;
			}
			set
			{
				_IsPassword = value;
				if (Entry != null && Entry.SecureTextEntry != _IsPassword)
				{
					Entry.SecureTextEntry = _IsPassword;
				}
			}
		}

		public bool JustifyText { get; set; }
		public UITextField Entry { get; set; }

		/// <summary>
		/// Constructs an EntryElement with the given caption, placeholder and initial value.
		/// </summary>
		/// <param name="caption">
		/// The caption to use
		/// </param>
		/// <param name="placeholder">
		/// Placeholder to display.
		public EntryElement(string caption, string placeholder) : base(caption)
		{
			_Placeholder = placeholder;
		}

		/// <summary>
		/// Constructs an EntryElement with the given caption, placeholder and initial value.
		/// </summary>
		/// <param name="caption">
		/// The caption to use
		/// </param>
		/// <param name="placeholder">
		/// Placeholder to display.
		/// </param>
		/// <param name="value">
		/// Initial value.
		/// </param>
		public EntryElement(string caption, string placeholder, string value) : base(caption)
		{
			Value = value;
			_Placeholder = placeholder;
		}

		/// <summary>
		/// Constructs  an EntryElement for password entry with the given caption, placeholder and initial value.
		/// </summary>
		/// <param name="caption">
		/// The caption to use
		/// </param>
		/// <param name="placeholder">
		/// Placeholder to display.
		/// <param name="isPassword">
		/// True if this should be used to enter a password.
		/// </param>
		public EntryElement(string caption, string placeholder, bool isPassword) : base(caption)
		{
			_IsPassword = isPassword;
			_Placeholder = placeholder;
		}

		/// <summary>
		/// Constructs  an EntryElement for password entry with the given caption, placeholder and initial value.
		/// </summary>
		/// <param name="caption">
		/// The caption to use
		/// </param>
		/// <param name="placeholder">
		/// Placeholder to display.
		/// </param>
		/// <param name="value">
		/// Initial value.
		/// </param>
		/// <param name="isPassword">
		/// True if this should be used to enter a password.
		/// </param>
		public EntryElement(string caption, string placeholder, string value, bool isPassword) : base(caption)
		{
			Value = value;
			_IsPassword = isPassword;
			_Placeholder = placeholder;
		}

		// 
		// Computes the X position for the entry by aligning all the entries in the Section
		//
		private SizeF ComputeEntryPosition(UITableView tv, UITableViewCell cell)
		{	
			SizeF max = new SizeF(-1, -1);
			var element = this as Element;
			ISection s = Parent as ISection;

			if (JustifyText)
			{
				if (s.EntryAlignment.Width != 0)
					return s.EntryAlignment;

				foreach (var e in s.Elements)
				{
					element = e as EntryElement;
					if (element != null)
						break;
				}
			}
				
			var size = tv.StringSize(element.Caption, _Font);
			if (size.Width > max.Width)
				max = size;

			s.EntryAlignment = new SizeF(25 + Math.Min(max.Width, 160), max.Height);
			return s.EntryAlignment;
		}

		public override void InitializeControls(UITableView tableView)
		{
			if (Entry == null)
			{
				SizeF size = ComputeEntryPosition(tableView, Cell);
				var _entry = new UITextField(new RectangleF(size.Width, (Cell.ContentView.Bounds.Height - size.Height) / 2 - 1, 290 - size.Width, size.Height)) { Tag = 1, Placeholder = _Placeholder ?? "", SecureTextEntry = _IsPassword };
				_entry.Font = UIFont.SystemFontOfSize(17);
				_entry.AddTarget(delegate { Value = _entry.Text; }, UIControlEvent.ValueChanged);

				Entry = _entry;

				Entry.Ended += delegate
				{
					Value = Entry.Text;
				};
				
				Entry.ShouldReturn += delegate
				{
					EntryElement focus = null;
					foreach (var e in (Parent as ISection).Elements)
					{
						if (e == this)
							focus = this; 
						else if (focus != null && e is EntryElement)
							focus = e as EntryElement;
					}
					
					if (focus != this)
					{
						tableView.ScrollToRow(focus.IndexPath, UITableViewScrollPosition.Middle, true);
						focus.Entry.BecomeFirstResponder();
						
					} 
					else
						focus.Entry.ResignFirstResponder();
					
					return true;
				};
				
				Entry.Started += delegate
				{
					var bindingExpression = GetBindingExpression("Value");
					
					if (bindingExpression != null) 
					{
						Entry.Text = (string)bindingExpression.ConvertbackValue(Value);
					}
					else
						Entry.Text = Value;

					EntryElement self = null;
					var returnType = UIReturnKeyType.Done;
					
					foreach (var e in (Parent as ISection).Elements)
					{
						if (e == this)
							self = this; else if (self != null && e is EntryElement)
							returnType = UIReturnKeyType.Next;
					}
					
					Entry.ReturnKeyType = returnType;
				};
			}

			Entry.KeyboardType = KeyboardType;
			Entry.TextAlignment = UITextAlignment.Right;
			
			Entry.Text = Value;

			Cell.TextLabel.Text = Caption;
			
			Cell.ContentView.AddSubview(Entry);
		}

		public override void InitializeCell(UITableView tableView)
		{
			RemoveTag(1);

			Cell.TextLabel.Font = _Font;

			base.InitializeCell(tableView);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Entry.Dispose();
				Entry = null;
			}
		}

		protected override void OnValueChanged()
		{
			base.OnValueChanged();
			
			if (Entry != null)
			{
				Entry.Text = Value;
			}
		}

		public override bool Matches(string text)
		{
			return (Value != null ? Value.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) != -1 : false) || base.Matches(text);
		}
	}
}

