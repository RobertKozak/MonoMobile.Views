//
// Section.cs
//
// Author:
//   Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak))
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
namespace MonoMobile.Views
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;
	
	[Preserve(AllMembers = true)]
	public class Section: NSObject, IDataContext<IList>, IEnumerable
	{
		internal DialogViewController Controller;

		private IList _DataContext;	
		public IList DataContext
		{ 
			get { return _DataContext; } 
			set { _DataContext = value; SetNumberOfRows(); }
		}
		
		public IDictionary<int, ListSource> ListSources { get; set; }
		public int Index { get; set; }
		public int NumberOfRows { get; set; }

		public IDictionary<string, IList<Type>> ViewTypes;
		public IDictionary<UITableViewCell, IList<UIView>> Views;
		
		public string HeaderText { get; set; }
		public string FooterText { get; set; }
		public UIView HeaderView { get; set; }
		public UIView FooterView { get; set; }
		
		public Section()
		{
			ListSources = new Dictionary<int, ListSource>(); 
			ViewTypes = new Dictionary<string, IList<Type>>();
			Views = new Dictionary<UITableViewCell, IList<UIView>>();
		}

		public Section(DialogViewController controller)
		{
			Controller = controller;

			ListSources = new Dictionary<int, ListSource>(); 
			ViewTypes = new Dictionary<string, IList<Type>>();
			Views = new Dictionary<UITableViewCell, IList<UIView>>();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Controller = null;

				if (HeaderView != null) HeaderView.Dispose();
				if (FooterView != null) FooterView.Dispose();

				foreach (var viewList in Views.Values)
				{
					foreach (var view in viewList)
					{
						var disposable = view as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
							disposable = null;
						}
					}
				}
			}

			base.Dispose(disposing);
		}

		public IEnumerator GetEnumerator()
		{
			foreach (var view in Views)
				yield return view;
		}

//		public int Add(object value)
//		{
//			int count = 0;
//			
////			var list = DataContext as IList;
////			if (list != null)
////			{
////				list.Add(value);
////			}
////			var members = data.GetType().GetMembers();
////			foreach (var item in data)
////			{
////				DataContext
////				Add(item);
////				count++;
////			}
//
//			return count;
//		}

		public void SetNumberOfRows()
		{
			var count = 0;
			//TODO: Come back and fix this for objects (member count)
			var collection = DataContext as ICollection;
			if (collection != null)
			{
				count = collection.Count;
			}

			NumberOfRows = count;
		}
	} 
}

