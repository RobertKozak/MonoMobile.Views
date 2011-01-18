//
// StyledStringElement.cs
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
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	/// <summary>
	///   A version of the StringElement that can be styled with a number of options.
	/// </summary>
	public class StyledStringElement : StringElement
	{
		public StyledStringElement(string caption) : base(caption)
		{
		}
		public StyledStringElement(string caption, string value) : base(caption, value)
		{
		}

		public UIFont Font;
		public UIColor TextColor;
		public UIColor BackgroundColor;
		public UILineBreakMode LineBreakMode = UILineBreakMode.WordWrap;
		public int Lines = 1;
		public UITableViewCellAccessory Accessory = UITableViewCellAccessory.None;

		public override UITableViewElementCell NewCell()
		{
			return new UITableViewElementCell(Value == null ? UITableViewCellStyle.Default : UITableViewCellStyle.Value1, Id);
		}
		public override void InitializeCell(UITableView tableView)
		{
			Cell.SelectionStyle = UITableViewCellSelectionStyle.Blue;

			var textLabel = Cell.TextLabel;
			textLabel.Text = Caption;
			textLabel.TextAlignment = Alignment;
			textLabel.TextColor = TextColor == null ? UIColor.Black : TextColor;
			textLabel.Font = Font == null ? UIFont.SystemFontOfSize(14) : Font;
			textLabel.LineBreakMode = LineBreakMode;
			textLabel.Lines = 0;

			// The check is needed because the cell might have been recycled.
			if (Cell.DetailTextLabel != null)
				Cell.DetailTextLabel.Text = Value == null ? "" : Value;
		}
	}
}

