//
// StringElement.cs
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
	using MonoTouch.Foundation;
	using MonoTouch.MVVM;
	using MonoTouch.UIKit;

	/// <summary>
	///   The string element can be used to render some text in a cell 
	///   that can optionally respond to tap events.
	/// </summary>
	public class StringElement : Element<string>, ITappable
	{
		public UITextAlignment Alignment = UITextAlignment.Left;
		private UILabel Label { get; set; }

		public ICommand Command { get; set; }
		public object CommandParameter { get; set; }

		public StringElement(string caption) : base(caption)
		{
		}

		public StringElement(string caption, string value) : base(caption)
		{
			Value = value;
		}

		public override UITableViewElementCell NewCell()
		{
			return new UITableViewElementCell(Value == null ? UITableViewCellStyle.Default : UITableViewCellStyle.Value1, Id);
		}

		public override void InitializeCell(UITableView tableView)
		{
			Cell.SelectionStyle = (Command != null) ? UITableViewCellSelectionStyle.Blue : UITableViewCellSelectionStyle.None;

			Cell.Accessory = UITableViewCellAccessory.None;
			
			TextLabel = Cell.TextLabel;
			TextLabel.Text = Caption;
			TextLabel.TextAlignment = Alignment;

			// The check is needed because the cell might have been recycled.
			if (Cell.DetailTextLabel != null)
			{
				Label = Cell.DetailTextLabel;
				Label.Text = Value == null ? "" : Value;
			}
		}

		public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath indexPath)
		{
			if (Command != null && Command.CanExecute(CommandParameter))
				Command.Execute(CommandParameter);
			
			tableView.DeselectRow(indexPath, true);
		}

		public override bool Matches(string text)
		{
			return (Value != null ? Value.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) != -1 : false) || base.Matches(text);
		}

		protected override void OnValueChanged()
		{
			base.OnValueChanged();
			if (Label != null)
				Label.Text = Value;
		}
	}
}

