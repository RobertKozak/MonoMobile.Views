// 
// ActionSheetView.cs
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
namespace MonoMobile.MVVM
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Linq;
	using System.Reflection;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	public class ActionSheetView : UIActionSheet
	{
		protected Dictionary<int, ICommand> CommandMap = new Dictionary<int, ICommand>();
	
		//Descendents cannot have a parameterless constructor. // Bug in MonoTouch? iOS?
		public ActionSheetView(string title) : base(title)
		{
			Prepare();
			Dismissed += HandleDismissed;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			Dismissed -= HandleDismissed;
		}

		private void HandleDismissed (object sender, UIButtonEventArgs e)
		{
			CommandMap[e.ButtonIndex].Execute(e.ButtonIndex);
		}

		public virtual void Cancel()
		{
		}
		
		public virtual void Destroy()
		{
		}
			
		protected void Prepare()
		{
			var methods = GetType().GetMethods().Where((methodInfo) => methodInfo.GetCustomAttribute<ButtonAttribute>(true) != null).ToList();
			
			var destroyMethod = methods.Where((methodInfo) => methodInfo.Name == "Destroy").SingleOrDefault();
			if (destroyMethod != null)
			{
				DestructiveButtonIndex = AddButton(GetTitle(destroyMethod));
				CommandMap.Add(DestructiveButtonIndex, new ReflectiveCommand(this, destroyMethod, null));
			}

			foreach(var method in methods)
			{							
				if (method.Name != "Cancel" && method.Name != "Destroy")
					CommandMap.Add(AddButton(GetTitle(method)), new ReflectiveCommand(this, method, null));
			}

			var cancelMethod = methods.Where((methodInfo) => methodInfo.Name == "Cancel").SingleOrDefault();
			if (cancelMethod != null)
			{
				CancelButtonIndex = AddButton(GetTitle(cancelMethod));
				CommandMap.Add(CancelButtonIndex, new ReflectiveCommand(this, cancelMethod, null));
			}
		}

		private string GetTitle(MethodInfo method)
		{
			var captionAttribute = method.GetCustomAttribute<CaptionAttribute>();
			var title = captionAttribute != null ? captionAttribute.Caption : method.Name.Capitalize();

			return title;
		}
	}
}

