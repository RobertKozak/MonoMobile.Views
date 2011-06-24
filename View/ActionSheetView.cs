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
		private UIActionSheet _ActualActionSheet;

		protected Dictionary<int, ICommand> CommandMap = new Dictionary<int, ICommand>();
	
		//Descendents cannot have a parameterless constructor. // Bug in MonoTouch? iOS?
		public ActionSheetView(string title) : base(title)
		{
		}
		
		public override void ShowFrom(RectangleF rect, UIView inView, bool animated)
		{
			Prepare();
			_ActualActionSheet.ShowFrom(rect, inView, animated);
		}
		
		public override void ShowFrom(UIBarButtonItem item, bool animated)
		{
			Prepare();
			_ActualActionSheet.ShowFrom(item, animated);
		}

		public override void ShowFromTabBar(UITabBar view)
		{
			Prepare();
			_ActualActionSheet.ShowFromTabBar(view);
		}

		public override void ShowFromToolbar(UIToolbar view)
		{
			Prepare();
			_ActualActionSheet.ShowFromToolbar(view);
		}

		public override void ShowInView(UIView view)
		{
			Prepare();
			_ActualActionSheet.ShowInView(view);
		}

		protected override void Dispose(bool disposing)
		{
			_ActualActionSheet.Dismissed -= HandleDismissed;
			_ActualActionSheet.Dispose();

			base.Dispose(disposing);
		}

		private void HandleDismissed(object sender, UIButtonEventArgs e)
		{
			CommandMap[e.ButtonIndex].Execute(e.ButtonIndex);
			DismissWithClickedButtonIndex(e.ButtonIndex, false);
		}

		public virtual void Cancel()
		{
		}
		
		public virtual void Destroy()
		{
		}
			
		protected void Prepare()
		{
			CommandMap.Clear();

			_ActualActionSheet = new UIActionSheet(Title);
			_ActualActionSheet.Dismissed += HandleDismissed;

			var methods = GetType().GetMethods().Where((methodInfo) => methodInfo.GetCustomAttribute<ButtonAttribute>(true) != null).ToList();
			
			ICommand command = null;
			var destroyMethod = methods.Where((methodInfo) => methodInfo.Name == "Destroy").SingleOrDefault();
			if (destroyMethod != null)
			{
				command = ViewParser.GetCommandForMember(this, destroyMethod);
				var destroyIndex = AddButtonToSheet(GetTitle(destroyMethod), command);
				if (destroyIndex > -1)
					_ActualActionSheet.DestructiveButtonIndex = destroyIndex;
			}

			foreach(var method in methods)
			{							
				if (method.Name != "Cancel" && method.Name != "Destroy")
				{
					command = ViewParser.GetCommandForMember(this, method);
					AddButtonToSheet(GetTitle(method), command);
				}
			}

			var cancelMethod = methods.Where((methodInfo) => methodInfo.Name == "Cancel").SingleOrDefault();
			if (cancelMethod != null)
			{
				command = ViewParser.GetCommandForMember(this, cancelMethod);
				var cancelIndex = AddButtonToSheet(GetTitle(cancelMethod), command);
				if (cancelIndex > -1)
					_ActualActionSheet.CancelButtonIndex = cancelIndex;
			}
		}

		private string GetTitle(MethodInfo method)
		{
			var captionAttribute = method.GetCustomAttribute<CaptionAttribute>();
			var title = captionAttribute != null ? captionAttribute.Caption : method.Name.Capitalize();

			return title;
		}

		private int AddButtonToSheet(string title, ICommand command)
		{
			if (command.CanExecute(null))
			{
				var index = _ActualActionSheet.AddButton(title);
				CommandMap.Add(index, command); 
				return index;
			}

			return -1;
		}
	}
}

