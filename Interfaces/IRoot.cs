//
// IRoot.cs
//
// Author:
//   Robert Kozak (rkozak@gmail.com) Twitter:@robertkozak
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
namespace MonoMobile.MVVM
{
	using System;
	using System.Collections.Generic;
	using MonoTouch.UIKit;
	using MonoTouch.Foundation;
	using MonoMobile.MVVM;

	public interface IRoot: IThemeable, IDisposable
	{	
		string Value { get; set; }
		ICommand PullToRefreshCommand { get; set; }
		string DefaultSettingsKey { get; set; }

		ViewBinding ViewBinding { get; set; }
		DialogViewController Controller { get; set; }
		
		List<CommandBarButtonItem> ToolbarButtons { get; set; }
		List<CommandBarButtonItem> NavbarButtons { get; set; }
 
		List<ISection> Sections { get; set; }
		bool UnevenRows { get; set;}
		string Caption { get; set; }

		UITableView TableView { get; set; }
		int Count { get; }
		int Index { get; set; }
		NSIndexPath PathForRadio();

		List<object> SelectedItems { get; }
		object SelectedItem { get; set; }

		void Prepare();
		void Add(IEnumerable<ISection> sections);
		void Add(ISection section);
		void Clear();
		int IndexOf(ISection section);
	}
}