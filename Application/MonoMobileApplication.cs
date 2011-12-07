// 
// MonoMobileApplication.cs
// 
// Author:
//   Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
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
	using System.Collections.Generic;
	using System.Linq;
	using MonoMobile.Views;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;
	
	[Register("MonoMobileApplication")]
	public class MonoMobileApplication : UIApplication
	{
		private int _NetworkActivityCount;

		public const string Version = "0.9";

		public static Type[] ViewTypes { get; private set;}

		public static UIWindow Window { get; set; }
		public static UINavigationController NavigationController { get; set; }
		public static List<object> Views { get; set; }
		public static List<DialogViewController> DialogViewControllers { get; private set;}

		public static string Title { get; set; }
		public static Action ResumeFromBackgroundAction { get; set; }

		public override bool NetworkActivityIndicatorVisible 
		{
			get { return _NetworkActivityCount > 0; }
			set 
			{ 
				if (value)
				{
					_NetworkActivityCount++;	
				}
				else
				{
					_NetworkActivityCount--;
				}

				Window.InvokeOnMainThread(() => base.NetworkActivityIndicatorVisible = _NetworkActivityCount > 0);
			}
		}

		public static DialogViewController CurrentDialogViewController 
		{ 
			get 
			{ 
				return NavigationController.ViewControllers.Count() > 0 ? NavigationController.ViewControllers.Last() as DialogViewController : null; 
			} 
		}

		public static UIViewController CurrentViewController  
		{ 
			get 
			{ 
				return CurrentDialogViewController != null ? CurrentDialogViewController as UIViewController : null;
			} 
		}

		static MonoMobileApplication()
		{
			DialogViewControllers = new List<DialogViewController>();
		}
		
		public static bool IsSearchbarVisible()
		{
			if (CurrentDialogViewController != null)
			{
				return CurrentDialogViewController.IsSearchBarVisible;
			}

			return false;
		}

		public static void ToggleSearchbar()
		{
			if (CurrentDialogViewController != null)
			{
				CurrentDialogViewController.ToggleSearchbar();
			}
		}
		
		public static void PushView(object view)
		{
			PushView(view, true);
		}

		public static void PushView(object view, bool animated)
		{
			var dvc = CreateDialogViewController(view, false, true);
		
			NavigationController.PushViewController(dvc, animated);
		}
		
		public static void PushView(object view, bool animated, bool pushing)
		{
			var dvc = CreateDialogViewController(view, false, pushing);
		
			NavigationController.PushViewController(dvc, animated);
		}

		public static void PresentModelView(object view)
		{
			PresentModelView(view, UIModalTransitionStyle.CoverVertical);
		}

		public static void PresentModelView(Type viewType)
		{
			var view = Activator.CreateInstance(viewType) as UIView;
			var intializable = view as IInitializable;
			if(intializable != null)
			{
				intializable.Initialize();
			}

			PresentModelView(view);
		}

		public static void PresentModelView(object view, UIModalTransitionStyle transistionStyle)
		{			
			var dvc = CreateDialogViewController(view, true, false);			
			dvc.ModalTransitionStyle = transistionStyle;
 
			var navController = new UINavigationController();
			navController.ViewControllers = new UIViewController[] { dvc };

			NavigationController.PresentModalViewController(navController, true);
		}

		public static void DismissModalView(bool animated)
		{
			NavigationController.DismissModalViewControllerAnimated(animated);
		}

		public static void Run(string title, Type mainViewType, string[] args)
		{
			Title = title;
			ViewTypes = new Type[] { mainViewType };

			Registrations.InitializeViewContainer();
			UIApplication.Main(args, "MonoMobileApplication", "MonoMobileAppDelegate");
		}

		public static void Run(string title, Type[] viewTypes, string[] args)
		{
			Title = title;
			ViewTypes = viewTypes;
			
			Registrations.InitializeViewContainer();
			UIApplication.Main(args, "MonoMobileApplication", "MonoMobileAppDelegate");
		}

		public static void Run(string delegateName, string[] args)
		{
			Registrations.InitializeViewContainer();
			UIApplication.Main(args, "MonoMobileApplication", delegateName);
		}

		private static DialogViewController CreateDialogViewController(object view, bool isModal, bool pushing)
		{
			Theme theme = null;
			
			if (CurrentDialogViewController != null)
			{
				theme = CurrentDialogViewController.Theme;
				if (theme == null)
					theme = new Theme();
				theme.TableViewStyle = UITableViewStyle.Grouped;
			}
			
			string title = null;
			var hasCaption = view as ICaption;
			if (hasCaption != null)
			{
				title = hasCaption.Caption;
			}
			
			if (string.IsNullOrEmpty(title))
				title = MonoMobileApplication.Title;
			
			var dvc = new DialogViewController(title, view, theme, pushing) { Autorotate = true };
			dvc.IsModal = isModal;
			
			return dvc;
		}	
	}
}
