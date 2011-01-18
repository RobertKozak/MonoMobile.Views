//
// ImageStringElement.cs
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
	using MonoTouch.UIKit;

	public class ImageStringElement : StringElement
	{
		private UIImage image;

		public UITableViewCellAccessory Accessory { get; set; }
		
		public ImageStringElement() : this("")
		{
		}

		public ImageStringElement(string caption) : base(caption)
		{
			this.Accessory = UITableViewCellAccessory.None;
		}

		public ImageStringElement(string caption, UIImage image) : base(caption)
		{
			this.image = image;
			this.Accessory = UITableViewCellAccessory.None;
		}

		public ImageStringElement(string caption, string value, UIImage image) : base(caption, value)
		{
			this.image = image;
			this.Accessory = UITableViewCellAccessory.None;
		}

		public override UITableViewElementCell NewCell()
		{
			return new UITableViewElementCell(Value == null ? UITableViewCellStyle.Default : UITableViewCellStyle.Subtitle, Id);

		}
		public override void InitializeCell(UITableView tableView)
		{
			Cell.SelectionStyle = UITableViewCellSelectionStyle.Blue;
			Cell.Accessory = Accessory;
			Cell.TextLabel.Text = Caption;
			Cell.TextLabel.TextAlignment = Alignment;

			Cell.ImageView.Image = image;

			// The check is needed because the cell might have been recycled.
			if (Cell.DetailTextLabel != null)
				Cell.DetailTextLabel.Text = Value == null ? "" : Value;
		}
	}
}

