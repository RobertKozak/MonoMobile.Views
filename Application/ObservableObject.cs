//
// ObservableObject.cs:
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
namespace MonoMobile.Views
{
    using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq.Expressions;
	using MonoTouch.Foundation;
	
	[Preserve(AllMembers = true)]
    public class ObservableObject : NSObject, IObservableObject
    {
		private bool _SuspendNotifications { get; set; }
		private List<string> _BatchedNotifications { get; set; }

        private IDictionary<string, object> _PropertyMap = new Dictionary<string, object>();

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public IDictionary<string, object> PropertyMap { get { return _PropertyMap; } set { _PropertyMap = value; } }

        public void NotifyPropertyChanged(string propertyName)
        {
			if (!_SuspendNotifications)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			else
			{
				_BatchedNotifications.Add(propertyName);
			}
        }

        public void NotifyPropertyChanged<T>(Expression<Func<T>> property)
        {
            NotifyPropertyChanged(property.PropertyName());
        }

		public object Get(string propertyName)
		{
			if (_PropertyMap.ContainsKey(propertyName))
				return (object)_PropertyMap[propertyName];

			return null;
		}

        public T Get<T>(Expression<Func<T>> property)
        {
            return Get(property, default(T));
        }

        public T Get<T>(Expression<Func<T>> property, T defaultValue)
        {
            return Get(property, () => defaultValue);
        }

        public T Get<T>(Expression<Func<T>> property, Func<T> defaultValue)
        {
            if (property == null)
            {
                throw new ArgumentException("cannot be null", "property");
            }

            var name = property.PropertyName();
            var isFunction = name.Contains(".") || string.IsNullOrEmpty(name);

            // Either property is of form:()=>Foo.Bar or property is a function:()=>CalculateValue();
            if (isFunction)
            {
				object source = this;
				var propertyInfo = GetType().GetNestedMember(ref source, name, false);
				if (source != this) 
				{
					return (T)propertyInfo.GetValue(source);
				}
            }

            if (_PropertyMap.ContainsKey(name))
                return (T)_PropertyMap[name];

            if (defaultValue != null)
            {
                T result = defaultValue.Invoke();
                if (!isFunction)
                {
                    Set(property, result);
                }

                return result;
            }

            return default(T);
        }

		public void Set<T>(string propertyName, T value)
		{
			if (propertyName.Contains("."))
			{
				object source = this;
				var propertyInfo = GetType().GetNestedMember(ref source, propertyName, false);
				if (source != this)
				{
					propertyInfo.SetValue(source, value);
					propertyName = propertyInfo.Name;
				}
			} 
			else
			{
				object tryValue = null;
				
				if (_PropertyMap.TryGetValue(propertyName, out tryValue))
				{
					T oldValue = (T)tryValue;
					
					if (oldValue == null && value == null)
						return;
					
					if (oldValue != null && oldValue.Equals(value))
						return;
					
					_PropertyMap[propertyName] = value;
				} 
				else
				{
					_PropertyMap.Add(propertyName, value);
				}
			}
			
			NotifyPropertyChanged(propertyName);
		}
		
        public void Set<T>(Expression<Func<T>> property, T value)
        {
            if (property == null)
            {
                throw new ArgumentException("cannot be null", "property");
            }

            var propertyName = property.PropertyName();

			Set(propertyName, value);
        }
	
		public virtual void BeginInit()
		{
			_SuspendNotifications = true;

			_BatchedNotifications = new List<string>();
		}

		public virtual void EndInit()
		{
			_SuspendNotifications = false;

			foreach(string propertyName in _BatchedNotifications)
			{
				NotifyPropertyChanged(propertyName);
			}

			_BatchedNotifications = null;
		}

	}
}

