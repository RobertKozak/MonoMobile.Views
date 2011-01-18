//
// BindingExpression.cs:
//
// Author:
//   Robert Kozak (rkozak@gmail.com)
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
namespace MonoTouch.MVVM
{
	using System;
	using System.Reflection;
	using System.Globalization;
	using MonoTouch.UIKit;
	using System.Collections.Generic;
	using MonoTouch.Dialog;

	public class BindingExpression : IBindingExpression
	{
		struct PendingUpdateKey
		{
			public UITableViewElementCell Cell;
			public PropertyInfo TargetProperty;
			public object Target;
		}

		public PropertyInfo SourceProperty;
		public PropertyInfo TargetProperty;
		public Element Element { get; set; }

		public Binding Binding { get; private set; }

		public BindingExpression(Binding binding, PropertyInfo targetProperty, object target)
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

			object source = Binding.Source;
			SourceProperty = source.GetType().GetNestedProperty(ref source, Binding.SourcePath, true);
			Binding.Source = source;
		}

		public void UpdateSource()
		{
			UpdateSource(GetTargetValue());
		}

		public void UpdateSource(object targetValue)
		{
			if (SourceProperty != null && SourceProperty.CanWrite && Binding.Mode != BindingMode.OneTime)
			{
				try
				{
					var convertedTargetValue = ConvertbackValue(targetValue);
					if (Convert.GetTypeCode(convertedTargetValue) != TypeCode.Object)
						convertedTargetValue = Convert.ChangeType(convertedTargetValue, SourceProperty.PropertyType);

					SourceProperty.SetValue(Binding.Source, convertedTargetValue, null);
				}
				catch (NotImplementedException)
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
			if (TargetProperty != null && TargetProperty.CanWrite && Binding.Mode == BindingMode.TwoWay)
			{
				if (sourceValue == null)
					sourceValue = Binding.TargetNullValue;
				try
				{
					var convertedSourceValue = ConvertValue(sourceValue);
			
					if (Convert.GetTypeCode(convertedSourceValue) != TypeCode.Object)
						convertedSourceValue = Convert.ChangeType(convertedSourceValue, TargetProperty.PropertyType);

					if (Element != null && Element.Cell != null && Element.Cell.Element == Element)
					{
						TargetProperty.SetValue(Binding.Target, convertedSourceValue, null);
					}
					
				}
				catch (InvalidCastException ex)
				{
					Console.WriteLine(ex.Message + " : "+ex.StackTrace);
				}
				catch (NotImplementedException ex)
				{
				}
			}
		}

		public object ConvertValue(object value)
		{
			object convertedValue = value;

			if (Binding.Converter != null)
			{
				object parameter = Element;
				if (Binding.ConverterParameter != null)
					parameter = Binding.ConverterParameter;

				convertedValue = Binding.Converter.Convert(value, TargetProperty.PropertyType, parameter, CultureInfo.CurrentUICulture);
			}

			return convertedValue;
		}

		public object ConvertbackValue(object value)
		{
			object convertedValue = value;
			
			if (Binding.Converter != null)
			{
				object parameter = Element;
				if (Binding.ConverterParameter != null)
					parameter = Binding.ConverterParameter;

				convertedValue = Binding.Converter.ConvertBack(value, SourceProperty.PropertyType, parameter, CultureInfo.CurrentUICulture);
			}
			
			return convertedValue;
		}

		protected virtual object GetSourceValue()
		{
			if (Binding.Source != null)
			{
				var property = Binding.Source.GetType().GetProperty(Binding.SourcePath, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

				if (property != null)
					return property.GetValue(Binding.Source, null);
			}

			return Binding.FallbackValue;
		}

		protected virtual object GetTargetValue()
		{
			return TargetProperty.GetValue(Binding.Target, null);
		}
	}
}

