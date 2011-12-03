//using MonoTouch.UIKit;
////
//// BindingExpression.cs:
////
//// Author:
////   Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak
////
//// Copyright 2011, Nowcom Corporation
////
//// Code licensed under the MIT X11 license
////
//// Permission is hereby granted, free of charge, to any person obtaining
//// a copy of this software and associated documentation files (the
//// "Software"), to deal in the Software without restriction, including
//// without limitation the rights to use, copy, modify, merge, publish,
//// distribute, sublicense, and/or sell copies of the Software, and to
//// permit persons to whom the Software is furnished to do so, subject to
//// the following conditions:
////
//// The above copyright notice and this permission notice shall be
//// included in all copies or substantial portions of the Software.
////
//// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////
//namespace MonoMobile.Views
//{
//	using System;
//	using System.Collections;
//	using System.Collections.Specialized;
//	using System.Globalization;
//	using System.Linq;
//	using System.Reflection;
//	using MonoMobile.Views;	
//
//	public class BindingExpression : IBindingExpression
//	{
//		struct PendingUpdateKey
//		{
//			public UITableViewCell Cell;
//			public PropertyInfo TargetProperty;
//			public object Target;
//		}
//		
//		private MemberInfo _ViewProperty { get; set; }
//
//		public MemberInfo SourceProperty { get; set; }
//		public MemberInfo TargetProperty { get; set; }
//
////		private IElement _Element;
////		public IElement Element 
////		{ 
////			get {return _Element; } 
////			set { _Element = value; } 
////		}
//
//		public Binding Binding { get; private set; }
//
//		public BindingExpression(Binding binding, MemberInfo targetProperty, object target)
//		{
//			if (binding == null)
//				throw new ArgumentNullException("binding");
//
//			if (targetProperty == null)
//				throw new ArgumentNullException("targetProperty");
//
//			if (target == null)
//				throw new ArgumentNullException("target");
//
//			Binding = binding;
//			Binding.Target = target;
//			TargetProperty = targetProperty;
//			if(string.IsNullOrEmpty(binding.TargetPath))
//			{
//				binding.TargetPath = targetProperty.Name;
//			}
//
//			object viewSource = Binding.Source;
//			_ViewProperty = viewSource.GetType().GetNestedMember(ref viewSource, Binding.SourcePath, true);
//			Binding.ViewSource = viewSource;
//			SourceProperty = _ViewProperty;
//
//			var dataContext = viewSource as IDataContext<object>;
//			if (dataContext != null && dataContext.DataContext != null)
//			{
//				var source = dataContext.DataContext;
//				
//				SourceProperty = source.GetType().GetNestedMember(ref source, Binding.SourcePath, true);
//				Binding.Source = source;
//			}
//		}
//
//		public void UpdateSource()
//		{
//			var targetValue = GetTargetValue();
//
//			UpdateSource(Binding.ViewSource, _ViewProperty, targetValue);
//
//			if (SourceProperty != null && SourceProperty != _ViewProperty)
//				UpdateSource(Binding.Source, SourceProperty, targetValue);
//		}
//
//		private void UpdateSource(object obj, MemberInfo member, object targetValue)
//		{
//			bool canWrite = true;
//			if (member is PropertyInfo) canWrite = ((PropertyInfo)member).CanWrite;
//
//			if (member != null && canWrite && Binding.Mode == BindingMode.TwoWay)
//			{
//				try
//				{
//					object convertedTargetValue = ConvertbackValue(targetValue, member);
//					
//					convertedTargetValue = CheckAndCoerceToGenericEnumerable(member.GetMemberType(), convertedTargetValue);
//					SetValue(member, obj, convertedTargetValue);	
//				}
//				catch (InvalidCastException ex)
//				{
//					Console.WriteLine(ex.Message);
//				}
//				catch (NotImplementedException ex1)
//				{
//					Console.WriteLine(ex1.Message);
//				}
//				catch (NotSupportedException ex2)
//				{
//					Console.WriteLine(ex2.Message);
//				}
//			}
//		}
//
//		public void UpdateTarget()
//		{
//			var sourceValue = GetSourceValue();
//			UpdateTarget(sourceValue);
//		}
//
//		public void UpdateTarget(object sourceValue)
//		{
//			var collection = sourceValue as INotifyCollectionChanged;
//			if (collection != null)
//			{
//				BindingOperations.SetNotificationCollectionHandler(this, collection);
//				SetValue(TargetProperty, Binding.Target, sourceValue);
//			}
//			else
//			{
//
//				bool canWrite = true;
//				if (TargetProperty is PropertyInfo) canWrite = ((PropertyInfo)TargetProperty).CanWrite;
//				if (TargetProperty != null && canWrite && Binding.Mode != BindingMode.OneTime)
//				{
//					if (sourceValue == null)
//						sourceValue = Binding.TargetNullValue;
//	
//					object convertedSourceValue = ConvertValue(sourceValue);
//	
////					if (Element != null && (Element.Cell != null && Element.Cell.Element == Element) || Element.Cell == null)
////					{
////						convertedSourceValue = CheckAndCoerceToObjectEnumerable(convertedSourceValue);
////						SetValue(TargetProperty, Binding.Target, convertedSourceValue);
////					}
//				}
//			}
//		}
//
//		public object ConvertValue(object value)
//		{
//			object convertedValue = value;
//			
//			var memberType = TargetProperty.GetMemberType();
//
//			if (Binding.Converter != null)
//			{
////				try
//				{
////					object parameter = Element;
////					if (Binding.ConverterParameter != null)
////						parameter = Binding.ConverterParameter;
////	
////					convertedValue = Binding.Converter.Convert(value, memberType, parameter, CultureInfo.CurrentUICulture);
//				}
////				catch (InvalidCastException) {}
////				catch (NotSupportedException) {}
////				catch (NotImplementedException) {}
//			}
//
//			var typeCode = Convert.GetTypeCode(convertedValue);
//			if (typeCode != TypeCode.Object && typeCode != TypeCode.Empty)
//			{
//				convertedValue = Convert.ChangeType(convertedValue, memberType);
//			}
//
//			return convertedValue;
//		}
//
//		public object ConvertbackValue(object value, MemberInfo member)
//		{
//			object convertedValue = value;
//			var convertSupported = true;
//			
//			try
//			{
//			if (Binding.Converter != null)
//			{
////				try
//				{
////					object parameter = Element;
////					if (Binding.ConverterParameter != null)
////						parameter = Binding.ConverterParameter;
////					
////					convertedValue = Binding.Converter.ConvertBack(value, member.GetMemberType(), parameter, CultureInfo.CurrentUICulture);
//				}
////				catch (InvalidCastException) {}
////				catch (NotSupportedException) { convertSupported = false; }
////				catch (NotImplementedException) { convertSupported = false; }
//			}
//			
//			if (convertSupported)
//			{
//				var typeCode = Convert.GetTypeCode(convertedValue);
//				if (typeCode != TypeCode.Object && typeCode != TypeCode.Empty && typeCode != TypeCode.Int32)
//					convertedValue = Convert.ChangeType(convertedValue, member.GetMemberType());
//			}
//			}
//			catch(FormatException)
//			{
//				var message = string.Format("{0} is a {1} but {2} is a {3}. Did you forget to specify an IValueConverter?", convertedValue, convertedValue.GetType(), member.Name, member.GetMemberType());
//				throw new FormatException(message);
//			}
//
//			return convertedValue;
//		}
//		
//		public object ConvertbackValue(object value)
//		{
//			var member = SourceProperty;
//			if (member == null)
//				member = _ViewProperty;
//			
//			object convertedValue = value;
//
//			if (member != null)
//				convertedValue = ConvertbackValue(value, member);
//
//			return convertedValue;
//		}
//
//		public virtual object GetSourceValue()
//		{
//			object value = null;
//
//			if (SourceProperty != null)
//			{
//				value = SourceProperty.GetValue(Binding.Source);
//			}
//			
//			if (value == null && _ViewProperty != null)
//			{
//				value = _ViewProperty.GetValue(Binding.ViewSource);
//			}
//			
//			if (value != null) return value;
//
//			return Binding.FallbackValue;
//		}
//
//		public virtual object GetTargetValue()
//		{
//			return TargetProperty.GetValue(Binding.Target);
//		}
//
//		private void SetValue(MemberInfo member, object obj, object value)
//		{
//			try
//			{
//				if (member.MemberType == MemberTypes.Field)
//				{
//					((FieldInfo)member).SetValue(obj, value);
//				}
//				
//				if (member.MemberType == MemberTypes.Property)
//				{
//					((PropertyInfo)member).SetValue(obj, value, null);
//				}
//			}
//			catch (Exception ex)
//			{
//				var message = string.Format("{0} : {1} : {2}", ex.Message, member.Name, value);
//				Console.WriteLine(message);
//			}
//		}
//
//		private object CheckAndCoerceToGenericEnumerable(Type type, object value)
//		{
//			object result = value;
//			if (type != null && result != null)
//			{
//				var isList = typeof(IEnumerable).IsAssignableFrom(type);
//				
//				if (type.IsGenericType && isList)
//				{
//					var genericTypeDefinition = type.GetGenericTypeDefinition();
//					var genericType = type.GetGenericArguments().FirstOrDefault();
//					Type[] generic = { genericType };
//					
//					result = Activator.CreateInstance(genericTypeDefinition.MakeGenericType(generic));
//					
//					var list = result as IList;
//					if (list != null)
//						foreach (var item in (IList)value)
//							list.Add(item);
//				}
//			}
//			return result;
//		}
//
//		private object CheckAndCoerceToObjectEnumerable(object value)
//		{
//			object result = value;
//			if (result != null)
//			{
//				Type type = value.GetType();			
//				var isList = typeof(IEnumerable).IsAssignableFrom(type);
//							
//				if (type.IsGenericType && isList)
//				{
//					var genericTypeDefinition = type.GetGenericTypeDefinition();
//					Type[] generic = { typeof(object) };
//					result = Activator.CreateInstance(genericTypeDefinition.MakeGenericType(generic));
//					
//					var list = result as IList;
//					if (list != null)
//						foreach (var item in (IList)value)
//							list.Add(item);
//				}
//			}
//			return result;
//		}
//	}
//}
//
