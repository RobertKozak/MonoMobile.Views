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
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;
	
	[Preserve(AllMembers = true)]
	[AttributeUsage(AttributeTargets.Class)]
	public class InitializeAttribute : Attribute
	{
	}
	
	[Preserve(AllMembers = true)]
	[Initialize]
	public static class Registrations
	{
		public static void InitializeViewContainer()
		{
			//Integral types
			ViewContainer.RegisterView(typeof(sbyte), typeof(StringCellView));			
			ViewContainer.RegisterView(typeof(byte), typeof(StringCellView));			
			ViewContainer.RegisterView(typeof(char), typeof(StringCellView));			
			ViewContainer.RegisterView(typeof(short), typeof(StringCellView));
			ViewContainer.RegisterView(typeof(ushort), typeof(StringCellView));			
			ViewContainer.RegisterView(typeof(int), typeof(StringCellView));			
			ViewContainer.RegisterView(typeof(uint), typeof(StringCellView));			
			ViewContainer.RegisterView(typeof(long), typeof(StringCellView));
			ViewContainer.RegisterView(typeof(ulong), typeof(StringCellView));
			
			//float types
			ViewContainer.RegisterView(typeof(float), typeof(StringCellView));
			ViewContainer.RegisterView(typeof(double), typeof(StringCellView));
			ViewContainer.RegisterView(typeof(decimal), typeof(StringCellView));

			//bool
			ViewContainer.RegisterView(typeof(bool), typeof(BooleanCellView));			

			//Reference types
			ViewContainer.RegisterView(typeof(object), typeof(ObjectCellView<object>));
			ViewContainer.RegisterView(typeof(string), typeof(StringCellView));

			//Enum
			ViewContainer.RegisterView(typeof(Enum), typeof(ListCellView));

			//IEnumerable
			ViewContainer.RegisterView(typeof(IEnumerable), typeof(ListCellView));
			ViewContainer.RegisterView(typeof(IEnumerable<object>), typeof(ListCellView));

			//Other 
			ViewContainer.RegisterView(typeof(UIView), typeof(ObjectCellView<UIView>));
			ViewContainer.RegisterView(typeof(ICommand), typeof(CommandCellView));
			ViewContainer.RegisterView(typeof(DateTime), typeof(DateCellView));
			ViewContainer.RegisterView(typeof(NSDate), typeof(DateCellView));
		}

		public static void Initialize()
		{
			InitializeViewContainer();
		}
	}
}

