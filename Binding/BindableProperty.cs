using System.Linq.Expressions;
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
	using System.Linq;
	using System.Reflection;	
	
	public class BindableProperty
	{
		private bool _Updating { get; set; }
		private string _SourcePropertyName { get; set; }
		
		protected PropertyInfo SourceProperty { get; private set; }
		protected MemberInfo Property { get; private set; }
		protected object SourceObject { get; private set; }
		protected object Obj { get; private set; }

		public Action PropertyChangedAction { get; set; }
		
		public object SourceValue
		{
			get
			{ 
				if (SourceProperty != null && SourceObject != null)
					return SourceProperty.GetValue(SourceObject); 
				
				return null;
			}
		}

		public object Value 
		{
			get 
			{ 
				if (Property != null && Obj != null)
					return Property.GetValue(Obj); 
				
				return null;
			}
			set 
			{
				if (!_Updating)
				{
					_Updating = true;
					try 
					{		
						if (Obj != null)
						{
							Property.SetValue(Obj, value);
						}
					
						if (SourceProperty != null)
						{							
							if (SourceProperty != Property)
							{
								SourceProperty.SetValue(SourceObject, value, null);
							}
							
							var propName = string.Concat(SourceProperty.Name, "Property.Value");
							var bindingExpression = BindingOperations.GetBindingExpression(this, propName);
							if (bindingExpression != null)
							{
								bindingExpression.UpdateSource();
							}
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
			bindableProperty._SourcePropertyName = sourcePropertyName;
			
			return bindableProperty;
		}

		public static BindableProperty Register()
		{
			return BindableProperty.Register(null);
		}
		
		public void BindTo<T>(object obj, Expression<Func<T>> property)
		{
			BindTo(obj, property.PropertyName());
		}
		
		private void BindTo(object obj, string propertyName)
		{
			object value = null;

			SourceObject = obj;

			Type type = SourceObject.GetType();
			SourceProperty= type.GetProperty(_SourcePropertyName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			if (SourceProperty != null)
				value = SourceProperty.GetValue(SourceObject, null);

			object owner = obj;
			Property = type.GetNestedMember(ref owner, propertyName, true);
			if (Property == null)
				throw new Exception(string.Format("Property named {0} was not found in class {1}", propertyName, owner.GetType().Name));
			
			Obj = owner;
			
			if (value != null && Value != value)
			{
				if (Obj != null)
					Property.SetValue(Obj, value);
			}
		}

		public void Update()
		{
			var bindingExpression = BindingOperations.GetBindingExpressionsForElement(SourceObject).FirstOrDefault();
			if (bindingExpression != null)
			{
				var value = bindingExpression.ConvertValue(Value);
				if (SourceProperty != null)
				{
					SourceProperty.SetValue(SourceObject, value, null);
				}

				bindingExpression.UpdateSource();
			}
		}
		
		public void ConvertBack<T>()
		{
			var bindingExpression = BindingOperations.GetBindingExpressionsForElement(SourceObject).FirstOrDefault();
			if (bindingExpression != null)
			{
				Value = (T)bindingExpression.ConvertbackValue(Value);
			}
		}
		
		public void RefreshFromSource()
		{
			if (SourceObject != null)
			{
				Type type = SourceObject.GetType();
				SourceProperty = type.GetProperty(_SourcePropertyName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
				if (SourceProperty != null)
					Value = SourceProperty.GetValue(SourceObject, null);
			}
		}

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

