// 
//  ProgressAttribute.cs
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
	using MonoTouch.UIKit;
	
	public enum IndicatorStyle 
	{
		Hud,
		AccessoryIndicator
	}

	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public class ProgressAttribute : Attribute
	{
		public ProgressAttribute(): this(null, null)
		{
		}

		public ProgressAttribute(string canExecutePropertyName): this(null, null)
		{
			CanExecutePropertyName = canExecutePropertyName;
			CommandOption = CommandOption.Hide;
		}

		public ProgressAttribute(string title, string detailText)
		{
			Title = title;
			DetailText = detailText;
			
			IndicatorStyle = IndicatorStyle.Hud;
		}

		internal string CanExecutePropertyName;
		public CommandOption CommandOption;
		public string Title { get; set; }
		public string DetailText { get; set;}
		public IndicatorStyle IndicatorStyle { get; set; }
	}
}

