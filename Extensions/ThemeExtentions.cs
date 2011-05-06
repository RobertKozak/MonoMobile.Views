// 
//  ThemeHelper.cs
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
	using MonoTouch.UIKit;
	using System.Reflection;

	public static class ThemeHelper
	{
		public static void ApplyRootTheme(object view, IThemeable element)
		{
			Theme theme = null;
			var themeAttributes = view.GetType().GetCustomAttributes(typeof(ThemeAttribute), true);
			
			foreach (ThemeAttribute themeAttribute in themeAttributes)
			{
				if (element != null)
				{
					if (themeAttribute != null && themeAttribute.ThemeType != null)
					{
						theme = Activator.CreateInstance(themeAttribute.ThemeType) as Theme;
						element.Theme.MergeTheme(theme);
					}
				}
			}
			
			var root = element as IRoot;
			if (root != null)
				root.Theme = element.Theme;
		}

		public static void ApplyElementTheme(Theme theme, IThemeable element, MemberInfo member)
		{
			if (theme != null)
			{
				var newTheme = Theme.CreateTheme(theme);
				newTheme.MergeTheme(element.Theme);
				
				element.Theme = newTheme;
			}
			
			if (member != null)
				ApplyMemberTheme(member, element);
		}

		public static void ApplyMemberTheme(MemberInfo member, IThemeable themeableElement)
		{
			var themeAttribute = member.GetCustomAttribute<ThemeAttribute>();
			
			if (themeableElement != null && themeAttribute != null && themeAttribute.ThemeType != null)
			{
				var theme = Activator.CreateInstance(themeAttribute.ThemeType) as Theme;
				
				if (themeAttribute.ThemeUsage == ThemeUsage.Merge)
					themeableElement.Theme.MergeTheme(theme);
				else
					themeableElement.Theme = theme;
			}
		}

	}
}

