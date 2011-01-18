//
// Element.cs: 
//
// Author:
//   Miguel de Icaza (miguel@gnome.org)
//
// Copyright 2010, Novell, Inc.
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
namespace MonoTouch.Dialog
{
	using System;
	using System.Collections.Generic;
	using MonoTouch.Foundation;
	using MonoTouch.MVVM;
	using MonoTouch.UIKit;

	/// <summary>
	/// Base class for all elements in MonoTouch.Dialog
	/// </summary>
	public class Element : DisposableObject, IBindable
	{
		public UITableViewElementCell Cell { get; set; }

		public UILabel TextLabel {get; set;}

		public NSString Id { get; set; }
		public int Order { get; set; }
		public int Index { get; set; }

		/// <summary>
		///  Handle to the container object.
		/// </summary>
		/// <remarks>
		/// For sections this points to a IRoot, for every
		/// other object this points to a Section and it is null
		/// for the root IRoot.
		/// </remarks>
		public Element Parent { get; set; }

		/// <summary>
		///  The caption to display for this given element
		/// </summary>
		private string _Caption;
		public string Caption 
		{ 
			get {return _Caption;} 
			set {_Caption = value;}
		}

		/// <summary>
		///  Initializes the element with the given caption.
		/// </summary>
		/// <param name="caption">
		/// The caption.
		/// </param>
		public Element(string caption)
		{
			Id = new NSString(GetType().FullName);
			Caption = caption;
		}

		protected override void Dispose(bool disposing)
		{
		}
		public virtual UITableViewElementCell NewCell()
		{
			var cell = new UITableViewElementCell(UITableViewCellStyle.Default, Id);
			cell.SelectionStyle = UITableViewCellSelectionStyle.None;

			return cell;
		}

		public virtual UITableViewElementCell GetCell(UITableView tableView)
		{
			Cell = tableView.DequeueReusableCell(Id) as UITableViewElementCell;
			if (Cell == null)
			{
				Cell = NewCell();
			}
			if (Cell.Element != this)
				InitializeCell(tableView);

			Cell.Element = this;
			TextLabel = Cell.TextLabel;
			UpdateTargets();

			return Cell;
		}

		public virtual void InitializeControls(UITableView tableView)
		{
		}

		public virtual void InitializeCell(UITableView tableView)
		{
			InitializeControls(tableView);
		}

		public IBindingExpression SetBinding(IBindable target, string targetProperty, Binding binding)
		{
			return BindingOperations.SetBinding(target, targetProperty, binding);
		}

		public IBindingExpression SetBinding(string targetProperty, Binding binding)
		{
			return SetBinding(this, targetProperty, binding);
		}

		public IBindingExpression GetBindingExpression(string targetProperty)
		{
			return BindingOperations.GetBindingExpression(this, targetProperty);
		}

		protected virtual void UpdateTarget(string property)
		{
			var bindingExpression = BindingOperations.GetBindingExpression(this, property);
			
			if (bindingExpression != null)
			{
				bindingExpression.UpdateTarget();
			}
		}

		protected virtual void UpdateSource(string property)
		{
			var bindingExpression = BindingOperations.GetBindingExpression(this, property);
			
			if (bindingExpression != null)
			{
				bindingExpression.UpdateSource();
			}
		}

		protected virtual void UpdateTargets()
		{
			var bindingExpressions = BindingOperations.GetBindingExpressionsForElement(this);
			if(bindingExpressions != null)
			{
				foreach (var bindingExpression in bindingExpressions)
				{
					bindingExpression.UpdateTarget();
				}
			}
		}

		protected virtual void UpdateSources()
		{
			var bindingExpressions = BindingOperations.GetBindingExpressionsForElement(this);
			if (bindingExpressions != null)
			{
				foreach (var bindingExpression in bindingExpressions)
				{
					bindingExpression.UpdateSource();
				}
			}
		}

		protected void RemoveTag(int tag)
		{
			var viewToRemove = Cell.ContentView.ViewWithTag(tag);
			if (viewToRemove != null)
			{
				viewToRemove.RemoveFromSuperview();
			}
		}

		/// <summary>
		/// Returns a summary of the value represented by this object, suitable 
		/// for rendering as the result of a IRoot with child objects.
		/// </summary>
		/// <returns>
		/// The return value must be a short description of the value.
		/// </returns>
		public override string ToString()
		{
			return "";
		}

		/// <summary>
		/// Invoked when the given element has been tapped by the user.
		/// </summary>
		/// <param name="dvc">
		/// The <see cref="DialogViewController"/> where the selection took place
		/// </param>
		/// <param name="tableView">
		/// The <see cref="UITableView"/> that contains the element.
		/// </param>
		/// <param name="path">
		/// The <see cref="NSIndexPath"/> that contains the Section and Row for the element.
		/// </param>
		public virtual void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
		}

		/// <summary>
		///  Returns the IndexPath of a given element.   This is only valid for leaf elements,
		///  it does not work for a toplevel IRoot or a Section of if the Element has
		///  not been attached yet.
		/// </summary>
		public NSIndexPath IndexPath
		{
			get {
				var section = Parent as ISection;
				if (section == null)
					return null;
				var root = section.Parent as IRoot;
				if (root == null)
					return null;
				
				int row = 0;
				foreach (var element in section.Elements)
				{
					if (element == this)
					{
						int nsect = 0;
						foreach (var sect in root.Sections)
						{
							if (section == sect)
							{
								return NSIndexPath.FromRowSection(row, nsect);
							}
							nsect++;
						}
					}
					row++;
				}
				return null;
			}
		}

		/// <summary>
		///   Method invoked to determine if the cell matches the given text, never invoked with a null value or an empty string.
		/// </summary>
		public virtual bool Matches(string text)
		{
			if (Caption == null)
				return false;
			return Caption.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) != -1;
		}
	}

	public abstract class Element<T> : Element
	{
		public event EventHandler ValueChanged;

		private T _Value;
		public virtual T Value
		{
			get { return _Value; }
			set { SetValue(value); }
		}

		public Element(string caption) : base(caption)
		{
		}

		protected virtual void SetValue(T value)
		{
			if (!EqualityComparer<T>.Default.Equals(_Value, value))
			{
				_Value = value;

				if (string.IsNullOrEmpty(Caption) && value != null)
				{ 
					Caption = value.ToString();
				}
 
				UpdateSources();
				OnValueChanged();
			}
		}

		protected virtual void OnValueChanged()
		{
			if (ValueChanged != null)
				ValueChanged(this, EventArgs.Empty);
		}

		protected virtual T FormatValue(T value)
		{
			var expression = GetBindingExpression("Value");
			if (expression != null)
			{
				value = (T)expression.ConvertValue(value);
			}

			return value;
		}

		public override string ToString()
		{
			if(Value == null)
				return Caption;

			return FormatValue(Value).ToString();
		}

//		protected virtual T FromString(string value)
//		{
//			return default(T);
//		}
	}
}