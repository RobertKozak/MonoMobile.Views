// 
//  DialogViewSearchDelegate.cs
// 
// Author:
//   Miguel de Icaza
//   With changes by Robert Kozak, Copyright 2011, Nowcom Corporation
// 
//  Copyright 2011 - 2012, Nowcom Corporation.
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
	using System.Linq;

	public class DialogViewSearchDelegate : UISearchBarDelegate
	{
		private readonly DialogViewController _Container;

		public DialogViewSearchDelegate(DialogViewController container)
		{
			_Container = container;
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing && _Container != null)
			{
				_Container.Dispose();;
			}

			base.Dispose(disposing);
		}
		public override void OnEditingStarted(UISearchBar searchbar)
		{
			searchbar.ShowsCancelButton = true;
			
			var searchable = _Container.TableView.Source as ISearchBar;
			if (searchable != null && searchable.IncrementalSearch)
			{
				var textField = searchbar.Subviews.FirstOrDefault((v)=>v.GetType() == typeof(UITextField)) as UITextField;
				if (textField != null)
					textField.ReturnKeyType = UIReturnKeyType.Done;	
			}

			_Container.StartSearch();
		}

//		public override void OnEditingStopped(UISearchBar searchbar)
//		{
//			var searchable = _Container.Root as ISearchBar;
//			if (searchable != null && searchable.IncrementalSearch)
//			{
//		//		searchbar.ShowsCancelButton = false;
//		//		_Container.FinishSearch(false);
//			}
//		}

		public override void TextChanged(UISearchBar searchbar, string searchText)
		{
			var searchable = _Container.TableView.Source as ISearchBar;
			if (searchable != null && searchable.IncrementalSearch)
				_Container.PerformFilter(searchText ?? "");
		}

		public override void CancelButtonClicked(UISearchBar searchbar)
		{
			searchbar.ShowsCancelButton = false;
			searchbar.ResignFirstResponder();
			new Wait(TimeSpan.FromMilliseconds(300), ()=> 
			{
				_Container.FinishSearch(false); 
				_Container.ToggleSearchbar();
			});
		}

		public override void SearchButtonClicked(UISearchBar searchbar)
		{
			_Container.SearchButtonClicked(searchbar.Text);
			var searchable = _Container.TableView.Source as ISearchBar;

			searchbar.ResignFirstResponder();

			if (searchable != null)
				_Container.PerformFilter(searchbar.Text);
		}
	}
}

