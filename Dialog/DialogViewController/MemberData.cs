// 
//  MemberData.cs
// 
//  Author:
//    Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
// 
//  Copyright 2011 - 2012, Nowcom Corporation.
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
namespace MonoMobile.Views
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.ComponentModel;
	using System.Globalization;
	using System.Linq;
	using System.Reflection;
	using MonoTouch.Foundation;
	
	[Preserve(AllMembers = true)]
	public class MemberData: NSObject, IHandleNotifyPropertyChanged, IHandleNotifyCollectionChanged
	{
		private object _Value;
		private object _DataContextValue;
		private DataContextBinder _DataContextBinder;
		
		public object UnconvertedValue { get; set; }
	
		public object Value { get { return GetValue(); } set { SetValue(value); } }
		public Type Type { get; set; }
		public Type TargetType { get; set; }

		public NSString Id { get; private set; }
 
		public object DataContextSource { get; private set; }
		public MemberInfo DataContextMember { get; private set; }

		public object Source { get; private set; }
		public MemberInfo Member { get; private set; }
 
		public int Section { get; set; }
		public int Order { get; set; }
		public float RowHeight { get; set; }

		public IValueConverter ValueConverter { get; set; }
		public object ConverterParameter { get; set; }
		public string ConverterParameterName { get; set; }

		public DataContextBinder DataContextBinder 
		{
			get { return _DataContextBinder; } 
			set { SetDataContextBinder(value); }
		}

		public MemberData(object source, MemberInfo member)
		{
			Source = source;
			Member = member;
			
			var valueConverterAttribute = member.GetCustomAttribute<ValueConverterAttribute>();
			if (valueConverterAttribute != null && !(valueConverterAttribute is CellViewTemplate))
			{
				ValueConverter = valueConverterAttribute.ValueConverter;
				ConverterParameter = valueConverterAttribute.ConverterParameter;
				ConverterParameterName = valueConverterAttribute.ConverterParameterPropertyName;
			}

			UpdateValue();
			Id = CreateId();
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Id.Dispose();

				if (DataContextBinder != null)
				{
					DataContextBinder = null;
				}
				
				RemoveNotifyPropertyChangedHandler(Source, this);
				RemoveNotifyPropertyChangedHandler(DataContextSource, this);

				RemoveNotifyCollectionChangedHandler(Source, this);
				RemoveNotifyCollectionChangedHandler(DataContextSource, this);
			}
			
			base.Dispose(disposing);
		}

		public void UpdateValue()
		{
			SetValue(GetValue());
		}
		
		public void HandleNotifyPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (CanHandleNotifyPropertyChanged(e.PropertyName))
			{
				var value = GetValue();
				if (_DataContextValue != null && _DataContextValue != value)
				{
					ResetCollection(_DataContextValue as INotifyCollectionChanged, value as IList);
				}

				if (_Value != null && _Value != value)
				{
					ResetCollection(_Value as INotifyCollectionChanged, value as IList);
				}
			}
		}

		public void HandleNotifyCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			UpdateValue();
		}

		public void RemoveNotifyCollectionChangedHandler(object value, IHandleNotifyCollectionChanged handler)
		{
			var notifyCollectionChanged = value as INotifyCollectionChanged;
			if (notifyCollectionChanged != null && handler != null)
			{
				notifyCollectionChanged.CollectionChanged -= handler.HandleNotifyCollectionChanged;
			}
		}

		public void RemoveNotifyPropertyChangedHandler(object value, IHandleNotifyPropertyChanged handler)
		{
			var notifyPropertyChanged = value as INotifyPropertyChanged;
			if (notifyPropertyChanged != null && handler != null)
			{
				notifyPropertyChanged.PropertyChanged -= handler.HandleNotifyPropertyChanged;
			}
		}

		public void AddNotifyCollectionChangedHandler(object value, IHandleNotifyCollectionChanged handler)
		{
			RemoveNotifyCollectionChangedHandler(value, handler);
			var notifyCollectionChanged = value as INotifyCollectionChanged;
			if (notifyCollectionChanged != null && handler != null)
			{
				notifyCollectionChanged.CollectionChanged += handler.HandleNotifyCollectionChanged;
			}
		}

		public void AddNotifyPropertyChangedHandler(object value, IHandleNotifyPropertyChanged handler)
		{
			RemoveNotifyPropertyChangedHandler(value, handler);
			var notifyPropertyChanged = value as INotifyPropertyChanged;
			if (notifyPropertyChanged != null && handler != null)
			{
				notifyPropertyChanged.PropertyChanged += handler.HandleNotifyPropertyChanged;
			}
		}
		
		protected virtual object GetValue()
		{
			Type targetType = null;

			if (Member != null && Source != null)
			{
				var view = Source as IDataContext<object>;
				if (view != null && view.DataContext != null)
				{
					DataContextSource = view.DataContext;
					DataContextMember = DataContextSource.GetType().GetMember(Member.Name).FirstOrDefault();
					
					if (DataContextMember != null)
					{
						targetType = Member.GetMemberType();
						var dataContextValue = DataContextMember.GetValue(DataContextSource);
						
						return Convert(dataContextValue, Type, ValueConverter);
					}
				}

				var value = Member.GetValue(Source);
				return value;
			}

			return ConvertValue(_Value, targetType);
		}
		
		public virtual bool CanHandleNotifyPropertyChanged(string propertyName)
		{
			return (Member != null && propertyName == Member.Name) || (DataContextMember != null && propertyName == DataContextMember.Name);
		}
		
		protected virtual void SetValue(object value)
		{	
			var shouldSetHandlers = false;		
			object oldValue = null;
			UnconvertedValue = value;
			var convertedValue = value;

			
			if (_DataContextValue != value || _Value != value)
			{
				if (_DataContextValue != value)
				{
					if (DataContextMember != null)
					{						
						var targetType = DataContextMember.GetMemberType();
						convertedValue = ConvertbackValue(value, targetType);

						oldValue = DataContextMember.GetValue(DataContextSource);
						if (oldValue != value)
						{
							shouldSetHandlers = true;
							
							if (DataContextMember.CanWrite())
							{
								DataContextMember.SetValue(DataContextSource, convertedValue);
							}
						}
	
						_DataContextValue = convertedValue;
					}
				}
	
				if (_Value != convertedValue)
				{
					var targetType = Member.GetMemberType();
					convertedValue = ConvertValue(value, targetType);

					oldValue = Member.GetValue(Source);
					if (oldValue != value)
					{							
						shouldSetHandlers = true;
	
						if (Member.CanWrite())
						{
							Member.SetValue(Source, convertedValue);
						}
					}
	
					_Value = convertedValue;
				}
			}

			if (shouldSetHandlers)
			{
				AddNotifyCollectionChangedHandler(value, this);

				AddNotifyPropertyChangedHandler(DataContextSource, this); 
				AddNotifyPropertyChangedHandler(Source, this); 
			}

			Type = Member.GetMemberType();
			Id = CreateId();
		}
		
		protected void SetDataContextBinder(DataContextBinder binder)
		{
			AddNotifyCollectionChangedHandler(Value, binder);

			AddNotifyPropertyChangedHandler(DataContextSource, binder);
			AddNotifyPropertyChangedHandler(Source, binder);

			_DataContextBinder = binder;
		}

		private void ResetCollection(INotifyCollectionChanged collection, IList newCollection)
		{	
			if (collection != null)
			{
				var collectionChangedMethod = collection.GetType().GetMethod("OnCollectionChanged", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
				if (collectionChangedMethod != null)
				{
					collectionChangedMethod.Invoke(collection, new object[] { new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, null) });
					collectionChangedMethod.Invoke(collection, new object[] { new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newCollection) });

					SetDataContextBinder(DataContextBinder);
				}
			}
		}

		public object Convert(object value, Type targetType, IValueConverter valueConverter)
		{
			object result = value;
			
			if (valueConverter != null)
			{
				var parameter = GetConverterParameter();
				result = valueConverter.Convert(value, targetType, parameter, CultureInfo.CurrentUICulture);
			}
				
			if (targetType != null)
			{
				var typeCode = System.Convert.GetTypeCode(value);
				if (typeCode != TypeCode.Object && typeCode != TypeCode.Empty)
				{
					try					
					{
						result = System.Convert.ChangeType(result, targetType);
					}
					catch (InvalidCastException)
					{
					}
				}
			}

			return result;
		}

		private object ConvertValue(object value, Type targetType)
		{
			if (value != null && value.GetType() == targetType)
				return value;

			object convertedValue = value;
			
			try 
			{
				convertedValue = Convert(convertedValue, targetType, ValueConverter);
			}
			catch (NotImplementedException)
			{
			}

			return convertedValue;
		}

		private object ConvertbackValue(object value, Type targetType)
		{
			if (value != null && value.GetType() == targetType)
				return value;

			object convertedValue = value;
			var memberType = Member.GetMemberType();
			
			try
			{			
				if (ValueConverter != null)
				{
					var parameter = GetConverterParameter();
					convertedValue = ValueConverter.ConvertBack(value, memberType, parameter, CultureInfo.CurrentUICulture);
				}
	
				if (targetType != null)
				{
					var typeCode = System.Convert.GetTypeCode(convertedValue);
					if (typeCode != TypeCode.Object && typeCode != TypeCode.Empty)
					{
						try
						{
							convertedValue = System.Convert.ChangeType(convertedValue, targetType);
						}
						catch (InvalidCastException)
						{
						}
					}
				}
			}
			catch (FormatException)
			{
				var message = string.Format("The value \"{0}\" is of type {1} but the {2} \"{3}\" is of type {4}. You need to specify an IValueConverter to convert it.", 
					convertedValue, convertedValue.GetType(), Member.GetMemberTypeName(), Member.Name, memberType);
				throw new FormatException(message);
			}
			catch (NotImplementedException)
			{
			}

			return convertedValue;
		}

		public object GetConverterParameter()
		{
			object parameter = null;
			if (ConverterParameter != null)
			{
				parameter = ConverterParameter;
			}
			else
			{
				if (!string.IsNullOrEmpty(ConverterParameterName))
				{
					MemberInfo[] parameterMember = null;
					if (DataContextSource != null)
					{
						parameterMember = DataContextSource.GetType().GetMember(ConverterParameterName);
						if (parameterMember.Length > 0)
						{
							parameter = parameterMember[0].GetValue(DataContextSource);
						}
						else
						{
							parameterMember = Source.GetType().GetMember(ConverterParameterName);
							if (parameterMember.Length > 0)
							{
								parameter = parameterMember[0].GetValue(Source);
							}
						}
					}
				}
			}

			return parameter;
		}

		public NSString CreateId()
		{
			if (Id != null)
			{
				if (string.Compare(Id.ToString(), Member.Name, true) == 0)
				{
					return Id;
				}

				Id.Dispose();
			}

			return new NSString(Member.Name);
		}
	}
}

