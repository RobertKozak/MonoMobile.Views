//
// DialogViewController.cs: drives MonoMobile.Views
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
namespace MonoMobile.Views
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Linq;
	using System.Threading;
	using MonoMobile.Views;
	using MonoMobile.Views.Utilities;
	using MonoTouch.Foundation;
	using MonoTouch.ObjCRuntime;
	using MonoTouch.UIKit;

	public class DialogViewController : UITableViewController
	{
		private CommandBarButtonItem _LeftFixedSpace = new CommandBarButtonItem(UIBarButtonSystemItem.FixedSpace) { Location = BarButtonLocation.Left };
		private CommandBarButtonItem _RightFixedSpace = new CommandBarButtonItem(UIBarButtonSystemItem.FixedSpace) { Location = BarButtonLocation.Right };

		private UITableView _TableView;

		private IRoot _Root;
		private bool _Pushing;
		private bool _Dirty;
		
		private UISearchBar _Searchbar;
		private ISection[] _OriginalSections;
		private IElement[][] _OriginalElements;
		private DialogViewDataSource _TableSource;
		
		public RefreshTableHeaderView RefreshView { get; set; }
		public bool Reloading { get; set; }
		public bool IsModal { get; set; }  
		public UIImage BackgroundImage { get; set; }
		public UIColor BackgroundColor { get; set; }
		public bool CanDeleteCells { get; set; }
		public UIView RootView { get; set; }

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

				var view = RootView as IView;
				if (view != null)
					view.TableView = TableView;

				ReloadData();
			}
		}

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
					RefreshView = MakeRefreshTableHeaderView(new RectangleF(0, -bounds.Height, bounds.Width, bounds.Height), Root.DefaultSettingsKey);

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
			if (Root == null && Root.PullToRefreshCommand == null)
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
			ReloadData();
			ConfigureBackgroundImage();
		}

		/// <summary>
		/// Allows caller to programatically activate the search bar and start the search process
		/// </summary>
		public void StartSearch()
		{		
			var searchbar = Root as ISearchBar;
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
					_OriginalSections = Root.Sections.ToArray();
					_OriginalElements = new IElement[_OriginalSections.Length][];
		
					for (int i = 0; i < _OriginalSections.Length; i++)
						_OriginalElements[i] = _OriginalSections[i].Elements.ToArray();
				}
			
				UIView.CommitAnimations();
			}
		}

		/// <summary>
		/// Allows the caller to programatically stop searching.
		/// </summary>
		public virtual void FinishSearch(bool hide)
		{
			if (_OriginalSections != null)
			{
				Root.Sections = new List<ISection>(_OriginalSections);
				_OriginalSections = null;
				_OriginalElements = null;
				
				ReloadData();
			}
			
			if (hide)
			{
				UIView.BeginAnimations(null);
				UIView.SetAnimationDuration(0.3);
				
				// we need to perform some post operations after the animation is complete
				UIView.SetAnimationDelegate(this);
				UIView.SetAnimationDidStopSelector(new Selector("fadeOutDidFinish"));
				
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
			
			OnSearchTextChanged(text);
			
			var newSections = new List<ISection>();
			
			var searchable = Root as ISearchBar;
			if (searchable != null)
			{
				if (searchable.SearchCommand == null)
				{
					for (int sidx = 0; sidx < _OriginalSections.Length; sidx++)
					{
						ISection newSection = null;
						var section = _OriginalSections[sidx];
						IElement[] elements = _OriginalElements[sidx];
						
						for (int eidx = 0; eidx < elements.Length; eidx++)
						{
							var searchableView = elements[eidx] as ISearchable;
							
							if ((searchableView != null && searchableView.Matches(text)) || (elements[eidx].Caption != null) && elements[eidx].Caption.Contains(text))
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
					newSections = searchable.SearchCommand.Execute(_OriginalSections, text);
				}
			}
			
			Root.Sections = newSections;

			ReloadData();
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

		public void Selected(NSIndexPath indexPath)
		{
			var element = _TableSource.GetElement(indexPath);

			if (element != null)
			{
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
			else
			{
				TableView.DeselectRow(indexPath, true);
				var selectable = RootView as ISelectable;
				if(selectable != null)
					selectable.Selected(this, TableView, indexPath);
			}
		}

		public virtual UITableView MakeTableView(RectangleF bounds, UITableViewStyle style)
		{
			return new DialogViewTable(bounds, style) { Controller = this };
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
		
			Root.TableView = TableView;
			View = TableView;

			if (Root == null)
				return;
			
			UpdateSource();

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
		
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			
			ConfigureNavbarItems();
			ConfigureToolbarItems();

			var searchBar = Root as ISearchBar;
			if (searchBar != null)
			{
				if (searchBar.EnableSearch && !searchBar.IsSearchbarHidden)
				{
					StartSearch();
				}
			}
		}

		private void ConfigureToolbarItems()
		{
			var standardButtonSize = 31;

			if (Root != null && Root.ToolbarButtons != null)
			{
				var buttonList = Root.ToolbarButtons.Where((button)=>button.Command == null || button.Command.CanExecute(null)).ToList();

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
 
					var themeable = Root as IThemeable;
					if (themeable != null)
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
				nav.SetToolbarHidden(ToolbarItems == null, true);
				
				if (Root.NavbarButtons != null)
				{
					foreach (var button in Root.NavbarButtons)
					{
						if (button.Command.CanExecute(null))
						{
							if (button.Location == BarButtonLocation.Right)
								NavigationItem.RightBarButtonItem = button;
							else
								NavigationItem.LeftBarButtonItem = button;
						}
						else 
						{
							if (button.Location == BarButtonLocation.Right)
								NavigationItem.RightBarButtonItem = null;
							else
								NavigationItem.LeftBarButtonItem = null;
						}
					}
				}
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

		public virtual RefreshTableHeaderView MakeRefreshTableHeaderView(RectangleF rect, string defaultSettingsKey)
		{
			return new RefreshTableHeaderView(rect, defaultSettingsKey);
		}
		
		public void ToggleSearchbar()
		{
			var searchbar = Root as ISearchBar;
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
				var searchable = Root as ISearchBar;
				if (searchable != null)
				{
					_Searchbar = new UISearchBar(new RectangleF(0, 0, TableView.Bounds.Width, 45)) 
					{ 
						Delegate = new DialogViewSearchDelegate(this),
						TintColor = Root.Theme.BarTintColor,
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
		
		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			
			if (Root == null)
				return;
			
			Root.Prepare();
			
			NavigationItem.HidesBackButton = !_Pushing;

			if (Root.Caption != null)
				NavigationItem.Title = Root.Caption;

			if (_Dirty)
			{
				TableView.ReloadData();
				_Dirty = false;
			}
			
			if (Root != null)
			{
				var index = Root.Index;
				if (index > -1)
				{
					var path = Root.PathForRadio();
					if (path != null)
						TableView.ScrollToRow(path, UITableViewScrollPosition.Top, false);
				}
			}
			
			var nav = ParentViewController as UINavigationController;
			if (nav != null)
			{
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
					nav.NavigationBar.TintColor = themeable.Theme.BarTintColor;
				}
			}

			ConfigureBackgroundImage();
		}
		
		public virtual DialogViewDataSource CreateSizingSource(bool unevenRows)
		{
//			if (unevenRows)
//				return new DialogViewListDataSource(this);
//			else
//				return new DialogViewListDataSource(this);

			return new DialogViewDataSource(this);
		}

		public void UpdateSource()
		{
			if (Root == null)
				return;
			
			if (_TableSource != null)
				_TableSource.Dispose();

			_TableSource = CreateSizingSource(Root.UnevenRows);
			TableView.Source = _TableSource;
			
			ConfigureNavbarItems();
			ConfigureToolbarItems();
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
		
		public void Flip()
		{
			var oldTB = TableView;
			var parent = TableView.Superview;
			
			if (parent == null) 
				return;

			UIView.BeginAnimations("Flipper");
			UIView.SetAnimationDuration(1.25);
			UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
			
			UIView.Transition(parent, 1, UIViewAnimationOptions.TransitionFlipFromRight, delegate 
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
			var searchbar = Root as ISearchBar;
			if (searchbar != null && searchbar.EnableSearch && !searchbar.IsSearchbarHidden)
				FinishSearch(true);

			base.ViewWillDisappear(animated);
			if (ViewDissapearing != null)
				ViewDissapearing(this, EventArgs.Empty);
		}
		
		public override void ViewDidDisappear(bool animated)
		{
			foreach(var section in Root.Sections)
			{
				foreach(var element in section.Elements)
				{
				//	if (!(element is IRoot))
				//		BindingOperations.ClearBindings(element);
				}
			}

			base.ViewDidDisappear(animated);
		}

		protected void PrepareRoot(IRoot root)
		{
			Root = root;
			if (Root != null)
				Root.Prepare();
		}
		
		public DialogViewController(string title, UIView view, UITableViewStyle style, bool pushing) : base(style)
		{
			RootView = view;
			
			var bindingContext = new BindingContext(RootView, title);
			_Pushing = pushing;
			PrepareRoot(bindingContext.Root);
		}
		
		public DialogViewController(string title, UIView view, bool pushing) : base(UITableViewStyle.Grouped)
		{
			RootView = view;

			var bindingContext = new BindingContext(RootView, title);
			_Pushing = pushing;
			
			PrepareRoot(bindingContext.Root);
		}

		public DialogViewController(UIView view, bool pushing) : base(UITableViewStyle.Grouped)
		{
			RootView = view;
			
			var title = string.Empty;
			var vw = RootView as IView;
			if (vw != null)
				title = vw.Caption;

			var bindingContext = new BindingContext(RootView, title);
			_Pushing = pushing;
			PrepareRoot(bindingContext.Root);
		}

		/// <summary>
		///    Creates a new DialogViewController from a IRoot and sets the push status
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
			_Pushing = pushing;
			PrepareRoot(root);
		}

		public DialogViewController(UITableViewStyle style, IRoot root, bool pushing) : base(style)
		{
			_Pushing = pushing;
			Style = style;
			PrepareRoot(root);
		}
	}
}
