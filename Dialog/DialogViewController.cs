//
// DialogViewController.cs: drives MonoMobile.MVVM
//
// Author:
//   Miguel de Icaza
//   With changes by Robert Kozak, Copyright 2011, Nowcom Corporation
//
// Code to support pull-to-refresh based on Martin Bowling's TweetTableView
// which is based in turn in EGOTableViewPullRefresh code which was created
// by Devin Doty and is Copyrighted 2009 enormego and released under the
// MIT X11 license
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
	using System.Drawing;
	using System.IO;
	using System.Linq;
	using System.Threading;
	using MonoMobile.MVVM.Utilities;
	using MonoTouch.Foundation;
	using MonoMobile.MVVM;
	using MonoTouch.ObjCRuntime;
	using MonoTouch.UIKit;

	public class DialogViewController : UITableViewController
	{
		private UISearchBar _Searchbar;
		private UITableView _TableView;
		private RefreshTableHeaderView _RefreshView;
		private IRoot _Root;
		private bool _Pushing;
		private bool _Dirty;
		private bool _Reloading;
		private ISection[] _OriginalSections;
		private IElement[][] _OriginalElements;
		private Source _TableSource;

		public UIImage BackgroundImage { get; set; }
		public UIColor BackgroundColor { get; set; }

		public UITableViewStyle Style = UITableViewStyle.Grouped;

		/// <summary>
		/// The _Root element displayed by the DialogViewController, the value can be changed during runtime to update the contents.
		/// </summary>
		public IRoot Root
		{
			get { return _Root; }
			set {
				if (_Root == value)
					return;
				if (_Root != null)
					_Root.Dispose();

				_Root = value;
				_Root.Controller = this;
				_Root.TableView = TableView;
				ReloadData();
			}
		}

		public bool ReloadCompleted
		{
			get { return !_Reloading; }
			set 
			{
				if (value)
				{
					ReloadComplete();
				}
			}
		}

		// If set, we automatically scroll the content to avoid showing the search bar until 
		// the user manually pulls it down.
		public bool AutoHideSearch { get; set; }
		public string SearchPlaceholder { get; set; }
		public bool EnableSearch { get; set; }
		public bool IncrementalSearch { get; set; }
		public SearchCommand SearchCommand { get; set; }


		public bool EnablePullToRefresh 
		{ 
			get { return _RefreshView != null; } 
			set 
			{
				if (value && _RefreshView == null)
				{
					var bounds = View.Bounds;
					_RefreshView = MakeRefreshTableHeaderView(new RectangleF(0, -bounds.Height, bounds.Width, bounds.Height), Root.DefaultSettingsKey);

					if (_Reloading)
						_RefreshView.SetActivity(true);

					TableView.AddSubview(_RefreshView);				
				}
				else
				{
					if(_RefreshView != null)
					{
						_RefreshView.Dispose();
						_RefreshView = null;
					}
				}
			} 
		}

		/// <summary>
		/// Invoke this method to trigger a data refresh.   
		/// </summary>
		/// <remarks>
		/// This will invoke the RerfeshRequested event handler, the code attached to it
		/// should start the background operation to fetch the data and when it completes
		/// it should call ReloadComplete to restore the control state.
		/// </remarks>
		public void TriggerRefresh()
		{
			TriggerRefresh(false);
		}

		private void TriggerRefresh(bool showStatus)
		{
			if (Root == null && Root.PullToRefreshCommand == null)
				return;
			
			if (_Reloading)
				return;
			
			_Reloading = true;
			
			if (_Reloading && showStatus && _RefreshView != null)
			{
				UIView.BeginAnimations("reloadingData");
				UIView.SetAnimationDuration(0.2);
				TableView.ContentInset = new UIEdgeInsets(60, 0, 0, 0);
				UIView.CommitAnimations();
			}
			
			if (_RefreshView != null)
				_RefreshView.SetActivity(true);
			
			Thread.Sleep(250);

			var refreshThread = new Thread(RefreshThread as ThreadStart);
			refreshThread.Start();
		}

		private void RefreshThread()
		{
			using (var pool = new NSAutoreleasePool())
			{
				InvokeOnMainThread(delegate
				{
					if (Root.PullToRefreshCommand != null)
						Root.PullToRefreshCommand.Execute(this);

					ReloadComplete();
				});
			}
		}

		/// <summary>
		/// Invoke this method to signal that a reload has completed, this will update the UI accordingly.
		/// </summary>
		public void ReloadComplete()
		{
			if (_RefreshView != null)
				_RefreshView.LastUpdate = DateTime.Now;
			if (!_Reloading)
				return;
			
			_Reloading = false;
			if (_RefreshView == null)
				return;
			
			_RefreshView.SetActivity(false);
			_RefreshView.Flip(false);
			UIView.BeginAnimations("doneReloading");
			UIView.SetAnimationDuration(0.3f);
			TableView.ContentInset = new UIEdgeInsets(0, 0, 0, 0);
			_RefreshView.SetStatus(RefreshStatus.PullToReload);
			UIView.CommitAnimations();
		}

		/// <summary>
		/// Controls whether the DialogViewController should auto rotate
		/// </summary>
		public bool Autorotate { get; set; }

		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			return Autorotate || toInterfaceOrientation == UIInterfaceOrientation.Portrait;
		}
		
		public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			if (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown)
				base.WillRotate(toInterfaceOrientation, duration);
		}

		public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate(fromInterfaceOrientation);
			ReloadData();
			ConfigureBackgroundImage();
		}

		/// <summary>
		/// Allows caller to programatically activate the search bar and start the search process
		/// </summary>
		public void StartSearch()
		{
			if (_OriginalSections != null)
				return;
			
			_Searchbar.BecomeFirstResponder();

			_OriginalSections = Root.Sections.ToArray();
			_OriginalElements = new IElement[_OriginalSections.Length][];

			for (int i = 0; i < _OriginalSections.Length; i++)
				_OriginalElements[i] = _OriginalSections[i].Elements.ToArray();
		}

		/// <summary>
		/// Allows the caller to programatically stop searching.
		/// </summary>
		public virtual void FinishSearch()
		{
			if (_OriginalSections == null)
				return;
			
			Root.Sections = new List<ISection>(_OriginalSections);
			_OriginalSections = null;
			_OriginalElements = null;

			ReloadData();
			HideSearch();
		}

		public delegate void SearchTextEventHandler(object sender, SearchChangedEventArgs args);
		public event SearchTextEventHandler SearchTextChanged;

		public virtual void OnSearchTextChanged(string text)
		{
			if (SearchTextChanged != null)
				SearchTextChanged(this, new SearchChangedEventArgs(text));
		}

		public void PerformFilter(string text)
		{
			if (_OriginalSections == null)
				return;
			
			OnSearchTextChanged(text);
			
			var newSections = new List<ISection>();
			
			if (SearchCommand == null)
			{
				for (int sidx = 0; sidx < _OriginalSections.Length; sidx++)
				{
					ISection newSection = null;
					var section = _OriginalSections[sidx];
					IElement[] elements = _OriginalElements[sidx];
					
					for (int eidx = 0; eidx < elements.Length; eidx++)
					{
						var searchableView = elements[eidx] as ISearchable;
						
						if (searchableView != null && searchableView.Matches(text))
						{
							if (newSection == null)
							{
								newSection = new Section(section.HeaderText, section.FooterText) { FooterView = section.FooterView, HeaderView = section.HeaderView };
								newSections.Add(newSection);
							}
							newSection.Add(elements[eidx]);
						}
					}
				}
			}
			else
			{
				newSections = SearchCommand.Execute(_OriginalSections, text);
			}
			
			Root.Sections = newSections;
			
			ReloadData();
		}

		public virtual void SearchButtonClicked(string text)
		{
		}

		private class SearchDelegate : UISearchBarDelegate
		{
			private DialogViewController _Container;

			public SearchDelegate(DialogViewController container)
			{
				_Container = container;
			}

			public override void OnEditingStarted(UISearchBar _Searchbar)
			{
				_Searchbar.ShowsCancelButton = true;

				if (_Container.IncrementalSearch)
				{
					var textField = _Searchbar.Subviews.FirstOrDefault((v)=>v.GetType() == typeof(UITextField)) as UITextField;
					if(textField != null)
						textField.ReturnKeyType = UIReturnKeyType.Done;	
				}

				_Container.StartSearch();
			}

			public override void OnEditingStopped(UISearchBar _Searchbar)
			{
				if (_Container.IncrementalSearch)
				{
					_Searchbar.ShowsCancelButton = false;
					_Container.FinishSearch();
				}
			}
			public override void TextChanged(UISearchBar _Searchbar, string searchText)
			{
				if (_Container.IncrementalSearch)
					_Container.PerformFilter(searchText ?? "");
			}

			public override void CancelButtonClicked(UISearchBar _Searchbar)
			{
				_Searchbar.ShowsCancelButton = false;
				_Container.FinishSearch();
			}

			public override void SearchButtonClicked(UISearchBar _Searchbar)
			{
				_Container.SearchButtonClicked(_Searchbar.Text);
				if (_Container.IncrementalSearch)
					_Container.FinishSearch();
				else
					_Container.PerformFilter(_Searchbar.Text);
			}
		}

		public class Source : UITableViewSource
		{
			private const float _SnapBoundary = 65;
			protected DialogViewController Container;
			protected IRoot Root;
			private bool _CheckForRefresh;

			public Source(DialogViewController container)
			{
				Container = container;
				Root = container.Root;
			}

			public override int RowsInSection(UITableView tableview, int section)
			{
				var s = Root.Sections[section];
				var count = s.Elements.Count;
				
				return count;
			}

			public override int NumberOfSections(UITableView tableView)
			{
				return Root.Sections.Count;
			}

			public override string TitleForHeader(UITableView tableView, int section)
			{
				return Root.Sections[section].Caption;
			}

			public override string TitleForFooter(UITableView tableView, int section)
			{
				return Root.Sections[section].FooterText;
			}

			public override UITableViewCell GetCell(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				var section = Root.Sections[indexPath.Section];
				var element = section.Elements[indexPath.Row]; 
				
				return element.GetCell(tableView) as UITableViewElementCell;
			}

			public override void RowSelected(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				Container.Selected(indexPath);
			}

			public override UIView GetViewForHeader(UITableView tableView, int sectionIdx)
			{
				var section = Root.Sections[sectionIdx];
				if (section.HeaderView == null && !string.IsNullOrEmpty(section.Caption))
				{
					section.HeaderView = CreateHeaderView(tableView, section.Caption);
					var themeable = section as IThemeable;
					if (themeable != null)
						themeable.ThemeChanged();
				}
				return section.HeaderView;
			}

			public override float GetHeightForHeader(UITableView tableView, int sectionIdx)
			{
				var section = Root.Sections[sectionIdx];
				if (section.HeaderView == null)
					return -1;
				return section.HeaderView.Frame.Height;
			}

			public override UIView GetViewForFooter(UITableView tableView, int sectionIdx)
			{
				var section = Root.Sections[sectionIdx];
				if (section.FooterView == null && !string.IsNullOrEmpty(section.FooterText))
				{
					section.FooterView = CreateFooterView(tableView, section.FooterText);
					var themeable = section as IThemeable;
					if (themeable != null)
						themeable.ThemeChanged();
				}
				return section.FooterView;
			}

			public override float GetHeightForFooter(UITableView tableView, int sectionIdx)
			{
				var section = Root.Sections[sectionIdx];
				if (section.FooterView == null)
					return -1;
				return section.FooterView.Frame.Height;
			}

			#region Pull to Refresh support
			public override void Scrolled(UIScrollView scrollView)
			{
				if (!_CheckForRefresh)
					return;
				if (Container._Reloading)
					return;
				var view = Container._RefreshView;
				if (view == null)
					return;
				
				var point = Container.TableView.ContentOffset;
				
				if (view.IsFlipped && point.Y > -_SnapBoundary && point.Y < 0)
				{
					view.Flip(true);
					view.SetStatus(RefreshStatus.PullToReload);
				}
				else if (!view.IsFlipped && point.Y < -_SnapBoundary)
				{
					view.Flip(true);
					view.SetStatus(RefreshStatus.ReleaseToReload);
				}
			}

			public override void DraggingStarted(UIScrollView scrollView)
			{
				_CheckForRefresh = true;
			}

			public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
			{
				if (Container._RefreshView == null)
					return;

				_CheckForRefresh = false;
				if (Container.TableView.ContentOffset.Y > -_SnapBoundary)
					return;

				Container.TriggerRefresh(true);
			}
			#endregion

			private UIView CreateHeaderView(UITableView tableView, string caption)
			{
				var headerLabel = new UILabel();

				headerLabel.Font = UIFont.BoldSystemFontOfSize(UIFont.LabelFontSize);
				var size = headerLabel.StringSize(caption, headerLabel.Font);
				
				var bounds = new RectangleF(tableView.Bounds.X, tableView.Bounds.Y, tableView.Bounds.Width, size.Height + 10);
				var frame = new RectangleF(bounds.X + 20, bounds.Y, bounds.Width - 20, size.Height + 10);
	
				headerLabel.Bounds = bounds;
				headerLabel.Frame = frame;
				
				headerLabel.TextColor = UIColor.FromRGB(76, 86, 108);
				headerLabel.ShadowColor = UIColor.White;
				headerLabel.ShadowOffset = new SizeF(0, 1);
				headerLabel.Text = caption;
				
				var view = new UIView(bounds) { BackgroundColor = tableView.BackgroundColor };

				if (tableView.Style == UITableViewStyle.Grouped)
				{
					headerLabel.BackgroundColor = UIColor.Clear;
					view.Opaque = false;
					view.BackgroundColor = UIColor.Clear;
				}

				view.AddSubview(headerLabel);

				return view;
			}
			
			private UIView CreateFooterView(UITableView tableView, string caption)
			{
				var bounds = tableView.Bounds;
				var footerLabel = new UILabel();

				footerLabel.Font = UIFont.SystemFontOfSize(15);
				var size = footerLabel.StringSize(caption, footerLabel.Font);
				var height = size.Height * (caption.Count((ch)=>ch == '\n') + 1); 
				caption = caption.Replace("\n","");
				var rect = new RectangleF(bounds.X + 10, bounds.Y, bounds.Width - 20, height + 10);
				
				footerLabel.Bounds = rect;
				footerLabel.BackgroundColor = UIColor.Clear;
				footerLabel.TextAlignment = UITextAlignment.Center;
				footerLabel.TextColor = UIColor.FromRGB(76, 86, 108);
				footerLabel.ShadowColor = UIColor.White;
				footerLabel.ShadowOffset = new SizeF(0, 1);
				footerLabel.Text = caption;

				return footerLabel;
			}
		}

		//
		// Performance trick, if we expose GetHeightForRow, the UITableView will
		// probe *every* row for its size;   Avoid this by creating a separate
		// model that is used only when we have items that require resizing
		//
		public class SizingSource : Source
		{
			public SizingSource(DialogViewController controller) : base(controller)
			{
			}

			public override float GetHeightForRow(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				var section = Root.Sections[indexPath.Section];
				var element = section.Elements[indexPath.Row];
				
				var sizable = element as ISizeable;
				if (sizable != null)
					return sizable.GetHeight(tableView, indexPath);
		
				return tableView.RowHeight;
			}
		}

		/// <summary>
		/// Activates a nested view controller from the DialogViewController.   
		/// If the view controller is hosted in a UINavigationController it
		/// will push the result.   Otherwise it will show it as a modal
		/// dialog
		/// </summary>
		public void ActivateController(UIViewController controller, DialogViewController oldController)
		{
			_Dirty = true;
			
			var parent = ParentViewController;
			var nav = parent as UINavigationController;
			
			if (typeof(DialogViewController) == controller.GetType())
			{
				var dialog = (DialogViewController)controller;

				dialog.TableView.Opaque = false;
				
				if (dialog.BackgroundImage == null)
					dialog.TableView.BackgroundColor = oldController.TableView.BackgroundColor;
			}
			
			// We can not push a nav controller into a nav controller
			if (nav != null && !(controller is UINavigationController))
				nav.PushViewController(controller, true);
			else
				PresentModalViewController(controller, true);
		}

		/// <summary>
		/// Dismisses the view controller.   It either pops or dismisses
		/// based on the kind of container we are hosted in.
		/// </summary>
		public void DeactivateController(bool animated)
		{
			var parent = ParentViewController;
			var nav = parent as UINavigationController;
			
			if (nav != null)
				nav.PopViewControllerAnimated(animated);
			else
				DismissModalViewControllerAnimated(animated);
		}

		private void SetupSearch()
		{
			var searchbar = Root as ISearchBar;
			if (searchbar != null)
			{
				IncrementalSearch = searchbar.IncrementalSearch ?  IncrementalSearch || searchbar.IncrementalSearch : false;
				AutoHideSearch = AutoHideSearch || searchbar.AutoHideSearch;
				EnableSearch = EnableSearch || searchbar.EnableSearch;
				SearchCommand = SearchCommand ?? searchbar.SearchCommand;

				if (string.IsNullOrEmpty(SearchPlaceholder))
					SearchPlaceholder = searchbar.SearchPlaceholder;
			}

			if (EnableSearch)
			{
				_Searchbar = new UISearchBar(new RectangleF(0, 0, TableView.Bounds.Width, TableView.RowHeight)) { Delegate = new SearchDelegate(this) };
				
				if (SearchPlaceholder != null)
					_Searchbar.Placeholder = SearchPlaceholder;

				TableView.TableHeaderView = _Searchbar;	
			}
			else if (TableView.TableHeaderView != null)
			{
				// For some reason setting to null when already null causes it to shift a few pixels down
				// Should work for MonoTouch 3.0
				//TableView.TableHeaderView = null;
			}
		}

		public void Selected(NSIndexPath indexPath)
		{
			var section = Root.Sections[indexPath.Section];
			var element = section.Elements[indexPath.Row];
			var selectable = element as ISelectable;

			if (selectable != null)
			{
				if (element.Cell.SelectionStyle != UITableViewCellSelectionStyle.None)
				{
					TableView.DeselectRow(indexPath, false);
					
					UIView.Animate(0.3f, delegate { element.Cell.Highlighted = true;  }, delegate { element.Cell.Highlighted = false; });
				}

				selectable.Selected(this, TableView, indexPath);
			}
		}

		public virtual UITableView MakeTableView(RectangleF bounds, UITableViewStyle style)
		{
			return new CustomTableView(bounds, style) { Controller = this };
		}

		public override void LoadView()
		{
			var themeable = Root as IThemeable; 
			if (themeable != null)
			{
				if (themeable.Theme.TableViewStyle.HasValue)
				{
					Style = themeable.Theme.TableViewStyle.Value;
				}
			}

			_TableView = MakeTableView(UIScreen.MainScreen.Bounds, Style);

			TableView = _TableView;

			TableView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin;
			TableView.AutosizesSubviews = true;

			UpdateSource();
			View = TableView;
		
			SetupSearch();

			if (Root == null)
				return;
			
			ConfigureToolbarItems();

			Root.TableView = TableView;
			
			if (themeable != null)
			{
				var separatorColor = themeable.Theme.SeparatorColor;
				if (separatorColor != null)
					TableView.SeparatorColor = separatorColor;

				var separatorStyle = themeable.Theme.SeparatorStyle;
				if (separatorStyle.HasValue)
					TableView.SeparatorStyle = separatorStyle.Value;
			}

			EnablePullToRefresh = Root.PullToRefreshCommand != null;
		}

		private void ConfigureToolbarItems()
		{
			if (Root != null && Root.ToolbarButtons != null)
			{
				SetToolbarItems(Root.ToolbarButtons.ToArray(), true);
			}
		}

		private void ConfigureBackgroundImage()
		{
			if (BackgroundColor == null)
			{
				if (BackgroundImage == null)
				{
					if (Root != null)
					{
						if (Root.Theme.BackgroundUri != null)
						{
							var imageUri = Root.Theme.BackgroundUri;
							BackgroundImage = ImageLoader.DefaultRequestImage(imageUri, null);
						}
						
						if (Root.Theme.BackgroundColor != null)
						{
							BackgroundColor = Root.Theme.BackgroundColor;
						}
		
						if (Root.Theme.BackgroundImage != null)
						{
							BackgroundImage = Root.Theme.BackgroundImage;
						}
					}
				}
		
				if (BackgroundImage != null)
				{
					BackgroundColor = UIColor.FromPatternImage(BackgroundImage);
				}
			}
				
			if (BackgroundColor != null)
			{
				if (TableView.RespondsToSelector(new Selector("backgroundView")))
				{
					if (TableView.BackgroundView == null)
					{
						TableView.BackgroundView = new UIView() { Opaque = false, BackgroundColor = UIColor.Clear };
					}
				}
				if (ParentViewController != null)
				{
					TableView.BackgroundColor = UIColor.Clear;
					ParentViewController.View.BackgroundColor = BackgroundColor;
				} 
				else
				{
					TableView.BackgroundColor = BackgroundColor;
				}
			}
		}

		public virtual RefreshTableHeaderView MakeRefreshTableHeaderView(RectangleF rect, string defaultSettingsKey)
		{
			return new RefreshTableHeaderView(rect, defaultSettingsKey);
		}
		
		private void HideSearch()
		{
			if (AutoHideSearch && EnableSearch && TableView.ContentOffset.Y < TableView.TableHeaderView.Bounds.Height)
			{
				TableView.ContentOffset = new PointF(0, TableView.TableHeaderView.Bounds.Height);
			}	
			
			if (_Searchbar != null)
			{
				_Searchbar.ResignFirstResponder();
				_Searchbar.Text = string.Empty;
			}
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			
			if (Root == null)
				return;

			HideSearch();
			
			Root.Prepare();
			
			NavigationItem.HidesBackButton = !_Pushing;

			if (Root.Caption != null)
				NavigationItem.Title = Root.Caption;

			if (_Dirty)
			{
				TableView.ReloadData();
				_Dirty = false;
			}

			var parent = ParentViewController;
			var nav = parent as UINavigationController;
			if (nav != null)
			{
				nav.SetToolbarHidden(ToolbarItems == null, true);
				
				if (Root.NavbarButtons != null)
				{
					foreach (var button in Root.NavbarButtons)
					{
						if (button.Location == BarButtonLocation.Right)
							NavigationItem.RightBarButtonItem = button;
						else
							NavigationItem.LeftBarButtonItem = button;
					}
				}

				nav.NavigationBar.Opaque = false;

				var themeable = Root as IThemeable;
				if (themeable != null)
				{
					if (themeable.Theme.BarStyle.HasValue)
					{
						nav.NavigationBar.BarStyle = themeable.Theme.BarStyle.Value;
					}

//				if (!string.IsNullOrEmpty(_Root.NavbarImage))
//				{
//					UIView view = new UIView(new RectangleF(0f, 0f, nav.NavigationBar.Frame.Width, nav.NavigationBar.Frame.Height));
//					view.BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle(_Root.NavbarImage).ImageToFitSize(view.Bounds.Size));
//					nav.NavigationBar.InsertSubview(view, 0);
//				}
//				else
					nav.NavigationBar.Translucent = themeable.Theme.BarTranslucent;
				//	if (themeable.Theme.BarTintColor != UIColor.Clear)
					{
						nav.NavigationBar.TintColor = themeable.Theme.BarTintColor;
						nav.Toolbar.TintColor = themeable.Theme.BarTintColor;
					}
				}
			}

			if (Root != null)
			{
				var index = Root.ItemIndex;
				if (index > -1)
				{
					var path = Root.PathForRadio();
					TableView.ScrollToRow(path, UITableViewScrollPosition.Top, false);
				}
			}

			ConfigureBackgroundImage();
		}

		public virtual Source CreateSizingSource(bool unevenRows)
		{
			return unevenRows ? new SizingSource(this) : new Source(this);
		}

		private void UpdateSource()
		{
			if (Root == null)
				return;
			
			_TableSource = CreateSizingSource(Root.UnevenRows);
			TableView.Source = _TableSource;
		}

		public void ReloadData()
		{
			if (Root == null)
				return;
			
			Root.Prepare();
			if (TableView != null)
			{
				UpdateSource();
				TableView.ReloadData();
			}
			_Dirty = false;
		}

		public event EventHandler ViewDissapearing;

		public override void ViewWillDisappear(bool animated)
		{
			HideSearch();

			base.ViewWillDisappear(animated);
			if (ViewDissapearing != null)
				ViewDissapearing(this, EventArgs.Empty);
		}

		protected void PrepareRoot(IRoot root)
		{
			Root = root;
			if (Root != null)
				Root.Prepare();
		}

		protected DialogViewController(bool pushing) : base(UITableViewStyle.Grouped)
		{
			_Pushing = pushing;
			IncrementalSearch = true;
		}

		protected DialogViewController(UITableViewStyle style, bool pushing) : base(style)
		{
			_Pushing = pushing;
			Style = style;
			IncrementalSearch = true;
		}

		public DialogViewController(UITableViewStyle style, BindingContext binding, bool pushing) : base(style)
		{
			if (binding == null)
				throw new ArgumentNullException("binding");
			_Pushing = pushing;
			IncrementalSearch = true;
			Style = style;
			
			PrepareRoot(binding.Root);
		}

		public DialogViewController(BindingContext binding, bool pushing) : this(UITableViewStyle.Grouped, binding, pushing)
		{
		}

		public DialogViewController(IRoot root) : base(UITableViewStyle.Grouped)
		{
			IncrementalSearch = true;

			PrepareRoot(root);
		}

		public DialogViewController(UITableViewStyle style, IRoot root) : base(style)
		{
			IncrementalSearch = true;

			PrepareRoot(root);
		}

		/// <summary>
		///     Creates a new DialogViewController from a IRoot and sets the push status
		/// </summary>
		/// <param name="_Root">
		/// The <see cref="IRoot"/> containing the information to render.
		/// </param>
		/// <param name="_Pushing">
		/// A <see cref="System.Boolean"/> describing whether this is being pushed 
		/// (NavigationControllers) or not.   If _Pushing is true, then the back button 
		/// will be shown, allowing the user to go back to the previous controller
		/// </param>
		public DialogViewController(IRoot root, bool pushing) : base(UITableViewStyle.Grouped)
		{
			IncrementalSearch = true;

			_Pushing = pushing;
			PrepareRoot(root);
		}

		public DialogViewController(UITableViewStyle style, IRoot root, bool pushing) : base(style)
		{
			IncrementalSearch = true;

			_Pushing = pushing;
			Style = style;
			PrepareRoot(root);
		}
	}

	public class CustomTableView : UITableView
	{		
		public DialogViewController Controller { get; set; }
		
		public CustomTableView(RectangleF bounds, UITableViewStyle style) : base(bounds, style)
		{
		}

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			if (Controller != null)
			{
				foreach (var section in Controller.Root.Sections) 
				{
					foreach(var element in section.Elements)
					{
						var selected = element as IFocusable;
						if (selected != null && selected.InputControl != null && selected.InputControl.IsFirstResponder) 
						{
							selected.InputControl.ResignFirstResponder();
							break;
						}
					}
				}	
			}

			base.TouchesEnded(touches, evt);
		}
	}
}
