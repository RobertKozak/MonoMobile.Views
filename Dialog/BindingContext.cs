//
// BindingContext.cs
//
// Author:
//   Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak
//
// Copyright 2011, Nowcom Corporation
//
// Based on code from MonoTouch.Dialog by Miguel de Icaza (miguel@gnome.org)
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
	using System.Collections;
	using System.Collections.Generic;
	using MonoMobile.MVVM;
	using MonoTouch.UIKit;

	public class BindingContext : DisposableObject
	{
		private static Dictionary<DataContextCode, Func<IRoot, IRoot>> RootBuilder = new Dictionary<DataContextCode, Func<IRoot, IRoot>>()
		{
			{ DataContextCode.Enum, (root) => { return null; } },
			{ DataContextCode.EnumCollection, (root) => { return null; } },
			{ DataContextCode.MultiselectCollection, (root) => { return null; } },
			{ DataContextCode.Enumerable, (root) => { return null; } },
			{ DataContextCode.Object, (root) => { return CreateObjectRoot(root); } },
			{ DataContextCode.ViewEnumerable, (root) => { return CreateViewEnumerableRoot(root); } }
		};

		public IRoot Root { get; set; }
	
		public BindingContext(UIView view, string title) : this(view, title, null)
		{
		}

		public BindingContext(UIView view, string title, Theme currentTheme)
		{
			if (view == null)
				throw new ArgumentNullException("view");
			
			var parser = new ViewParser();
			parser.Parse(view, title, currentTheme);
			Root = parser.Root;

			var viewContext = view as IBindingContext;
			if (viewContext != null)
			{
				viewContext.BindingContext = this;
				var dataContext = view as IDataContext;
				if (dataContext != null)
				{
					var vmContext = dataContext.DataContext as IBindingContext;
					if (vmContext != null)
					{
						vmContext.BindingContext = this;
					}
				}
			}

			if (view is IView)
			{
				((IView)view).TableView = Root.TableView;
			}
		}
		
		public static IRoot CreateRootedView(IRoot root)
		{
			IRoot newRoot = null;

			if (root.ViewBinding != null)
			{
				newRoot = RootBuilder[root.ViewBinding.DataContextCode](root);
			}

			return newRoot;
		}
		
		public void Fetch()
		{
			foreach(var section in Root.Sections)
			{
				foreach(var element in section)
				{
					var bindingExpressions = BindingOperations.GetBindingExpressionsForElement(element); 
					foreach(var expression in bindingExpressions)
					{
						expression.UpdateSource();
					}
				}
			}
		}
		
		public static Section CreateSection(IRoot root, Section section, object dataContext, Type elementType, bool popOnSelection)
		{
			var items = dataContext as IEnumerable;
			if (items != null)
			{
				if (section == null)
					section = new Section();

				section.Clear();

				foreach (var item in items)
				{
					var caption = item.ToString();
					if (string.IsNullOrEmpty(caption))
					{
						caption = root.ViewBinding.ViewType.Name.Capitalize();
					}
					
					if (elementType == null)
						elementType = typeof(RadioElement);

					var element = Activator.CreateInstance(elementType) as IElement;
					element.Caption = caption;
					element.DataContext = item;
					element.ViewBinding.DataContextCode = DataContextCode.Object;

					if (root != null)
					{
						element.ViewBinding.ViewType = root.ViewBinding.ViewType;
						element.ViewBinding.MemberInfo = root.ViewBinding.MemberInfo;
						element.EditingStyle = root.EditingStyle;
						element.Theme.MergeTheme(root.Theme);
					}

					var uiView = element as UIView;
					if (uiView != null)
						uiView.Opaque = false;

					if (item is UIView)
						element.ViewBinding.View = item as UIView;
					else
						element.DataContext = item;
	
					if (element.ViewBinding.ViewType == null)
						element.ViewBinding.ViewType = item.GetType();
	
					section.Add(element);
				}
			}

			return section;
		}

		public static IRoot CreateViewEnumerableRoot(IRoot root)
		{
			IRoot newRoot = null;

			Type elementType = root.ViewBinding.ElementType;
			if (elementType == null)
				elementType = typeof(RootElement);

			newRoot = new RootElement() { Opaque = false, ViewBinding = root.ViewBinding, EditingStyle = root.EditingStyle };
			
			var section = new Section() { Opaque = false, ViewBinding = root.ViewBinding, Parent = newRoot as IElement };

			newRoot.Sections.Add(section);
			
			section = CreateSection(newRoot, section, root.DataContext, elementType, false);
		
			ThemeHelper.ApplyElementTheme(root.Theme, newRoot, null);

			return newRoot;
		}
		
		private static IRoot CreateObjectRoot(IRoot root)
		{
			IRoot newRoot = null;
			UIView view = null;
			object context = root.DataContext;

			try
			{				
				if (root.ViewBinding.ViewType != null)
				{
					view = Activator.CreateInstance(root.ViewBinding.ViewType) as UIView;
				}

				var dataContext = view as IDataContext;
				if (dataContext != null)
				{
					dataContext.DataContext = context;

					var lifetime = context as IInitializable;
					if (lifetime != null)
						lifetime.Initialize();
					
					lifetime = dataContext as IInitializable;
					if (lifetime != null)
						lifetime.Initialize();
				}
				
				var bindingContext = new BindingContext(view, root.Caption, root.Theme);

				newRoot = (IRoot)bindingContext.Root;
				newRoot.ViewBinding = root.ViewBinding;
				newRoot.ViewBinding.View = view;
			}
			catch(MissingMethodException)
			{
			}
			
			return newRoot;
		}
 
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
			
			base.Dispose(disposing);
		}
	}
}
