//
// CheckboxView.cs
//
// Author:
//   Robert Kozak (rkozak@gmail.com) Twitter:@robertkozak
//
// Copyright 2011, Nowcom Corporation
//
// Based on code from MonoTouch.Dialog by Miguel de Icaza (miguel@gnome.org)
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
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;
	using MonoMobile.MVVM;

	public partial class CheckboxElement : BoolElement, ISelectable
	{
		public string Group { get; set; }

		public CheckboxElement(string caption) : base(caption)
		{
		}
		
		public override void UpdateCell()
		{
			base.UpdateCell();
			//There is a visual artifact when using Checkmark and UITableViewCellSelectionStyle.Blue;
			Cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			
			Value = (bool)ValueProperty.Value;
			ValueProperty.Update();
			UpdateSelected();
		}
		
		public override void UpdateSelected()
		{
			Cell.AccessoryView = null;
			Cell.Accessory = (bool)Value ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
			Cell.TextLabel.Text = Caption;
		}

		public override string ToString()
		{
			return Caption;
		}
	
		public void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			Value = !Value;
			ValueProperty.Update();
			UpdateSelected();
		}

	}
}

