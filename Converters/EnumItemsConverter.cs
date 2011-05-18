//
// EnumItemsConverter.cs: 
//
// Author:
//   Robert Kozak (rkozak@nowcom.com)
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
	using System.Linq;
	using MonoTouch.Foundation;
	
	[Preserve(AllMembers = true)]
    public class EnumItemsConverter : DisposableObject, IValueConverter
    {
        private EnumCollection _EnumCollection;
		private int _ConvertedValue;

        public EnumCollection EnumCollection { get { return _EnumCollection; } }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return value;

			Type valueType = value.GetType();

			if (!typeof(EnumCollection).IsAssignableFrom(valueType))
				throw new ArgumentException("value must be an EnumCollection");

			_ConvertedValue = 0;
			var collection = (EnumCollection)value;
			foreach(var item in collection.SelectedItems)
			{
				_ConvertedValue |= (1 << item.Index);
			}

			
			return collection.SelectedItems.Count;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
			if (!typeof(EnumCollection).IsAssignableFrom(targetType))
				throw new ArgumentException("target must be an EnumCollection");

			var intValue = System.Convert.ToInt64(_ConvertedValue);
			if (intValue == -1 )
				intValue = 0;

			var collectionType = typeof(EnumCollection<>);
			var enumType = targetType.GetGenericArguments().FirstOrDefault();
			Type[] generic = { enumType };

			var collection = Activator.CreateInstance(collectionType.MakeGenericType(generic));
			_EnumCollection = (EnumCollection)collection;

			foreach (var item in _EnumCollection)
			{
				item.IsChecked = (intValue & (1 << item.Index)) != 0;
			}

			return collection;
        }

        protected override void Dispose(bool calledExplicitly)
        {
            if (_EnumCollection != null)
                _EnumCollection.Dispose();

            base.Dispose(calledExplicitly);
        }
    }
}
