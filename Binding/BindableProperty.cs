// 
//  BindableProperty.cs
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
		private string _ElementPropertyName { get; set; }
		private Type _ElementType { get; set; }

		protected PropertyInfo ElementProperty { get; private set; }
		protected PropertyInfo ControlProperty { get; private set; }
		protected object Element { get; private set; }
		protected object Control { get; private set; }

		public Action PropertyChangedAction { get; set; }	

		public IBindingExpression _BindingExpression { get; set; }
		public IBindingExpression BindingExpression
		{
			get { return _BindingExpression; }
			set { _BindingExpression = value; }
		}

		public object ElementValue
		{
			get
			{ 
				if (ElementProperty != null && Element != null)
					return ElementProperty.GetValue(Element); 
				
				return null;
			}

			set
			{
				if (ElementProperty != null)
				{
					if (ElementProperty != ControlProperty && ElementProperty.CanWrite)
					{
						ElementProperty.SetValue(Element, value, null);
					}
					
					if (BindingExpression != null)
					{
						BindingExpression.UpdateSource();
					}
				}
			}
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

		public static BindableProperty Register(string sourcePropertyName)
		{
			var bindableProperty = new BindableProperty();
			bindableProperty._ElementPropertyName = sourcePropertyName;
			
			return bindableProperty;
		}

		public void BindTo(object source, object target, string propertyName)
		{
			if (Element == null)
			{
				Element = source;
				
				_ElementType = Element.GetType();
				ElementProperty = _ElementType.GetProperty(_ElementPropertyName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			}

			if (Control != target)
			{
				Control = target;

				ControlProperty = Control.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
				if (ControlProperty == null)
					throw new Exception(string.Format("Property named {0} was not found in class {1}", propertyName, Control.GetType().Name));
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
				
				ElementValue = value;
			}
		}

//		public T Convert<T>()
//		{
//			object sourceValue = ElementValue;
//			sourceValue = BindingExpression.ConvertValue(sourceValue);
//			
//			return (T)sourceValue;
//		}
//
//		public object Convert()
//		{
//			object sourceValue = ElementValue;
//			sourceValue = BindingExpression.ConvertValue(sourceValue);
//			
//			return sourceValue;
//		}
//
//		public T ConvertBack<T>()
//		{
//			object sourceValue = ElementValue;
//			sourceValue = BindingExpression.ConvertbackValue(sourceValue);
//
//			return (T)sourceValue;
//		}
//
//		public object ConvertBack(object value, Type convertType)
//		{
//			object sourceValue = value;
//			sourceValue = BindingExpression.ConvertbackValue(sourceValue);
//
//			return sourceValue;
//		}

		public static BindableProperty GetBindableProperty(object obj, string bindablePropertyName)
		{
			Type type = obj.GetType();

			var fieldInfo = type.GetField(bindablePropertyName);

			if (fieldInfo != null)
			{
				return fieldInfo.GetValue(obj) as BindableProperty;
			}

			return null;
		}
	}
}

