//
// BindingOperations.cs:
//
// Author:
//   Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak
//
// Copyright 2011, Nowcom Corporation
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
	using System.Collections.Specialized;
	using System.ComponentModel;
	using System.Linq;
	using System.Reflection;
	using MonoMobile.MVVM;

	public static class BindingOperations
	{
		private class PropertyBinder
		{
			public IBindable Object;
			public string Property;
		}

		private static bool _UpdatingBindings { get; set; }
		private static Dictionary<PropertyBinder, Binding> _Bindings = new Dictionary<PropertyBinder, Binding>();
		private static List<IBindingExpression> _BindingExpressions;

		public static void ClearAllBindings()
		{
			_BindingExpressions.Clear();
			_Bindings.Clear();
		}
		
		public static void ClearBindings(object element)
		{
			var bindingExpressions = GetBindingExpressionsForElement(element);
			foreach(var bindingExpression in bindingExpressions)
				ClearBinding(bindingExpression.Binding.Target, bindingExpression.Binding.TargetPath);
		}

		public static void ClearBinding(object target, string property)
		{
			var bindingExpression = GetBindingExpression(target, property);
			if (bindingExpression != null)
				_BindingExpressions.Remove(bindingExpression);

			var binding = _Bindings.SingleOrDefault((kvp)=>kvp.Key.Object == target && kvp.Key.Property == property);
			if (binding.Key != null)
				_Bindings.Remove(binding.Key);
		}

		public static Binding GetBinding(object target, string property)
		{
			var bindingExpression = GetBindingExpression(target, property);
			if (bindingExpression != null)
				return bindingExpression.Binding;

			return null;
		}

		public static IBindingExpression GetBindingExpression(Binding binding)
		{
			UpdateBindings();

			if (_BindingExpressions != null)
			{
				return _BindingExpressions.SingleOrDefault((b)=>b.Binding == binding);
			}

			return null;
		}

		public static IBindingExpression GetBindingExpression(object target, string property)
		{
			UpdateBindings();

			if (_BindingExpressions != null)
			{
				return _BindingExpressions.SingleOrDefault((b)=>b.Binding.TargetPath == property && b.Binding.Target.Equals(target));
			}

			return null;
		}

		public static IBindingExpression[] GetBindingExpressionsForElement(object element)
		{
			UpdateBindings();
			if (_BindingExpressions != null)
			{
				return _BindingExpressions.Where((b)=>b.Element == element).ToArray();
			}

			return null;
		}

		private static IBindingExpression[] GetBindingExpressions(string property)
		{
			UpdateBindings();
			if (_BindingExpressions != null)
			{
				return _BindingExpressions.Where((b)=>b.Binding.SourcePath == property).ToArray();
			}

			return null;
		}

		public static bool IsDataBound(IBindable target, string property) { return false; }
		
		public static IBindingExpression SetBinding(IBindable target, string targetProperty, object dataContext)
		{
			var binding = new Binding(targetProperty, "DataContext") { Source = dataContext } ;
			return SetBinding(target, binding);
		}

		public static IBindingExpression SetBinding(IBindable target, Binding binding)
		{
			if (target == null)
				throw new ArgumentNullException("target");

			if (binding == null)
				throw new ArgumentNullException("binding");
			
			var targetProperty = binding.TargetPath;

			IBindingExpression bindingExpression = null;
			
			object dataTemplate = target.DataTemplate;
			object nestedTarget = dataTemplate;
			var element = target as IElement;

			MemberInfo memberInfo = null;
			FieldInfo bindablePropertyInfo = null;

			if (dataTemplate != null)
			{
				var name = string.Concat(targetProperty, "Property");
				bindablePropertyInfo = dataTemplate.GetType().GetField(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
				
				name = string.Concat(name, ".ControlValue");			
				memberInfo = dataTemplate.GetType().GetNestedMember(ref nestedTarget, name, false);
				if (memberInfo != null)
				{
					binding.TargetPath = name;
					binding.Target = nestedTarget;
				}
			}

			var targetReady = memberInfo != null && nestedTarget != null && binding.Source != null;

			if (targetReady)
			{
				if (_BindingExpressions == null)
					_BindingExpressions = new List<IBindingExpression>();
				
				bindingExpression = GetBindingExpression(binding);

				if (bindingExpression != null)
				{
					_BindingExpressions.Remove(bindingExpression);
				}
			
				bindingExpression = new BindingExpression(binding, memberInfo, nestedTarget) { Element = element };

				_BindingExpressions.Add(bindingExpression);
					
				var vmINPC = bindingExpression.Binding.Source as INotifyPropertyChanged;
				if (vmINPC != null)
				{
					vmINPC.PropertyChanged -= HandleDataContextPropertyChanged;
					vmINPC.PropertyChanged += HandleDataContextPropertyChanged;
				}

				var viewINPC = bindingExpression.Binding.ViewSource as INotifyPropertyChanged;
				if (viewINPC != null)
				{
					viewINPC.PropertyChanged -= HandleDataContextPropertyChanged;
					viewINPC.PropertyChanged += HandleDataContextPropertyChanged;
				}
				
				var sourceValue = bindingExpression.GetSourceValue();
				 
				var sourceCollection = sourceValue as INotifyCollectionChanged;
				if (sourceCollection != null)
				{	
					SetNotificationCollectionHandler(bindingExpression, sourceCollection);
				}	
			}
			else
			{
				var binderKey =_Bindings.SingleOrDefault((kvp)=>kvp.Key.Object == target && kvp.Key.Property == targetProperty).Key;

				if (binderKey == null)
					_Bindings.Add(new PropertyBinder() { Object = target, Property = targetProperty }, binding);
				else
					_Bindings[binderKey] = binding;
			}
			
			if (bindablePropertyInfo != null)
			{
				var bindableProperty = bindablePropertyInfo.GetValue(dataTemplate) as BindableProperty;
				if (bindableProperty != null)
					bindableProperty.BindingExpression = bindingExpression;
			}

			return bindingExpression;
		}
		
		public static void SetNotificationCollectionHandler(IBindingExpression bindingExpression, INotifyCollectionChanged collection)
		{
			NotifyCollectionChangedEventHandler handler = (sender, e) => 
			{
				var section = bindingExpression.Element as ISection;
				section.CollectionChanged(e);

//				bindingExpression.UpdateTarget();
			};			

			collection.CollectionChanged -= handler;
			collection.CollectionChanged += handler;
		}

		private static void UpdateBindings()
		{
			if (!_UpdatingBindings)
			{
				try
				{
					_UpdatingBindings = true;

					var bindingsToRemove = new List<PropertyBinder>();

					if (_Bindings.Count > 0)
					{
						var keys = _Bindings.Keys.ToArray();
						for (var i = 0; i < keys.Length; i++)
						{
							var binding = _Bindings[keys[i]];
							var bindingExpression = SetBinding(keys[i].Object, keys[i].Property, binding);
							if(bindingExpression != null)
								bindingsToRemove.Add(keys[i]);
						}

						if (bindingsToRemove.Count > 0)
						{
							foreach(var bindingKey in bindingsToRemove)
							{
								_Bindings.Remove(bindingKey);
							}
						}

						bindingsToRemove.Clear();
					}
				}
				finally
				{
					_UpdatingBindings = false;
				}
			}
		}

		private static void HandleDataContextPropertyChanged(object sender, PropertyChangedEventArgs e)
		{ 
			IBindingExpression[] bindingExpressions = GetBindingExpressions(e.PropertyName);
			foreach (var bindingExpression in bindingExpressions)
			{
				bindingExpression.UpdateTarget();
			}
		}
	}
}

