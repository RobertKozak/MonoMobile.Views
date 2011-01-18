//
// ButtonElement.cs
//
// Author:
//   Robert Kozak (rkozak@gmail.com)
//
// Copyright 2011, Nowcom Corporation
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
	using MonoTouch.MVVM;
	using MonoTouch.UIKit;

	/// <summary>
	///   The button element can be used to render a cell as a button that responds to Tap events
	/// </summary>
	public class ButtonElement : Element, ITappable
	{
		public UIFont Font;
		public UIColor TextColor;
		public UIColor BackgroundColor;

		public ICommand Command
		{
			get;
			set;
		}
		public object CommandParameter
		{
			get;
			set;
		}

		public ButtonElement(string caption) : base(caption)
		{
		}

		public override void InitializeCell(UITableView tableView)
		{
			Cell.SelectionStyle = UITableViewCellSelectionStyle.Blue;
			Cell.TextLabel.TextAlignment = UITextAlignment.Center;

			Cell.Accessory = UITableViewCellAccessory.None;

			Cell.BackgroundColor = BackgroundColor == null ? UIColor.White : BackgroundColor;

		
			var textLabel = Cell.TextLabel;
			textLabel.Text = Caption;
			textLabel.TextColor = TextColor == null ? UIColor.Black : TextColor;
			textLabel.Font = Font == null ? UIFont.BoldSystemFontOfSize(17) : Font;
			textLabel.Lines = 0;
		}

		public override string ToString()
		{
			return Caption;
		}

		public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath indexPath)
		{
			if (Command != null && Command.CanExecute(CommandParameter))
				Command.Execute(CommandParameter);
			
			tableView.DeselectRow(indexPath, true);
		}
	}
}

