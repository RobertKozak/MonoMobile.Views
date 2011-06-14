// 
//  MonoMobileApplication.cs
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
namespace MonoMobile.MVVM
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using MonoMobile.MVVM;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;
	
	[Register("MonoMobileApplication")]
	public class MonoMobileApplication : UIApplication
	{
		public const string Version = "0.5";

		public static Type[] ViewTypes { get; private set;}

		public static UIWindow Window { get; set; }
		public static UINavigationController NavigationController { get; set; }
		public static DialogViewController CurrentDialogViewController { get; set; }
		public static UIViewController CurrentViewController { get; set; }
		public static List<UIView> Views { get; set; }
		public static List<DialogViewController> DialogViewControllers { get; private set;}

		public static string Title { get; set; }
 
		public static Action ResumeFromBackgroundAction { get; set; }

		static MonoMobileApplication()
		{
			DialogViewControllers = new List<DialogViewController>();
		}
		
		public static void ToggleSearchbar()
		{
			if (CurrentDialogViewController != null)
			{
				CurrentDialogViewController.ToggleSearchbar();
			}
		}
		
		public static DialogViewController CreateDialogViewController(UIView view)
		{
			Theme theme = null;
			
			if (CurrentDialogViewController != null)
			{
				theme = CurrentDialogViewController.Root.Theme;
				theme.TableViewStyle = UITableViewStyle.Grouped;
			}
		
			var dvc = new DialogViewController(MonoMobileApplication.Title, view, true) { Autorotate = true };
			dvc.IsModal = true;

			return dvc;
		}

		public static void PushView(UIView view)
		{
			PushView(view, true);
		}

		public static void PushView(UIView view, bool animated)
		{
			var controller = CreateDialogViewController(view);
			NavigationController.PushViewController(controller, animated);
		}

		public static void PresentModelView(UIView view)
		{
			//NavigationController.PopToRootViewController(false);
			PresentModelView(view, UIModalTransitionStyle.CoverVertical);
		}

		public static void PresentModelView(UIView view, UIModalTransitionStyle transistionStyle)
		{			
			var controller = CreateDialogViewController(view);
			
			controller.ModalTransitionStyle = transistionStyle;

			var navigation = new UINavigationController();
			navigation.ViewControllers = new UIViewController[] { controller };
			
			NavigationController.PresentModalViewController(navigation, true);
		}

		public static void DismissModalView(bool animated)
		{
			NavigationController.DismissModalViewControllerAnimated(animated);
		}

		public static void Run(string title, Type mainViewType, string[] args)
		{
			Title = title;
			ViewTypes = new Type[] { mainViewType };
			UIApplication.Main(args, "MonoMobileApplication", "MonoMobileAppDelegate");
		}

		public static void Run(string title, Type[] viewTypes, string[] args)
		{
			Title = title;
			ViewTypes = viewTypes;
			UIApplication.Main(args, "MonoMobileApplication", "MonoMobileAppDelegate");
		}

		public static void Run(string delegateName, string[] args)
		{
			UIApplication.Main(args, "MonoMobileApplication", delegateName);
		}
	}
}
