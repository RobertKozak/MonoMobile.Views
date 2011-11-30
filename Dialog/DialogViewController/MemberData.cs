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
	using System.Linq;
	using System.Reflection;
	using MonoTouch.Foundation;
	
	[Preserve(AllMembers = true)]
	public class MemberData: IHandleNotifyPropertyChanged, IHandleNotifyCollectionChanged
	{
		private object _Value;
		private object _DataContextValue;
		private DataContextBinder _DataContextBinder;

		public object Value { get { return GetValue(); } set { SetValue(value); } }
		public Type Type { get; set; }

		public NSString Id { get; private set; }
 
		public object DataContextSource { get; private set; }
		public MemberInfo DataContextMember { get; private set; }

		public object Source { get; private set; }
		public MemberInfo Member { get; private set; }
 
		public int Section { get; set; }
		public int Order { get; set; }
		public float RowHeight { get; set; }

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
			Id = new NSString(Type.ToString());
			
		//	RemoveNotifyPropertyChangedHandler(Source, this);
			AddNotifyPropertyChangedHandler(Source, this);
			
		//	RemoveNotifyPropertyChangedHandler(DataContextSource, this);
			AddNotifyPropertyChangedHandler(DataContextSource, this);
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
						var _DataContextValue = DataContextMember.GetValue(DataContextSource);
						return _DataContextValue;
					}
				}

				//_Value = 
				return Member.GetValue(Source);
				//return _Value;
			}

			return _Value;
		}

		protected virtual void SetValue(object value)
		{	
			var shouldSetHandlers = false;		
			object oldValue = null;

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

						DataContextMember.SetValue(DataContextSource, value);
					}

					_DataContextValue = value;
				}
			}

			if (_Value != value)
			{
				oldValue = Member.GetValue(Source);
				if (oldValue != value)
				{
					RemoveNotifyCollectionChangedHandler(_Value, this);
					RemoveNotifyPropertyChangedHandler(_Value, this);
					
					shouldSetHandlers = true;

					ResetCollection(_Value as INotifyCollectionChanged, value as IList);

					Member.SetValue(Source, value);
				}

				_Value = value;
			}

			if (shouldSetHandlers)
			{
				AddNotifyCollectionChangedHandler(value, this);
				AddNotifyPropertyChangedHandler(value, this); 
			}

			Type = Member.GetMemberType();
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
			UpdateValue();
		}

		public void HandleNotifyCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			UpdateValue();
		}

		private void RemoveNotifyCollectionChangedHandler(object value, IHandleNotifyCollectionChanged handler)
		{
			var notifyCollectionChanged = value as INotifyCollectionChanged;
			if (notifyCollectionChanged != null && handler != null)
			{
				notifyCollectionChanged.CollectionChanged -= handler.HandleNotifyCollectionChanged;
			}
		}

		private void RemoveNotifyPropertyChangedHandler(object value, IHandleNotifyPropertyChanged handler)
		{
			var notifyPropertyChanged = value as INotifyPropertyChanged;
			if (notifyPropertyChanged != null && handler != null)
			{
				notifyPropertyChanged.PropertyChanged -= handler.HandleNotifyPropertyChanged;
			}
		}

		private void AddNotifyCollectionChangedHandler(object value, IHandleNotifyCollectionChanged handler)
		{
			var notifyCollectionChanged = value as INotifyCollectionChanged;
			if (notifyCollectionChanged != null && handler != null)
			{
				notifyCollectionChanged.CollectionChanged += handler.HandleNotifyCollectionChanged;
			}
		}

		private void AddNotifyPropertyChangedHandler(object value, IHandleNotifyPropertyChanged handler)
		{
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
	}
}

