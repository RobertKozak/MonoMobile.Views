//
// EnumCollection.cs: Generic Collection of Enum Values
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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class EnumCollection<T> : EnumCollection where T : struct
    {
        public EnumCollection() : base(typeof(T))
        {
        }
    }

    public class EnumCollection : ObservableCollection<EnumBinder>, IDisposable
    {
        public Type EnumType { get; set; }

        public ICollection<EnumBinder> SelectedItems
        {
            get { return this.Where(binder => binder.IsChecked).ToList(); }
        }

        public ICollection<EnumBinder> Items
        {
            get { return this.ToList(); }
        }

        private bool _IsOrdered { get; set; }

        public EnumCollection(Type type) : this(type, false) { }

		public EnumCollection(Type type, bool isOrdered)
        {
            _IsOrdered = isOrdered;

            if (type == null || !type.IsEnum)
                throw new ArgumentException("This class only supports Enum types");

            EnumType = type;

            var enumItems = from field in EnumType.GetFields()
                            where field.IsLiteral
                            select field.Name;

            if (_IsOrdered)
                enumItems = from item in enumItems
                            orderby item
                            select item;

            foreach (var item in enumItems)
            {
                Add(new EnumBinder()
                {
                    GroupName = EnumType.FullName,
                    Index = EnumExtensions.GetValueFromString(EnumType, item),
                    FieldName = item,
                    Description = EnumExtensions.GetDescriptionValue(item, EnumType)
                });
            }
        }

        ~EnumCollection()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool calledExplicitly)
        {
            foreach (var item in Items)
            {
                item.Dispose();
            }
        }
    }
}