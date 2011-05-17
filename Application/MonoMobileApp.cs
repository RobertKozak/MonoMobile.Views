// 
//  MonoMobileApp.cs
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
	using System.Linq;
	using MonoTouch.Foundation;
	using MonoMobile.MVVM;
	using MonoTouch.UIKit;
	
	[Register("MonoMobileApplication")]
	public class MonoMobileApplication: UIApplication
	{
		public static Type MainViewType { get; private set; }  

		public static UIWindow Window { get; set; }
		public static UINavigationController Navigation { get; set; }
		public static UIView MainView { get; set; }
		public static string Title { get; set; }
 
		public static void ToggleSearchbar()
		{
			var dvc = (DialogViewController)Navigation.ViewControllers.FirstOrDefault();
			if (dvc != null)
			{
				dvc.ToggleSearchbar();
			}
		}
		
		public static void Run(string title, Type mainViewType, string[] args)
		{
			Title = title;
			MainViewType = mainViewType;
			UIApplication.Main(args, "MonoMobileApplication", "MonoMobileAppDelegate");
		}
	}
}
