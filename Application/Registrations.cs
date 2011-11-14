// 
//  Registrations.cs
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
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;
	using MonoTouch.CoreLocation;
	using MonoTouch.UIKit;

	public static class Registrations
	{
		public static void InitializeViewContainer()
		{
			ViewContainer.RegisterView(typeof(object), typeof(ObjectView));
			ViewContainer.RegisterView(typeof(string), typeof(StringView));
			ViewContainer.RegisterView(typeof(int), typeof(StringView));
			ViewContainer.RegisterView(typeof(float), typeof(StringView));
			ViewContainer.RegisterView(typeof(bool), typeof(BooleanView));	
			ViewContainer.RegisterView(typeof(Enum), typeof(ListView));
			ViewContainer.RegisterView(typeof(IEnumerable), typeof(ListView));

			ViewContainer.RegisterView(typeof(UIView), typeof(ObjectView));
			ViewContainer.RegisterView(typeof(MethodInfo), typeof(MethodView));
			ViewContainer.RegisterView(typeof(ICommand), typeof(CommandView));
			ViewContainer.RegisterView(typeof(Uri), typeof(HtmlView));
			ViewContainer.RegisterView(typeof(HtmlAttribute), typeof(HtmlView));
			ViewContainer.RegisterView(typeof(EntryAttribute), typeof(EntryView));
			ViewContainer.RegisterView(typeof(PasswordAttribute), typeof(EntryView));
	//		ViewContainer.RegisterView(typeof(NavigateToViewAttribute), typeof(List));
			ViewContainer.RegisterView(typeof(DateTime), typeof(DateView));
			ViewContainer.RegisterView(typeof(DateAttribute), typeof(DateView));
			ViewContainer.RegisterView(typeof(TimeAttribute), typeof(TimeView));
			
			ViewContainer.RegisterView(typeof(LoadMoreAttribute), typeof(LoadMoreView));
			ViewContainer.RegisterView(typeof(CLLocationCoordinate2D), typeof(MapView));
			ViewContainer.RegisterView(typeof(IEnumerable<CLLocationCoordinate2D>), typeof(MapView));
			ViewContainer.RegisterView(typeof(MapAttribute), typeof(MapView));
			ViewContainer.RegisterView(typeof(RangeAttribute), typeof(SliderView));
			ViewContainer.RegisterView(typeof(ReadOnlyAttribute), typeof(EntryView));

			ViewContainer.RegisterView(typeof(IEnumerable<object>), typeof(ListView));

		//	ViewContainer.RegisterView(typeof(ReadOnlyAttribute), typeof(EntryView));
		}
	}
}

