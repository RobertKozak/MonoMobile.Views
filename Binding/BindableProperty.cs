// 
// BindableProperty.cs
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
	using System.Collections;
	using System.Collections.Specialized;
	using System.ComponentModel;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Globalization;
	using System.Reflection;
	using MonoTouch.Foundation;
	
	[Preserve(AllMembers = true)]
	public class BindableProperty
	{
		private bool _Updating { get; set; }

		protected PropertyInfo ControlProperty { get; private set; }
		protected object Control { get; private set; }
		protected string ControlPropertyName { get; private set; }

		public Action PropertyChangedAction { get; set; }	

		public IBindingExpression _BindingExpression { get; set; }
		public IBindingExpression BindingExpression
		{
			get { return _BindingExpression; }
			set { _BindingExpression = value; }
		}

		public object ControlValue 
		{
			get 
			{ 
				if (ControlProperty != null && Control != null)
				{
					var value = ControlProperty.GetValue(Control); 
					return value;
				}

				return null;
			}
			set 
			{
				if (!_Updating)
				{
					_Updating = true;
					try 
					{		
						if (Control != null && (ControlProperty.CanWrite))
						{
							ControlProperty.SetValue(Control, value);

							Update();
						}

						if (PropertyChangedAction != null)
							PropertyChangedAction();
					}
					finally 
					{
						_Updating = false;
					}
				} 
			}
		}
		
		private BindableProperty()
		{
		}

		public static BindableProperty Register(string controlPropertyName)
		{
			var bindableProperty = new BindableProperty();
			bindableProperty.ControlPropertyName = controlPropertyName;
			
			return bindableProperty;
		}
		
		public void BindTo(object target)
		{
			BindTo(target, ControlPropertyName);
		}

		public void BindTo(object target, object source)
		{
			BindTo(target, ControlPropertyName, source);
		}

		public void BindTo(object target, string propertyName)
		{
			BindTo(target, propertyName, null);
		}

		public void BindTo(object target, string propertyName, object source)
		{
			if (Control != target)
			{
				Control = target;

				var property = propertyName;
				if (BindingExpression == null)
					property = ControlPropertyName;
				
				ControlProperty = Control.GetType().GetProperty(property, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
				if (ControlProperty == null)
					throw new Exception(string.Format("Property named {0} was not found in class {1}", property, Control.GetType().Name));
			}

			if (source != null && BindingExpression == null)
			{
				var bindable = target as IBindable;
				BindingExpression = BindingOperations.SetBinding(bindable, propertyName, source);
			}
		}

		public void Update()
		{
			Update(ControlValue);
		}
		
		public void Update(object value)
		{			
			ControlValue = value;

			if (BindingExpression != null)
			{
				value = BindingExpression.ConvertbackValue(value);

				BindingExpression.UpdateSource();
			}
		}

		public static BindableProperty GetBindableProperty(object obj, string bindablePropertyName)
		{
			var bindable = obj as IBindable;
			if (bindable != null)
				obj = bindable.DataTemplate;

			Type type = obj.GetType();

			var fieldInfo = type.GetField(bindablePropertyName, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);

			if (fieldInfo != null)
			{
				return fieldInfo.GetValue(obj) as BindableProperty;
			}

			return null;
		}

		public T Convert<T>()
		{
			object sourceValue = ControlValue;
			sourceValue = BindingExpression.ConvertValue(sourceValue);
			return (T)sourceValue;
		 }
		
		public object Convert()
		{
			object sourceValue = ControlValue;
			sourceValue = BindingExpression.ConvertValue(sourceValue);
			return sourceValue;
		}
		
		public T ConvertBack<T>()
		{
			object sourceValue = ControlValue;
			sourceValue = BindingExpression.ConvertbackValue(sourceValue);
			return (T)sourceValue;
		}
		
		public object ConvertBack(object value, Type convertType)
		{
			object sourceValue = value;
			sourceValue = BindingExpression.ConvertbackValue(sourceValue);
			return sourceValue;
		}
	}
}

