// 
//  DataContextBinder.cs
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
	using System.Collections.Specialized;
	using MonoTouch.UIKit;
	using MonoTouch.Foundation;
	using System.ComponentModel;

	public class DataContextBinder: NSObject, IHandleNotifyPropertyChanged, IHandleNotifyCollectionChanged
	{
		public Section Section { get; set; }
		public DialogViewController Controller { get; set; }

		public DataContextBinder(DialogViewController controller, Section section)
		{
			Section = section;
			Controller = controller;
		}
		
//		protected override void Dispose(bool disposing)
//		{
//			if (disposing)
//			{
//				Controller = null;
//				Section = null;
//			}
//
//			base.Dispose(disposing);
//		}

		public void HandleNotifyPropertyChanged(object sender, PropertyChangedEventArgs e)
		{		
			if (e.PropertyName != "Count" && e.PropertyName != "Item[]")
			{
				if (Controller == MonoMobileApplication.CurrentViewController)
				{
					//Controller.TableView.ReloadSections(NSIndexSet.FromIndex(Section.Index), UITableViewRowAnimation.Automatic);
				}
			}
		}

		//TODO: Double check each one of these for out of bounds conditions
		public void HandleNotifyCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{	
			var needsReload = Section.DataContext.Count == 0;

			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				var index = 0;
				foreach (var item in e.NewItems)
				{
					var row = e.NewStartingIndex < 0 ? 0 : e.NewStartingIndex;
					AddRow(Section, row + index, item);
					index++;
				}
			}

			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (var item in e.OldItems)
				{
					RemoveRow(Section, item);
				}
			}

			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				Section.DataContext.Clear();
				needsReload = true;
			}

			if (e.Action == NotifyCollectionChangedAction.Move)
			{
				var index = 0;
				foreach (var newItem in e.NewItems)
				{
					var row = e.OldStartingIndex + index;
					var oldItem = Section.DataContext[row];

					row = e.NewStartingIndex + index;

					RemoveRow(Section, oldItem, UITableViewRowAnimation.Fade);
					InsertRow(Section, row, newItem, UITableViewRowAnimation.Left);
					index++;
				}				
			}

			if (e.Action == NotifyCollectionChangedAction.Replace)
			{
				var index = 0;
				foreach (var item in e.NewItems)
				{
					var row = index;
					ReplaceRow(Section, e.OldItems[row], item);
					index++;
				}
			}

			needsReload = Section.DataContext.Count == 0 || needsReload;

			if (needsReload)
			{
				if (Controller == MonoMobileApplication.CurrentViewController)
				{
					Controller.TableView.ReloadSections(NSIndexSet.FromIndex(Section.Index), UITableViewRowAnimation.Automatic);
				}
				else
				{
					Controller.TableView.ReloadData();
				}	
			}
		}
		
		private void AddPropertyChangedHandler(object item)
		{
			var notifyPropertyChanged = item as INotifyPropertyChanged;
			if (notifyPropertyChanged != null)
			{
				notifyPropertyChanged.PropertyChanged += HandleNotifyPropertyChanged;
			}
		}

		private void RemovePropertyChangedHandler(object item)
		{
			var notifyPropertyChanged = item as INotifyPropertyChanged;
			if (notifyPropertyChanged != null)
			{
				notifyPropertyChanged.PropertyChanged -= HandleNotifyPropertyChanged;
			}
		}
		
		private void AddRow(Section section, int row, object item, UITableViewRowAnimation animation = UITableViewRowAnimation.Fade)
		{
			AddPropertyChangedHandler(item);

			section.DataContext.Insert(row, item);
					
			if (Controller == MonoMobileApplication.CurrentViewController)
			{
				var indexPaths = new NSIndexPath[] { NSIndexPath.FromRowSection(row, section.Index) };
				Controller.TableView.InsertRows(indexPaths, animation);
			}
		}

		private void InsertRow(Section section, int row, object item, UITableViewRowAnimation animation = UITableViewRowAnimation.Fade)
		{
			AddPropertyChangedHandler(item);

			section.DataContext.Insert(row, item); 
			
			if (Controller == MonoMobileApplication.CurrentViewController)
			{		
				var indexPaths = new NSIndexPath[] { NSIndexPath.FromRowSection(row, section.Index) };
				Controller.TableView.InsertRows(indexPaths, animation);
			}
		}

		private void RemoveRow(Section section, object item, UITableViewRowAnimation animation = UITableViewRowAnimation.Fade)
		{
			RemovePropertyChangedHandler(item);

			var row = section.DataContext.IndexOf(item);
			section.DataContext.Remove(item);
					
			if (Controller == MonoMobileApplication.CurrentViewController)
			{
				var indexPaths = new NSIndexPath[] { NSIndexPath.FromRowSection(row, section.Index) };
				Controller.TableView.DeleteRows(indexPaths, animation);
			}
		}

		private void ReplaceRow(Section section, object oldItem, object newItem, UITableViewRowAnimation animation = UITableViewRowAnimation.Fade)
		{
			RemovePropertyChangedHandler(oldItem);
			AddPropertyChangedHandler(newItem);

			var row = section.DataContext.IndexOf(oldItem);

			section.DataContext[row] = newItem;
					
			if (Controller == MonoMobileApplication.CurrentViewController)
			{
				var indexPaths = new NSIndexPath[] { NSIndexPath.FromRowSection(row, section.Index) };
				Controller.TableView.ReloadRows(indexPaths, animation);
			}
		}
	}
}

