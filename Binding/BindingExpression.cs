//
// BindingExpression.cs:
//
// Author:
//   Robert Kozak (rkozak@gmail.com) Twitter:@robertkozak
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
	using System.Reflection;
	using System.Globalization;
	using MonoTouch.UIKit;
	using System.Collections.Generic;
	using MonoMobile.MVVM;

	public class BindingExpression : IBindingExpression
	{
		struct PendingUpdateKey
		{
			public UITableViewElementCell Cell;
			public PropertyInfo TargetProperty;
			public object Target;
		}
		
		private MemberInfo _ViewProperty { get; set; }

		public MemberInfo SourceProperty { get; set; }
		public MemberInfo TargetProperty { get; set; }
		public IElement Element { get; set; }

		public Binding Binding { get; private set; }

		public BindingExpression(Binding binding, MemberInfo targetProperty, object target)
		{
			if (binding == null)
				throw new ArgumentNullException("binding");

			if (targetProperty == null)
				throw new ArgumentNullException("targetProperty");

			if (target == null)
				throw new ArgumentNullException("target");

			Binding = binding;
			Binding.Target = target;
			TargetProperty = targetProperty;
			if(string.IsNullOrEmpty(binding.TargetPath))
			{
				binding.TargetPath = targetProperty.Name;
			}

			object viewSource = Binding.Source;
			_ViewProperty = viewSource.GetType().GetNestedMember(ref viewSource, Binding.SourcePath, true);
			Binding.ViewSource = viewSource;

			var dataContext = viewSource as IDataContext;
			if (dataContext != null && dataContext.DataContext != null)
			{
				var source = dataContext.DataContext;
				
				SourceProperty = source.GetType().GetNestedMember(ref source, Binding.SourcePath, true);
				Binding.Source = source;
			}
		}

		public void UpdateSource()
		{
			var targetValue = GetTargetValue();

			UpdateSource(Binding.ViewSource, _ViewProperty, targetValue);

			if (SourceProperty != null && SourceProperty != _ViewProperty)
				UpdateSource(Binding.Source, SourceProperty, targetValue);
		}

		private void UpdateSource(object obj, MemberInfo member, object targetValue)
		{
			bool canWrite = true;
			if (member is PropertyInfo) canWrite = ((PropertyInfo)member).CanWrite;

			if (member != null && canWrite && Binding.Mode != BindingMode.OneTime)
			{
				try
				{
					object convertedTargetValue = ConvertbackValue(targetValue, member);

					SetValue(member, obj, convertedTargetValue);	
				}
				catch (NotImplementedException)
				{
				}
				catch (NotSupportedException)
				{
				}
			}
		}

		public void UpdateTarget()
		{
			UpdateTarget(GetSourceValue());
		}

		public void UpdateTarget(object sourceValue)
		{
			bool canWrite = true;
			if (TargetProperty is PropertyInfo) canWrite = ((PropertyInfo)TargetProperty).CanWrite;
			if (TargetProperty != null && canWrite && Binding.Mode == BindingMode.TwoWay)
			{
				if (sourceValue == null)
					sourceValue = Binding.TargetNullValue;
				try
				{
					object convertedSourceValue = ConvertValue(sourceValue);

					if (Element != null && Element.Cell != null && Element.Cell.Element == Element)
					{
						SetValue(TargetProperty, Binding.Target, convertedSourceValue);
					}
				
				}
				catch (InvalidCastException)
				{
				}
				catch (NotImplementedException)
				{
				}
				catch (NotSupportedException)
				{
				}
			}
		}

		public object ConvertValue(object value)
		{
			object convertedValue = value;
			var memberType = GetMemberType(TargetProperty);

			if (Binding.Converter != null)
			{
				object parameter = Element;
				if (Binding.ConverterParameter != null)
					parameter = Binding.ConverterParameter;

				convertedValue = Binding.Converter.Convert(value, memberType, parameter, CultureInfo.CurrentUICulture);
			}

			var typeCode = Convert.GetTypeCode(convertedValue);
			if (typeCode != TypeCode.Object && typeCode != TypeCode.Empty)
			{
				convertedValue = Convert.ChangeType(convertedValue, memberType);
			}

			return convertedValue;
		}

		public object ConvertbackValue(object value, MemberInfo member)
		{
			object convertedValue = value;
			
			if (Binding.Converter != null)
			{
				object parameter = Element;
				if (Binding.ConverterParameter != null)
					parameter = Binding.ConverterParameter;

				convertedValue = Binding.Converter.ConvertBack(value, GetMemberType(member), parameter, CultureInfo.CurrentUICulture);
			}
			
			var typeCode = Convert.GetTypeCode(convertedValue);
			if (typeCode != TypeCode.Object && typeCode != TypeCode.Empty && typeCode != TypeCode.Int32)
				convertedValue = Convert.ChangeType(convertedValue, GetMemberType(member));

			return convertedValue;
		}
		
		public object ConvertbackValue(object value)
		{
			var member = SourceProperty;
			if (member == null)
				member = _ViewProperty;
			
			return ConvertbackValue(value, member);
		}

		public virtual object GetSourceValue()
		{
			object value = null;

			if (SourceProperty != null)
			{
				value = GetValue(SourceProperty, Binding.Source);
			}
			
			if (value == null && _ViewProperty != null)
			{
				value = GetValue(_ViewProperty, Binding.ViewSource);
			}
			
			if (value != null) return value;

			return Binding.FallbackValue;
		}

		public virtual object GetTargetValue()
		{
			return GetValue(TargetProperty, Binding.Target);
		}

		private Type GetMemberType(MemberInfo member)
		{
			if (member.MemberType == MemberTypes.Field)
			{
				return ((FieldInfo)member).FieldType;
			}
			
			if (member.MemberType == MemberTypes.Property)
			{
				return ((PropertyInfo)member).PropertyType;
			}
			
			return null;
		}

		private void SetValue(MemberInfo member, object obj, object value)
		{
			if (member.MemberType == MemberTypes.Field)
			{
				((FieldInfo)member).SetValue(obj, value);
			}
			
			if (member.MemberType == MemberTypes.Property)
			{
				((PropertyInfo)member).SetValue(obj, value, null);
			}
		}

		private object GetValue(MemberInfo member, object obj)
		{
			if (member.MemberType == MemberTypes.Field)
			{
				return ((FieldInfo)member).GetValue(obj);
			}
			
			if (member.MemberType == MemberTypes.Property)
			{
				return ((PropertyInfo)member).GetValue(obj, null);
			}
			
			return null;
		}
	}
}

