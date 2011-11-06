// 
//  ICell.cs
// 
//  Author:
//    Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
// 
//  Copyright 2011, Nowcom Corporation.
// 
//  Code licensed under the MIT X11 license
// 
//  Permission is hereby granted, free of charge, to any person obtaining
//  a copy of this software and associated documentation files (the
//  "Software"), to deal in the Software without restriction, including
//  without limitation the rights to use, copy, modify, merge, publish,
//  distribute, sublicense, and/or sell copies of the Software, and to
//  permit persons to whom the Software is furnished to do so, subject to
//  the following conditions:
// 
//  The above copyright notice and this permission notice shall be
//  included in all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//  LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//  OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//  WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 

namespace MonoMobile.Views
{
	using System;
	using MonoTouch.UIKit;
	
	public interface ICell
	{
		bool Visible { get; set; }
		bool Enabled { get; set; }

		Type ViewType { get; set; }
		string NibName { get; set; }

		UITableViewCellEditingStyle EditingStyle { get; set; }

		UITableViewCell Cell { get; set; }
	}

	public class CellData: IDataContext<object>
	{
		public object DataContext { get; set; }

		public UIImageView ImageView { get; set; }
		public UILabel TextLabel { get; set; } 
		public UILabel DetailTextLabel { get; set; }
		public UIView ContentView { get; set; } 
		public UIView BackgroundView { get; set; } 
		public UIView SelectedBackgroundView { get; set; }
		public string ReuseIdentifier { get; set; } 
		public UITableViewCellSelectionStyle SelectionStyle { get; set; }
		public bool Selected { get; set; }
		public bool Highlighted { get; set; }
		public UITableViewCellEditingStyle EditingStyle { get; set; } 
		public bool ShowsReorderControl { get; set; } 
		public bool ShouldIndentWhileEditing { get; set; } 
		public UITableViewCellAccessory Accessory { get; set; } 
		public UIView AccessoryView { get; set; } 
		public UITableViewCellAccessory EditingAccessory { get; set; } 
		public UIView EditingAccessoryView { get; set; } 
		public int IndentationLevel { get; set; } 
		public float IndentationWidth { get; set; } 
		public bool Editing { get; set; } 
		public bool ShowingDeleteConfirmation { get; set; } 
		public UIView MultipleSelectionBackgroundView{ get; set; }

		public void UpdateDataContext()
		{
		}

		public void UpdateContent()
		{
		}

		public void InititalizeCell()
		{

		}
	}
}

