//
// NotifyCollectionChangedEventArgs.cs
//
// Contact:
//   Moonlight List (moonlight-list@lists.ximian.com)
//
// Copyright 2008 Novell, Inc.
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
using System.Collections.Generic;

namespace System.Collections.Specialized 
{

    public sealed class NotifyCollectionChangedEventArgs : EventArgs
	{
        private List<object> _NewItems, _OldItems;
        
		public NotifyCollectionChangedAction Action { get; private set; }

		public IList NewItems
		{
			get { return _NewItems; }
		}

		public IList OldItems 
		{
			get { return _OldItems; }
		}

		public int NewStartingIndex { get; private set; }
		public int OldStartingIndex { get; private set; }

        public NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction action)
        {
            if (action != NotifyCollectionChangedAction.Reset)
                throw new NotSupportedException ();

            Action = action;
            NewStartingIndex = -1;
            OldStartingIndex = -1;
        }

        public NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction action, object changedItem, int index)
        {
            switch (action) 
			{
            case NotifyCollectionChangedAction.Add:
                _NewItems = new List<object>();
                _NewItems.Add(changedItem);
                NewStartingIndex = index;
                OldStartingIndex = -1;
                break;
            case NotifyCollectionChangedAction.Remove:
                _OldItems = new List<object>();
                _OldItems.Add(changedItem);
                OldStartingIndex = index;
                NewStartingIndex = -1;
                break;
            default:
                throw new NotSupportedException();
            }

            Action = action;
        }

        public NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction action, object newItem, object oldItem, int index)
        {
            if (action != NotifyCollectionChangedAction.Replace)
                throw new NotSupportedException();

            Action = action;

            _NewItems = new List<object>();
            _NewItems.Add(newItem);

            _OldItems = new List<object>();
            _OldItems.Add(oldItem);

            NewStartingIndex = index;
            OldStartingIndex = -1;
        }
    }
}