//
// CommandBarButtonItem.cs
//
// Author:
//   Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak
//
// Copyright 2011, Nowcom Corporation.
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
namespace MonoMobile.Views
{
	using System;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;
	using MonoTouch.ObjCRuntime;

	public class CommandBarButtonItem: UIBarButtonItem
	{
		public BarButtonLocation Location { get; set; }
		public int Order { get; set; }
		public ICommand Command { get; set; }
		
		public CommandBarButtonItem(UIImage image, UIBarButtonItemStyle style, EventHandler handler): base(image, style, handler) {}
		public CommandBarButtonItem(string title, UIBarButtonItemStyle style, EventHandler handler): base(title, style, handler) {}
		public CommandBarButtonItem(UIBarButtonSystemItem systemItem, EventHandler handler): base(systemItem, handler) {}
		public CommandBarButtonItem(UIBarButtonSystemItem systemItem): base(systemItem, delegate { Command.Execute(null); }) {}
		public CommandBarButtonItem(): base() { }
		public CommandBarButtonItem(NSCoder coder): base(coder) {}
		public CommandBarButtonItem(IntPtr handle): base(handle) {}
		public CommandBarButtonItem(UIImage image, UIBarButtonItemStyle style, NSObject target, Selector action): base(image, style, target, action) {}
		public CommandBarButtonItem(string title, UIBarButtonItemStyle style, NSObject target, Selector action): base(title, style, target, action) {}
		public CommandBarButtonItem(UIBarButtonSystemItem systemItem, NSObject target, Selector action): base(systemItem, target, action) {}
		public CommandBarButtonItem(UIView customView): base(customView) {}
	}
}

