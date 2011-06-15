//
// BaseBooleanImageElement.cs
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
	using System.Drawing;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	/// <summary>
	/// This class is used to render a string + a state in the form
	/// of an image.  
	/// </summary>
	/// <remarks>
	/// It is abstract to avoid making this element
	/// keep two pointers for the state images, saving 8 bytes per
	/// slot.   The more derived class "BooleanImageElement" shows
	/// one way to implement this by keeping two pointers, a better
	/// implementation would return pointers to images that were 
	/// preloaded and are static.
	/// 
	/// A subclass only needs to implement the GetImage method.
	/// </remarks>
	public abstract class BaseBooleanImageElement : BoolElement
	{
		public class TextWithImageCellView : UITableViewElementCell
		{
			const int fontSize = 17;
			static UIFont font = UIFont.BoldSystemFontOfSize(fontSize);
			BaseBooleanImageElement parent;
			UILabel label;
			UIButton button;
			const int ImageSpace = 32;
			const int Padding = 8;

			public TextWithImageCellView(BaseBooleanImageElement parent, NSString Id) : base(UITableViewCellStyle.Value1, Id, parent)
			{
				this.parent = parent;
				label = new UILabel { TextAlignment = UITextAlignment.Left, Text = parent.Caption, Font = font };
				button = UIButton.FromType(UIButtonType.Custom);
				button.TouchDown += delegate
				{
					parent.DataContext = !(bool)parent.DataContext;
					UpdateImage();
					if (parent.Tapped != null)
						parent.Tapped();
				};
				ContentView.Add(label);
				ContentView.Add(button);
				UpdateImage();
			}

			void UpdateImage()
			{
				button.SetImage(parent.GetImage(), UIControlState.Normal);
			}

			public override void LayoutSubviews()
			{
				base.LayoutSubviews();
				var full = ContentView.Bounds;
				var frame = full;
				frame.Height = 22;
				frame.X = Padding;
				frame.Y = (full.Height - frame.Height) / 2;
				frame.Width -= ImageSpace + Padding;
				label.Frame = frame;
				
				button.Frame = new RectangleF(full.Width - ImageSpace, -3, ImageSpace, 48);
			}

			public void UpdateFrom(BaseBooleanImageElement newParent)
			{
				parent = newParent;
				UpdateImage();
				label.Text = parent.Caption;
				SetNeedsDisplay();
			}
		}

		public BaseBooleanImageElement(string caption, bool value) : base(caption, value)
		{
			Id = new NSString("BooleanImageElement");
		}

		public event NSAction Tapped;

		protected abstract UIImage GetImage();

		public override UITableViewElementCell GetCell(UITableView tableView)
		{
			var cell = tableView.DequeueReusableCell(Id) as TextWithImageCellView;
			if (cell == null)
				cell = new TextWithImageCellView(this, Id);
			else
				cell.UpdateFrom(this);
			Cell = cell;
			return cell as UITableViewElementCell;
		}
	}
}

