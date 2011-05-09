//
// MultiselectCollectionConverter.cs: 
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
	using System.Reflection;
	using System.Linq;
	using MonoTouch.Foundation;
	
	[Preserve(AllMembers = true)]
    public class MultiselectCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return value;

			Type valueType = value.GetType();

			if (!valueType.IsAssignableToGenericType(typeof(IMultiselectCollection<>)))
				throw new ArgumentException("value must be an MultiselectCollection");
			
			var genericType = valueType.GetGenericArguments().FirstOrDefault();
			Type[] generic = { genericType };
		
			MethodInfo method = typeof(MultiselectCollectionConverter).GetMethod("CollectionToInt", BindingFlags.NonPublic | BindingFlags.Static);
			MethodInfo genericMethod = method.MakeGenericMethod(generic);
			var convertedValue = genericMethod.Invoke(null, new object[] { value });

			return convertedValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
			if (!targetType.IsAssignableToGenericType(typeof(IMultiselectCollection<>)))
				throw new ArgumentException("target must be an MultiselectCollection");

			var intValue = System.Convert.ToInt64(value);
			if (intValue == -1 )
				intValue = 0;

			var genericType = targetType.GetGenericArguments().FirstOrDefault();
			Type[] generic = { genericType };
			
			MethodInfo method = typeof(MultiselectCollectionConverter).GetMethod("IntToCollection", BindingFlags.NonPublic | BindingFlags.Static);
			MethodInfo genericMethod = method.MakeGenericMethod(generic);
			var multiCollection = genericMethod.Invoke(null, new object[] { intValue });

			return multiCollection;
        }

		private static object IntToCollection<T>(Int64 value)
		{
			var collection = new MultiselectCollection<T>();

			foreach (var item in collection.Selected)
			{
				item.IsSelected = (value & (1 << item.Index)) != 0;
			}
			
			return collection;
		} 

		private static Int64 CollectionToInt<T>(object collection)
		{
			var multiCollection = collection as MultiselectCollection<T>;

			var convertedValue = 0;
			var items = multiCollection.Selected;
			foreach (var item in items)
			{
				convertedValue |= (1 << item.Index);
			}

			return convertedValue;
		}
    }
}
