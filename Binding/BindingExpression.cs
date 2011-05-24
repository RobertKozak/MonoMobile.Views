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
			SourceProperty = _ViewProperty;

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

				object convertedSourceValue = ConvertValue(sourceValue);

				if (Element != null && Element.Cell != null && Element.Cell.Element == Element)
				{
					SetValue(TargetProperty, Binding.Target, convertedSourceValue);
				}

			}
		}

		public object ConvertValue(object value)
		{
			object convertedValue = value;
			
			var memberType = TargetProperty.GetMemberType();

			if (Binding.Converter != null)
			{
				try
				{
					object parameter = Element;
					if (Binding.ConverterParameter != null)
						parameter = Binding.ConverterParameter;
	
					convertedValue = Binding.Converter.Convert(value, memberType, parameter, CultureInfo.CurrentUICulture);
				}
				catch (InvalidCastException) {}
				catch (NotSupportedException) {}
				catch (NotImplementedException) {}
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
			var convertSupported = true;
			
			try
			{
			if (Binding.Converter != null)
			{
				try
				{
					object parameter = Element;
					if (Binding.ConverterParameter != null)
						parameter = Binding.ConverterParameter;
					
					convertedValue = Binding.Converter.ConvertBack(value, member.GetMemberType(), parameter, CultureInfo.CurrentUICulture);
				}
				catch (InvalidCastException) {}
				catch (NotSupportedException) { convertSupported = false; }
				catch (NotImplementedException) { convertSupported = false; }
			}
			
			if (convertSupported)
			{
				var typeCode = Convert.GetTypeCode(convertedValue);
				if (typeCode != TypeCode.Object && typeCode != TypeCode.Empty && typeCode != TypeCode.Int32)
					convertedValue = Convert.ChangeType(convertedValue, member.GetMemberType());
			}
			}
			catch(FormatException ex)
			{
				var message = string.Format("{0} is a {1} but {2} is a {3}. Did you forget to specify an IValueConverter?", convertedValue, convertedValue.GetType(), member.Name, member.GetMemberType());
				throw new FormatException(message);
			}

			return convertedValue;
		}
		
		public object ConvertbackValue(object value)
		{
			var member = SourceProperty;
			if (member == null)
				member = _ViewProperty;
			
			var convertedValue = ConvertbackValue(value, member);

			return convertedValue;
		}

		public virtual object GetSourceValue()
		{
			object value = null;

			if (SourceProperty != null)
			{
				value = SourceProperty.GetValue(Binding.Source);
			}
			
			if (value == null && _ViewProperty != null)
			{
				value = _ViewProperty.GetValue(Binding.ViewSource);
			}
			
			if (value != null) return value;

			return Binding.FallbackValue;
		}

		public virtual object GetTargetValue()
		{
			return TargetProperty.GetValue(Binding.Target);
		}

		private void SetValue(MemberInfo member, object obj, object value)
		{
			try
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
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message+ ":"+ member.Name);
			}
		}
	}
}

