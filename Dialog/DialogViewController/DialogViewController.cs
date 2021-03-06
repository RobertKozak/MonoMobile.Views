//
// DialogViewController.cs: drives MonoMobile.Views
//
//  Author:
//    Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
// 
//  Copyright 2011 - 2012, Nowcom Corporation.
//
//  Code licensed under the MIT X11 license
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
namespace MonoMobile.Views
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Linq;
	using System.Reflection;
	using System.Threading;
	using MonoMobile.Views.Utilities;
	using MonoTouch.Foundation;
	using MonoTouch.ObjCRuntime;
	using MonoTouch.UIKit;
			
	public class DialogViewController : UITableViewController
	{
		private readonly CommandBarButtonItem _LeftFixedSpace = new CommandBarButtonItem(UIBarButtonSystemItem.FixedSpace) { Location = BarButtonLocation.Left };
		private readonly CommandBarButtonItem _RightFixedSpace = new CommandBarButtonItem(UIBarButtonSystemItem.FixedSpace) { Location = BarButtonLocation.Right };

		private UITableView _TableView;

		private bool _Pushing;
		private bool _Dirty;

		private bool DisableScrolling;
		
		private UISearchBar _Searchbar;
		private Dictionary<int, Section> _OriginalSections;
		private List<object> _OriginalDataContext;

		public List<CommandBarButtonItem> ToolbarButtons { get; set; }		
		public List<CommandBarButtonItem> NavbarButtons { get; set; }
		
		public Theme Theme { get; set; }

		public RefreshTableHeaderView RefreshView { get; set; }
		public bool Reloading { get; set; }
		public bool IsModal { get; set; }  
		public UIImage BackgroundImage { get; set; }
		public UIColor BackgroundColor { get; set; }
		public bool CanDeleteCells { get; set; }
		public object RootView { get; set; }
		
		public ICommand PullToRefreshCommand { get; set; }
		public string RefreshKey { get; set; }

		public UITableViewStyle Style = UITableViewStyle.Grouped;

		public bool ReloadCompleted
		{
			get { return !Reloading; }
			set 
			{
				if (value)
				{
					ReloadComplete();
				}
			}
		}

		public bool EnablePullToRefresh 
		{ 
			get { return RefreshView != null; } 
			set 
			{
				if (value && RefreshView == null)
				{
					var bounds = View.Bounds;
					RefreshView = MakeRefreshTableHeaderView(new RectangleF(0, -bounds.Height, bounds.Width, bounds.Height), RefreshKey);

					RefreshView.Frame = new RectangleF(0,0, 320, 300);
					if (Reloading)
						RefreshView.SetActivity(true);

					TableView.AddSubview(RefreshView);				
				}
				else
				{
					if(RefreshView != null)
					{
						RefreshView.Dispose();
						RefreshView = null;
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

		public void TriggerRefresh(bool showStatus)
		{
			if (PullToRefreshCommand == null)
				return;
			
			if (Reloading)
				return;
			
			Reloading = true;

			if (Reloading && showStatus && RefreshView != null)
			{
				UIView.BeginAnimations("reloadingData");
				UIView.SetAnimationDuration(0.2);
				TableView.ContentInset = new UIEdgeInsets(60, 0, 0, 0);
				UIView.CommitAnimations();
			}
			
			if (RefreshView != null)
				RefreshView.SetActivity(true);
			
			Thread.Sleep(250);
			
			ThreadStart threadStart = RefreshThread;
			var refreshThread = new Thread(threadStart);
			refreshThread.Start();
		}

		private void RefreshThread()
		{
			using (var pool = new NSAutoreleasePool())
			{
				if (PullToRefreshCommand != null)
					PullToRefreshCommand.Execute(this);
	
				InvokeOnMainThread(() => ReloadComplete());
			}
		}

		/// <summary>
		/// Invoke this method to signal that a reload has completed, this will update the UI accordingly.
		/// </summary>
		public void ReloadComplete()
		{
			if (RefreshView != null)
				RefreshView.LastUpdate = DateTime.Now;
			if (!Reloading)
				return;
			
			Reloading = false;
			if (RefreshView == null)
				return;
			
			RefreshView.SetActivity(false);
			RefreshView.Flip(false);

			UIView.BeginAnimations("doneReloading");
			UIView.SetAnimationDuration(0.3f);
			TableView.ContentInset = new UIEdgeInsets(0, 0, 0, 0);
			
			RefreshView.SetStatus(RefreshStatus.PullToReload);
			
			UIView.CommitAnimations();
		}

		/// <summary>
		/// Controls whether the DialogViewController should auto rotate
		/// </summary>
		public bool Autorotate { get; set; }

		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			return (Autorotate || toInterfaceOrientation == UIInterfaceOrientation.Portrait) && toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown;
		}
		
		public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			var orientation = toInterfaceOrientation;

			if (toInterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown)
				orientation = UIInterfaceOrientation.Portrait;

			base.WillRotate(orientation, duration);
		}

		public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate(fromInterfaceOrientation);
			ConfigureBackgroundImage();
			ReloadData();
		}

		/// <summary>
		/// Allows caller to programatically activate the search bar and start the search process
		/// </summary>
		public void StartSearch()
		{		
			var searchbar = TableView.Source as ISearchBar;
			if (searchbar != null && searchbar.IsSearchbarHidden || _Searchbar == null)
			{
				TableView.ScrollToRow(NSIndexPath.FromRowSection(0, 0), UITableViewScrollPosition.Top, true);

				CreateSearchbar();
			
				UIView.BeginAnimations(null);
				UIView.SetAnimationDuration(0.3);
				
				_Searchbar.Frame = new RectangleF(0, 0, _Searchbar.Frame.Width, 45);
	
				TableView.TableHeaderView = _Searchbar;

				if (_OriginalSections == null)
				{
					_OriginalSections = new Dictionary<int, Section>(((BaseDialogViewSource)TableView.Source).Sections);
					_OriginalDataContext = new List<object>();
					foreach(var section in _OriginalSections.Values)
					{
						_OriginalDataContext.Add(section.DataContext);
					}
				}
			
				UIView.CommitAnimations();
			}
		}

		/// <summary>
		/// Allows the caller to programatically stop searching.
		/// </summary>
		public virtual void FinishSearch(bool hide)
		{
			if (_Searchbar == null) return;
							
			if (_OriginalSections != null)
			{
				var index = 0;
				foreach(var section in _OriginalSections.Values)
				{
					var list = section.DataContext as IEnumerable;
					if (list != null)
					{
						section.DataContext = _OriginalDataContext[index++] as IList;
					}
				}
				((BaseDialogViewSource)TableView.Source).Sections = _OriginalSections;
				
				_OriginalSections = null;
				ReloadData();
			}
			
			if (hide)
			{
				UIView.BeginAnimations(null);
				UIView.SetAnimationDuration(0.3);
				
				// we need to perform some post operations after the animation is complete
				UIView.SetAnimationDelegate(this);
				UIView.SetAnimationDidStopSelector(new Selector("fadeOutDidFinish"));
				
				if (_Searchbar != null)
					_Searchbar.Frame = new RectangleF(0, -45, _Searchbar.Frame.Width, 45);
				
				TableView.ContentOffset = new PointF(0, 45);
				UIView.CommitAnimations();
			}

			_Searchbar.ResignFirstResponder();
			_Searchbar.Text = string.Empty;
		}
	
		[Export("fadeOutDidFinish")]
		public void FadeOutDidFinish()
		{
			_Searchbar.Hidden = true;

			TableView.TableHeaderView = null;
			TableView.ContentOffset = PointF.Empty;
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
			
			var progress = new ProgressHud() { TitleText = string.Format("Searching for {0}", text) , GraceTime = 0.5f };
			progress.ShowWhileExecuting(()=>
			{
				var index = 0;
				foreach(var section in _OriginalSections.Values)
				{
					section.DataContext = _OriginalDataContext[index++] as IList;
				}
				
				OnSearchTextChanged(text);
				
				var newSections = new Dictionary<int, Section>();
				
				var searchable = TableView.Source as ISearchBar;
				if (searchable != null)
				{
					if (searchable.SearchCommand == null)
					{
						index = 0;
						foreach(var section in _OriginalSections.Values)
						{
							if (TableView.Source is ListSource)
							{
								var newList = new List<object>();
								var list = section.DataContext as IEnumerable;
								if (list != null)
								{
									foreach(var item in list)
									{
										var caption = item as ICaption;
										var searchableItem = item as ISearchable;
										if ((searchableItem != null && searchableItem.Matches(text)) || 
											(caption != null && !string.IsNullOrEmpty(caption.Caption)) || 
											item.ToString().ToLower().Contains(text.ToLower()))
										{
											newList.Add(item);
	
											if (!newSections.ContainsKey(index))
											{
												newSections.Add(index, section);
											}
										}
									}
								}
	
								section.DataContext = newList;
							}
	
							index++;
						}
					}
					else
					{
						newSections = searchable.SearchCommand.Execute(_OriginalSections, text);
					}
				}
				
				((BaseDialogViewSource)TableView.Source).Sections = newSections;
	
				InvokeOnMainThread(()=>ReloadData());
			}, true);
		}

		public virtual void SearchButtonClicked(string text)
		{
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
		
		public void Selected(object item, NSIndexPath indexPath)
		{
			var selectable = RootView as ISelectable;
			if (selectable != null)
			{
				selectable.Selected(this, TableView, item, indexPath);
			}
			
	//		TableView.DeselectRow(indexPath, true);
		}

		public virtual UITableView MakeTableView(RectangleF bounds, UITableViewStyle style)
		{
			var tableView = new DialogViewTable(bounds, style) { Controller = this };
			return tableView;
		}

//		public override void LoadView()
//		{
////			var themeable = Root as IThemeable; 
////			if (themeable != null)
////			{
////				if (themeable.Theme.TableViewStyle.HasValue)
////				{
////					Style = themeable.Theme.TableViewStyle.Value;
////				}
////			}
//
//			_TableView = MakeTableView(UIScreen.MainScreen.Bounds, Style);
//			TableView = _TableView;
//
//			TableView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin;
//			TableView.AutosizesSubviews = true;
//		
//			View = TableView;
//			
//			UpdateSource();
//
////			if (themeable != null)
////			{
////				var separatorColor = themeable.Theme.SeparatorColor;
////				if (separatorColor != null)
////					TableView.SeparatorColor = separatorColor;
////
////				var separatorStyle = themeable.Theme.SeparatorStyle;
////				if (separatorStyle.HasValue)
////					TableView.SeparatorStyle = separatorStyle.Value;
////			}
//
//			var pullToRefreshAttribute = RootView.GetType().GetCustomAttribute<PullToRefreshAttribute>();
//			EnablePullToRefresh = pullToRefreshAttribute != null; //|| RootView.PullToRefreshCommand
//		}
		
		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			
			var activation = TableView.Source as IActivation;
			if (activation != null)
			{
				activation.Activated();
			}			

//			if (_Dirty)
			{
				TableView.ReloadData();
				_Dirty = false;
			}
			
			TableView.BeginUpdates();
			SetScrollEnabled();
			
			ReloadData();
			TableView.EndUpdates();
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			
			ConfigureNavbarItems();
			ConfigureToolbarItems();

			var searchBar = TableView.Source as ISearchBar;
			if (searchBar != null)
			{
				if (searchBar.EnableSearch && !searchBar.IsSearchbarHidden)
				{
					StartSearch();
				}
			}
			
			if (RootView != null)
			{
//				var activation = RootView as IActivation;
//				if (activation != null)
//					activation.Activated();

			}
		}

		private void ConfigureToolbarItems()
		{
			var standardButtonSize = 31;

			if (ToolbarButtons != null)
			{
				var buttonList = ToolbarButtons.Where((button)=>button.Command == null || !(button as ICommandButton).Hidden).ToList();

				var leftCount = buttonList.Where((button)=>button.Location == BarButtonLocation.Left).Count();
				var rightCount = buttonList.Where((button)=>button.Location == BarButtonLocation.Right).Count();
				
				if (rightCount > leftCount)
				{
					var buttonWidth = buttonList.Last((button)=>button.Location == BarButtonLocation.Right).Width;
					_LeftFixedSpace.Width = buttonWidth == 0 ? standardButtonSize : buttonWidth;

					buttonList.Add(_LeftFixedSpace);
				}

				if (leftCount > rightCount)
				{
					var buttonWidth = buttonList.First((button)=>button.Location == BarButtonLocation.Left).Width;
					_RightFixedSpace.Width = buttonWidth == 0 ? standardButtonSize : buttonWidth;

					buttonList.Add(_RightFixedSpace);
				}

				CommandBarButtonItem[] buttons = buttonList.ToArray();	
				SetToolbarItems(buttons, false);
 
				var nav = ParentViewController as UINavigationController;
				if (nav != null)
				{
					nav.NavigationBar.Opaque = false;
 
					var themeable = RootView as IThemeable;
					if (themeable != null && themeable.Theme != null)
					{
						nav.Toolbar.TintColor = themeable.Theme.BarTintColor;
					}
				}
			}
		}

		private void ConfigureNavbarItems()
		{
			var nav = ParentViewController as UINavigationController;
			if (nav != null)
			{
				nav.SetToolbarHidden(ToolbarButtons == null, true);
			
				NavigationItem.RightBarButtonItem = null;
				NavigationItem.LeftBarButtonItem = null;

				if (NavbarButtons != null)
				{
					foreach (var button in NavbarButtons)
					{
						if (button.Command.CanExecute(null))
						{
							if (button.Location == BarButtonLocation.Right)
								NavigationItem.RightBarButtonItem = button;
							else if (button.Location != BarButtonLocation.Back)
								NavigationItem.LeftBarButtonItem = button;
							else
							{
								NavigationItem.LeftBarButtonItem = button;
								NavigationItem.SetHidesBackButton(true, false);
							}
						}
					}
				}
			}
		}

		private void ConfigureBackgroundImage()
		{
			if (BackgroundColor == null)
			{
				if (BackgroundImage == null && Theme != null)
				{
					if (Theme.BackgroundUri != null)
					{
						var imageUri = Theme.BackgroundUri;
						BackgroundImage = ImageLoader.DefaultRequestImage(imageUri, null);
					}
					
					if (Theme.BackgroundColor != null)
					{
						BackgroundColor = Theme.BackgroundColor;
					}
	
					if (Theme.BackgroundImage != null)
					{
						BackgroundImage = Theme.BackgroundImage;
					}
				}
		
				if (BackgroundImage != null)
				{
					BackgroundColor = UIColor.FromPatternImage(BackgroundImage);
				}
			}
				
			if (BackgroundColor != null)
			{
				if (TableView != null)
				{
					if (TableView.RespondsToSelector(new Selector("backgroundView")))
					{
						//if (TableView.BackgroundView == null)
							TableView.BackgroundView = new UIView(); 
	
						TableView.BackgroundView.Opaque = false;
						TableView.BackgroundColor = UIColor.Clear;
					}

					if (ParentViewController != null && !IsModal)
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
		}

		public virtual RefreshTableHeaderView MakeRefreshTableHeaderView(RectangleF rect, string defaultSettingsKey)
		{
			return new RefreshTableHeaderView(rect, defaultSettingsKey);
		}
		
		public bool IsSearchBarVisible
		{
			get 
			{ 
				var searchbar = TableView.Source as ISearchBar;
				if (searchbar != null)
				{
					return !searchbar.IsSearchbarHidden;
				}

				return false; 
			}
		}

		public void ToggleSearchbar()
		{
			var searchbar = TableView.Source as ISearchBar;
			if (searchbar != null)
			{
				if (!searchbar.IsSearchbarHidden)
				{
					FinishSearch(true);
			
					_Searchbar.ResignFirstResponder();
					_Searchbar.Text = string.Empty;
					searchbar.IsSearchbarHidden = true;
				}
				else
				{
					StartSearch();
					searchbar.IsSearchbarHidden = false;
					_Searchbar.BecomeFirstResponder();
				}
			}
		}
		
		private void CreateSearchbar()
		{
			if (_Searchbar == null)
			{
				var searchable = TableView.Source as ISearchBar;
				if (searchable != null)
				{
					_Searchbar = new UISearchBar(new RectangleF(0, 0, TableView.Bounds.Width, 45)) 
					{ 
						Delegate = new DialogViewSearchDelegate(this),
						TintColor = Theme.BarTintColor,
					};	

					if (!string.IsNullOrEmpty(searchable.SearchPlaceholder))
						_Searchbar.Placeholder = searchable.SearchPlaceholder;
					else 
						_Searchbar.Placeholder = "Search";
				}
			}

			var frame = _Searchbar.Frame;
			frame.Height = 45;
			_Searchbar.Frame = frame;
			_Searchbar.Hidden = false;
		}
		
		public void UpdateSource()
		{			
			TableView.ReloadData();
			ConfigureNavbarItems();
			ConfigureToolbarItems();
		}

		public void ReloadData()
		{			
			if (Theme != null)
			{
				var nav = ParentViewController as UINavigationController;
				if (nav != null)
				{
					nav.NavigationBar.Opaque = false;
	
					if (Theme.BarStyle.HasValue)
					{
						nav.NavigationBar.BarStyle = Theme.BarStyle.Value;
					}
	
					nav.NavigationBar.Translucent = Theme.BarTranslucent;
					nav.NavigationBar.TintColor = Theme.BarTintColor;

					nav.Toolbar.Translucent = Theme.BarTranslucent;
					nav.Toolbar.TintColor = Theme.BarTintColor;
				}
			
				var separatorColor = Theme.SeparatorColor;
				if (separatorColor != null)
					TableView.SeparatorColor = separatorColor;

				var separatorStyle = Theme.SeparatorStyle;
				if (separatorStyle.HasValue)
					TableView.SeparatorStyle = separatorStyle.Value;
			}

			ConfigureBackgroundImage();

			if (TableView != null)
				TableView.ReloadData();

			_Dirty = false;
		}
		
		public void ResetTheme(Theme newTheme)
		{
			this.Theme.MergeTheme(newTheme);

			var source = TableView.Source as BaseDialogViewSource;

			foreach(var cell in TableView.VisibleCells)
			{
				var listCell = cell as ComposableViewListCell;
				if (listCell != null)
				{
					listCell.Theme.MergeTheme(newTheme);
				}
			}

			foreach (var section in source.Sections.Values)
			{
				foreach (var viewList in section.Views.Values)
				{
					foreach (var view in viewList)
					{
						var themeable = view as IThemeable;
						if (themeable != null)
						{
							themeable.Theme.MergeTheme(Theme);	
						}
					}
				}
			}

			ReloadData();
		}

		public void Flip()
		{
			var oldTB = TableView;
			var parent = TableView.Superview;
			
			if (parent == null) 
				return;

			UIView.BeginAnimations("Flipper");
			UIView.SetAnimationDuration(1.25);
			UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
			
			UIView.Transition(parent, 1, UIViewAnimationOptions.TransitionFlipFromRight, () => 
			{
				TableView.RemoveFromSuperview();
				TableView = oldTB;
				parent.AddSubview(TableView);
				
			}, null);

			UIView.CommitAnimations();
		}

		public event EventHandler ViewDissapearing;
		
		public override void ViewWillDisappear(bool animated)
		{
			if (TableView != null)
			{
				var searchbar = TableView.Source as ISearchBar;
				if (searchbar != null && searchbar.EnableSearch && !searchbar.IsSearchbarHidden)
					FinishSearch(true);
			}

			base.ViewWillDisappear(animated);
			if (ViewDissapearing != null)
				ViewDissapearing(this, EventArgs.Empty);

			if (TableView != null)
			{
				var activation = TableView.Source as IActivation;
				if (activation != null)
				{
					activation.Deactivated();
				}
			}
		}
		
		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
			
			if (RootView != null)
			{
				var activation = RootView as IActivation;
				if (activation != null)
					activation.Deactivated();
			}
		}

		private void SetPushing(bool pushing)
		{
			_Pushing = pushing;
			NavigationItem.HidesBackButton = !_Pushing;
		}

		private void SetScrollEnabled()
		{
			if (DisableScrolling && TableView != null && TableView.Source != null)
			{
				var totalCells = 0;
				var numberOfSections = TableView.Source.NumberOfSections(TableView);
				for (var index = 0; index < numberOfSections; index++)
				{
					totalCells += TableView.Source.RowsInSection(TableView, index);
				}

				TableView.ScrollEnabled = TableView.VisibleCells.Length < totalCells || EnablePullToRefresh;
			}
		}

		private void CreateTableView(object view, Theme theme)
		{
			CreateTableView(view, null, theme);
		}

		private void CreateTableView(object view, MemberInfo member, Theme theme)
		{
			Theme = Theme.CreateTheme(theme);
			var themeable = view as IThemeable;
			if (themeable != null)
			{
				var themeAttribute = view.GetType().GetCustomAttribute<ThemeAttribute>();
				if (themeAttribute != null)
				{
					var viewTheme = Theme.CreateTheme(themeAttribute.ThemeType);
					themeable.Theme = viewTheme;
					var newTheme = Theme.CreateTheme(viewTheme);

					Theme = newTheme;
					themeable.Theme = Theme;
				}
			}

			using(var parser = new ViewParser())
			{
				var source = parser.Parse(this, view, member);
				
				var tableViewStyle = Theme.TableViewStyle;
				var tableStyle = source as ITableViewStyle;
				if (tableStyle != null)
				{
					tableViewStyle = tableStyle.TableViewStyle;
				}
				
				if (source != null)
				{
					_TableView = MakeTableView(UIScreen.MainScreen.Bounds, tableViewStyle);
					_TableView.Source = source;
					
					TableView = _TableView;
					DisableScrolling = view.GetType().GetCustomAttribute<DisableScrollingAttribute>() != null;
				}
			}
		}
		
		public DialogViewController(IntPtr handle): base(handle)
		{

		}
		public DialogViewController(string title, object view, Theme theme, bool pushing) : base(UITableViewStyle.Grouped)
		{
			Title = title;
			SetPushing(pushing);
			
			CreateTableView(view, theme);	
		}

		public DialogViewController(string title, object view, MemberInfo member, Theme theme, bool pushing) : base(UITableViewStyle.Grouped)
		{
			Title = title;
			SetPushing(pushing);
			
			CreateTableView(view, member, theme);	
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (BackgroundImage != null)
				{
					BackgroundImage = null;
				}

				_LeftFixedSpace.Dispose();
				_RightFixedSpace.Dispose();

				var source = TableView.Source as ViewSource;
				if (source != null)
				{
					foreach (var section in source.Sections.Values)
					{
						foreach(var listSource in section.ListSources.Values)
						{
							if (listSource != null)
							{
								listSource.Dispose();
							}
						}
					}
	
					source.Dispose();
				}

				if (_TableView != null)
				{
					_TableView.Dispose();
					_TableView = null;
				}
				
				if (_Searchbar != null)
				{
					_Searchbar.Dispose();
					_Searchbar = null;
				}
				
				if (RootView != null)
				{
					var disposable = RootView as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
						RootView = null;
					}
				}
				
				if (ToolbarButtons != null)
				{
					foreach(var button in ToolbarButtons)
					{
						button.Dispose();
					}
				}

				if (NavbarButtons != null)
				{
					foreach (var button in NavbarButtons)
					{
						button.Dispose();
					}
				}

				Theme.Dispose();
				Theme = null;
			}

			base.Dispose(disposing);
		}
	}
}
