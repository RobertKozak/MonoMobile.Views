// 
//  MemberData.cs
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

			UpdateValue();
			Id = CreateId();
			
			AddNotifyPropertyChangedHandler(Source, this);
			AddNotifyPropertyChangedHandler(DataContextSource, this);
			
			Console.WriteLine("Creating MemberData: {0} from {1}", Member.Name, Source.GetType());
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Console.WriteLine("Disposing MemberData: {0} from {1}", Member.Name, Source.GetType());
				
				Id.Dispose();

				if (DataContextBinder != null)
				{
					DataContextBinder.Dispose();
				}
			}
			
			base.Dispose(disposing);
		}

		protected virtual object GetValue()
		{
			if (Member != null && Source != null)
			{
				var view = Source as IDataContext<object>;
				if (view != null && view.DataContext != null)
				{
					DataContextSource = view.DataContext;
					DataContextMember = DataContextSource.GetType().GetMember(Member.Name).FirstOrDefault();
					
					if (DataContextMember != null)
					{
						var dataContextValue = DataContextMember.GetValue(DataContextSource);
						return ConvertValue(dataContextValue);
					}
				}

				var value = Member.GetValue(Source);
				return ConvertValue(value);
			}

			return ConvertValue(_Value);
		}

		protected virtual void SetValue(object value)
		{	
			var shouldSetHandlers = false;		
			object oldValue = null;
			UnconvertedValue = value;

			if (_DataContextValue != value || _Value != value)
			{
				var convertedValue = ConvertbackValue(value);

				if (_DataContextValue != value)
				{
					if (DataContextMember != null)
					{
						oldValue = DataContextMember.GetValue(DataContextSource);
						if (oldValue != value)
						{
							RemoveNotifyCollectionChangedHandler(_DataContextValue, this);
							RemoveNotifyPropertyChangedHandler(_DataContextValue, this);
						
							shouldSetHandlers = true;
							
							ResetCollection(_DataContextValue as INotifyCollectionChanged, value as IList);
							
							if (DataContextMember.CanWrite())
								DataContextMember.SetValue(DataContextSource, convertedValue);
						}
	
						_DataContextValue = convertedValue;
					}
				}
	
				if (_Value != value)
				{
					oldValue = Member.GetValue(Source);
					if (oldValue != value)
					{	
						RemoveNotifyCollectionChangedHandler(_Value, this);
						RemoveNotifyPropertyChangedHandler(_Value, this);

						ResetCollection(_Value as INotifyCollectionChanged, value as IList);
						
						shouldSetHandlers = true;
	
						if (Member.CanWrite())
							Member.SetValue(Source, convertedValue);
					}
	
					_Value = convertedValue;
				}
			}

			if (shouldSetHandlers)
			{
				AddNotifyCollectionChangedHandler(value, this);
				AddNotifyPropertyChangedHandler(value, this); 
			}

			Type = Member.GetMemberType();
			Id = CreateId();
		}
		
		protected void SetDataContextBinder(DataContextBinder binder)
		{
			AddNotifyCollectionChangedHandler(Value, binder);
			AddNotifyPropertyChangedHandler(Value, binder);

			_DataContextBinder = binder;
		}
		
		public void UpdateValue()
		{
			SetValue(GetValue());
		}
		
		public void HandleNotifyPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Member.Name)
			{
				Log.Time("MemberData NotifyPropertyChanged property = "+ e.PropertyName+ " sender: "+sender.ToString(), ()=>
				        {
					UpdateValue();
				});
			}
		}

		public void HandleNotifyCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			UpdateValue();
		}

		private static void RemoveNotifyCollectionChangedHandler(object value, IHandleNotifyCollectionChanged handler)
		{
			var notifyCollectionChanged = value as INotifyCollectionChanged;
			if (notifyCollectionChanged != null && handler != null)
			{
				notifyCollectionChanged.CollectionChanged -= handler.HandleNotifyCollectionChanged;
			}
		}

		private static void RemoveNotifyPropertyChangedHandler(object value, IHandleNotifyPropertyChanged handler)
		{
			var notifyPropertyChanged = value as INotifyPropertyChanged;
			if (notifyPropertyChanged != null && handler != null)
			{
				notifyPropertyChanged.PropertyChanged -= handler.HandleNotifyPropertyChanged;
			}
		}

		private static void AddNotifyCollectionChangedHandler(object value, IHandleNotifyCollectionChanged handler)
		{
			RemoveNotifyCollectionChangedHandler(value, handler);
			var notifyCollectionChanged = value as INotifyCollectionChanged;
			if (notifyCollectionChanged != null && handler != null)
			{
				notifyCollectionChanged.CollectionChanged += handler.HandleNotifyCollectionChanged;
			}
		}

		private static void AddNotifyPropertyChangedHandler(object value, IHandleNotifyPropertyChanged handler)
		{
			RemoveNotifyPropertyChangedHandler(value, handler);
			var notifyPropertyChanged = value as INotifyPropertyChanged;
			if (notifyPropertyChanged != null && handler != null)
			{
				notifyPropertyChanged.PropertyChanged += handler.HandleNotifyPropertyChanged;
			}
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

		private object ConvertValue(object value)
		{
			object convertedValue = value;
			
			try 
			{
				if (ValueConverter != null)
				{
					var parameter = GetConverterParameter();
					convertedValue = ValueConverter.Convert(value, TargetType, parameter, CultureInfo.CurrentUICulture);
				}
				
				if (TargetType != null)
				{
					var typeCode = Convert.GetTypeCode(convertedValue);
					if (typeCode != TypeCode.Object && typeCode != TypeCode.Empty)
					{
						try 
						{
							convertedValue = Convert.ChangeType(convertedValue, TargetType);
						}
						catch(InvalidCastException)
						{
						}
					}
				}
			}
			catch (NotImplementedException)
			{
			}

			return convertedValue;
		}

		private object ConvertbackValue(object value)
		{
			object convertedValue = value;
			var memberType = Member.GetMemberType();
			
			try
			{
				if (ValueConverter != null)
				{
					var parameter = GetConverterParameter();
					convertedValue = ValueConverter.ConvertBack(value, memberType, parameter, CultureInfo.CurrentUICulture);
				}
				
				var typeCode = Convert.GetTypeCode(convertedValue);
				if (typeCode != TypeCode.Object && typeCode != TypeCode.Empty && typeCode != TypeCode.Int32)
				{
					try
					{
						convertedValue = Convert.ChangeType(convertedValue, memberType);
					}
					catch(InvalidCastException)
					{
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

