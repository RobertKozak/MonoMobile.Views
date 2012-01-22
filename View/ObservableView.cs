// 
//  ObservableView.cs
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
	using System.ComponentModel;
	using System.Linq.Expressions;

	public class ObservableView : View, INotifyPropertyChanged, IObservableObject
	{
		private readonly IObservableObject _Observable = new ObservableObject();
 
		public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };
	
		public T Get<T>(Expression<Func<T>> property)
		{
			return _Observable.Get(property);
		}

		public T Get<T>(Expression<Func<T>> property, T defaultValue)
		{
			return _Observable.Get(property, defaultValue);
		}

		public T Get<T>(Expression<Func<T>> property, Func<T> defaultValue)
		{
			return _Observable.Get(property, defaultValue);
		}

		public void Set<T>(Expression<Func<T>> property, T value)
		{
			_Observable.Set(property, value);
		}

		public void NotifyPropertyChanged(string propertyName)
		{
			_Observable.NotifyPropertyChanged(propertyName);
		}

		public void NotifyPropertyChanged<T>(Expression<Func<T>> property)
		{
			_Observable.NotifyPropertyChanged(property);
		}

		public void BeginInit()
		{
			_Observable.BeginInit();
		}

		public void EndInit()
		{
			_Observable.EndInit();
		}
	}
}

