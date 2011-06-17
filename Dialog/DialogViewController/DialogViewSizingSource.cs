// 
//  DialogViewSizingSource.cs
// 
// Author:
//   Miguel de Icaza
//   With changes by Robert Kozak, Copyright 2011, Nowcom Corporation
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
namespace MonoMobile.MVVM
{
	using System;
	using MonoTouch.UIKit;
	using MonoTouch.Foundation;

	//
	// Performance trick, if we expose GetHeightForRow, the UITableView will
	// probe *every* row for its size;   Avoid this by creating a separate
	// model that is used only when we have items that require resizing
	//
	public class DialogViewSizingSource : DialogViewDataSource
	{
		public DialogViewSizingSource(DialogViewController controller) : base(controller)
		{
		}

		public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			var section = Root.Sections[indexPath.Section];
			var element = section.Elements[indexPath.Row];
			
			var sizable = element as ISizeable;
			if (sizable != null)
				return sizable.GetHeight(tableView, indexPath);
			
			return tableView.RowHeight;
		}
	}

}

